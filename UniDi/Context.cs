using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Networking.NetworkSystem;
using Vexe.Runtime.Extensions;

namespace UniDi
{
    public class Context
    {
        private Dictionary<Type, MemberInfo[]> _membersCache;
        private Dictionary<Type, Dictionary<string, IDependencyRegistration>> _registrations;
        private Dictionary<Type, object> _constructedInstances; 

        public Context()
        {
            _membersCache = new Dictionary<Type, MemberInfo[]>();
            _registrations = new Dictionary<Type, Dictionary<string, IDependencyRegistration>>();
            _constructedInstances = new Dictionary<Type, object>();
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
            if (!_registrations.TryGetValue(typeof(T), out registrations))
            {
                registrations = new Dictionary<string, IDependencyRegistration>();
                _registrations.Add(type, registrations);
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
            if (!_registrations.TryGetValue(typeof(T), out registrations))
            {
                registrations = new Dictionary<string, IDependencyRegistration>();
                _registrations.Add(interfaceType, registrations);
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
            Dictionary<string, IDependencyRegistration> registrations;
            if (_registrations.TryGetValue(type, out registrations))
            {
                IDependencyRegistration registration;
                if (registrations.TryGetValue(name, out registration))
                {
                    return registration.Provider.Create();
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

        #endregion

        #region Injection

        public void Inject()
        {
            var typesToInject = _registrations.Keys.Where(t =>
            {
                var members = t.GetMembersToInject();
                if (members.Length == 0)
                    return false;
                else
                {
                    if(!_membersCache.ContainsKey(t))
                        _membersCache.Add(t, members);
                    return true;
                }
            });

            foreach (var type in typesToInject)
            {
                IDependencyRegistration reg;
                if (!_registrations[type].TryGetValue(string.Empty, out reg))
                {
                    throw new InjectionException("Cannot inject to " + type + " beacause no unnamed registrations found.");
                }

                var members = _membersCache[type];
                var ctorInfo = members.FirstOrDefault(m => m is ConstructorInfo) as ConstructorInfo;
                object injectable = null;

                if (ctorInfo != null)
                {
                    injectable = InjectToConstructor(type, ctorInfo);
                    _constructedInstances.Add(type, injectable);
                }

                if (injectable == null)
                    injectable = Resolve(type);

                foreach (var member in members)
                {
                    if (member.IsProperty())
                    {
                        var property = (PropertyInfo) member;
                        var attribute = property.GetCustomAttributes(typeof (InjectAttribute), false).First() as InjectAttribute;
                        object value;
                        if (attribute != null && !string.IsNullOrEmpty(attribute.Name))
                            value = Resolve(property.PropertyType, attribute.Name);
                        else
                            value = Resolve(property.PropertyType);


                        InjectToPropery((PropertyInfo)member, value, ref injectable);
                    }

                    if (member.IsMethod())
                    {
                        var method = (MethodInfo) member;

                        var parameters = method.GetParameters();
                        var parametersValues = new object[parameters.Length];

                        for (int i = 0; i < parameters.Length; i++)
                        {
                            var param = parameters[i];
                            var attribute = param.GetCustomAttributes(typeof(InjectAttribute), false).First() as InjectAttribute;
                            object value;
                            if (attribute != null && !string.IsNullOrEmpty(attribute.Name))
                                value = Resolve(param.ParameterType, attribute.Name);
                            else
                                value = Resolve(param.ParameterType);

                            parametersValues[i] = value;
                        }

                        InjectToMethod(method, parametersValues, injectable);
                    }
                }

                var properties = members.Where(m => m is PropertyInfo).Cast<PropertyInfo>();

                var initializable = injectable as IInitializable;
                if(initializable != null)
                    initializable.Initialize();
            }
        }

        private object InjectToConstructor(Type type, ConstructorInfo ctorInfo)
        {
            var ctorParameters = ctorInfo.GetParameters();
            var parametersValues = new object[ctorParameters.Length];
            for (int i = 0; i < ctorParameters.Length; i++)
            {
                var param = ctorParameters[i];
                var attribute = param.GetCustomAttributes(typeof (InjectAttribute), false).Cast<InjectAttribute>().FirstOrDefault();
                if (attribute != null && !string.IsNullOrEmpty(attribute.Name))
                    parametersValues[i] = Resolve(param.ParameterType, attribute.Name);
                else
                    parametersValues[i] = Resolve(param.ParameterType);
            }
            var del = type.DelegateForCtor(ctorParameters.Select(p => p.ParameterType).ToArray());
            return del(parametersValues);
        }

        private void InjectToPropery(PropertyInfo property, object value, ref object injectable)
        {
            var del = property.DelegateForSet();
            del(ref injectable, value);
        }

        private void InjectToMethod(MethodInfo method, object[] values, object injectable)
        {
            var del = method.DelegateForCall();
            del(injectable, values);
        }

        #endregion

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

    #region IInitializable, ITickable

    public interface IInitializable
    {
        void Initialize();
    }

    public interface ITickable
    {
        void Tick();
    }

    #endregion
}