using System;
using System.Collections.Generic;
using System.Reflection;
using UniDi.Interfaces;

namespace UniDi
{
    public class Container
    {
        public Dictionary<Type, Dictionary<string, IDependencyRegistration>> Registrations { get; private set; }

        private Container _parentContainer;

        public Container()
        {
            Registrations = new Dictionary<Type, Dictionary<string, IDependencyRegistration>>();
        }

        #region Registration

        /// <summary>
        /// Register implementation only
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DependencyRegistration<T> Register<T>() where T : class, new()
        {
            return Register<T>(string.Empty);
        }

        /// <summary>
        /// Register implementation only with name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public DependencyRegistration<T> Register<T>(string name) where T : class, new()
        {
            var type = typeof(T);

            Dictionary<string, IDependencyRegistration> registrations;
            if (!Registrations.TryGetValue(typeof(T), out registrations))
            {
                registrations = new Dictionary<string, IDependencyRegistration>();
                Registrations.Add(type, registrations);
            }

            if (registrations.ContainsKey(name))
                throw new RegistrationException(
                    string.IsNullOrEmpty(name) ?
                    "Already registered unnamed dependency." :
                    ("Already registered dependency with name : " + name));

            var reg = new DependencyRegistration<T>();
            reg.AsTransient();
            registrations.Add(name, reg);
            return reg;
        }


        /// <summary>
        /// Register instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public void Register<T>(T instance) where T : class, new()
        {
            Register<T>(instance, string.Empty);
        }


        /// <summary>
        /// Register instance with name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        public void Register<T>(T instance, string name) where T : class, new()
        {
            Register<T>(name).AsInstance(instance);
        }

        /// <summary>
        /// Register implementation of itnerface
        /// </summary>
        /// <typeparam name="IT"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public DependencyRegistration<T> Register<IT, T>() where T : class, new()
        {
            return Register<IT, T>(string.Empty);
        }

        /// <summary>
        /// Register implementation of itnerface with name
        /// </summary>
        /// <typeparam name="IT"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public DependencyRegistration<T> Register<IT, T>(string name) where T : class, new()
        {
            var interfaceType = typeof(IT);
            var type = typeof(T);

            if (!interfaceType.IsInterface)
                throw new RegistrationException(interfaceType + " is not an interface");

            if (type.IsAssignableFrom(interfaceType))
                throw new RegistrationException(type + " does not implement " + interfaceType);

            Dictionary<string, IDependencyRegistration> registrations;
            if (!Registrations.TryGetValue(typeof(T), out registrations))
            {
                registrations = new Dictionary<string, IDependencyRegistration>();
                Registrations.Add(interfaceType, registrations);
            }

            if (registrations.ContainsKey(name))
                throw new RegistrationException(
                    string.IsNullOrEmpty(name) ?
                    "Already registered unnamed dependency." :
                    ("Already registered dependency with name : " + name));

            var reg = new DependencyRegistration<T>();
            reg.AsSingle();
            registrations.Add(name, reg);
            return reg;
        }

        /// <summary>
        /// Register intance of type that implements interface
        /// </summary>
        /// <typeparam name="IT"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        public void Register<IT, T>(T instance) where T : class, new()
        {
            Register<IT, T>().AsInstance(instance);
        }


        /// <summary>
        /// Register intance of type that implements interface with name
        /// </summary>
        /// <typeparam name="IT"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        public void Register<IT, T>(T instance, string name) where T : class, new()
        {
            Register<IT, T>(name).AsInstance(instance);
        }

        #endregion

        #region Resolving

        /// <summary>
        /// Resolve dependency of type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Resolve(Type type)
        {
            return Resolve(type, string.Empty);
        }

        /// <summary>
        /// Resolve named dependency of type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public object Resolve(Type type, string name)
        {
            object result;
            if (TryResolve(type, name, out result) || (_parentContainer != null && _parentContainer.TryResolve(type, name, out result)))
                return result;
            else
                throw new ResolvingException("Could not resolve dependency of type " + type.FullName);
        }

        /// <summary>
        /// Resolve dependency of type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        /// <summary>
        /// Resolve named dependency of type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T Resolve<T>(string name)
        {
            return (T)Resolve(typeof(T), name);
        }

        private bool TryResolve(Type type, out object obj)
        {
            return TryResolve(type, string.Empty, out obj);
        }

        private bool TryResolve(Type type, string name, out object obj)
        {
            Dictionary<string, IDependencyRegistration> registrations;
            if (Registrations.TryGetValue(type, out registrations))
            {
                IDependencyRegistration registration;
                if (registrations.TryGetValue(name, out registration))
                {
                    obj = registration.Provider.Create();
                    return true;
                }
                else
                    throw new ResolvingException(
                        string.IsNullOrEmpty(name) ?
                            ("No unnamed registrations for " + type + " found.")
                            : ("No registrations with name " + name + " for " + type + " found"));
            }
            else
                throw new ResolvingException("No registrations for " + type + " found.");
        }

        #endregion

        public void SetParentContainer(Container container)
        {
            _parentContainer = container;
        }
    }

    #region InjectAttribute
    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
        public string Name { get; private set; }

        public InjectAttribute()
        {
        }

        public InjectAttribute(string name)
        {
            Name = name;
        }
    }
    #endregion
}