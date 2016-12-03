#if UNITY_STANDALONE || UNITY_EDITOR
#define USE_FAST_REFLECTION
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UniDi;
using UniDi.Interfaces;
using Vexe.Runtime.Extensions;

namespace UniDi.Injection
{
    public class Injector
    {
        public List<IInitializable> Initializables { get; private set; }
        public List<ITickable> Tickables { get; private set; }

        private readonly Container _container;
        private readonly ReflectedMembersCache _reflectedMembers;

        private Dictionary<Type, object> _constructedInstances;

        public Injector(Container container)
        {
            _container = container;
            _constructedInstances = new Dictionary<Type, object>();
            _reflectedMembers = new ReflectedMembersCache();
            _reflectedMembers.AddRange(_container.Registrations.Keys);
            Tickables = new List<ITickable>();
        }

        public void Inject()
        {
            var types = _reflectedMembers.Types;
            foreach (var type in types)
            {
                var entry =  _reflectedMembers[type];

                object injectable;
                if (entry.Constructors.Count > 0)
                    injectable = InjectToConstructor(type, entry.Constructors.First());
                else
                    injectable = _container.Resolve(type);

                entry.Properties.ForEach(p => InjectToPropery(p, injectable));
                entry.Methods.ForEach(m => InjectToMethod(m, injectable));

                ProcessEvents(injectable);
            }
        }

        private object InjectToConstructor(Type type, ConstructorInfo ctorInfo)
        {
            var values = ResolveParameters(ctorInfo.GetParameters());
            foreach (var value in values)
            {
                ProcessEvents(value);
            }
#if USE_FAST_REFLECTION
            var del = type. DelegateForCtor(ctorInfo.GetParameters().Select(p => p.ParameterType).ToArray()); 
            return del(values);
#else
            return ctorInfo.Invoke(values);
#endif
        }

        private void InjectToPropery(PropertyInfo property, object injectable)
        {
            var attribute = property.GetInjectAttribute();
            object value;
            if (string.IsNullOrEmpty(attribute.Name))
                value = _container.Resolve(property.PropertyType);
            else
                value = _container.Resolve(property.PropertyType, attribute.Name);
            ProcessEvents(value);
#if USE_FAST_REFLECTION
            var del = property.DelegateForSet();
            del(ref injectable, value);
#else
            property.SetValue(injectable, value, null);
#endif
        }

        private void InjectToMethod(MethodInfo method, object injectable)
        {
            var values = ResolveParameters(method.GetParameters());
            foreach (var value in values)
            {
                ProcessEvents(value);
            }
#if USE_FAST_REFLECTION
            var del = method.DelegateForCall();
            del(injectable, values);
#else
            method.Invoke(injectable, values);
#endif
        }

        private object[] ResolveParameters(ParameterInfo[] parameters)
        {
            var parametersValues = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                var param = parameters[i];
                var attribute = param.GetCustomAttributes(typeof(InjectAttribute), false).Cast<InjectAttribute>().FirstOrDefault();
                if (attribute != null && !string.IsNullOrEmpty(attribute.Name))
                    parametersValues[i] = _container.Resolve(param.ParameterType, attribute.Name);
                else
                    parametersValues[i] = _container.Resolve(param.ParameterType);
            }
            return parametersValues;
        }

        private void ProcessEvents(object target)
        {
            var initializable = target as IInitializable;
            if (initializable != null)
                initializable.Initialize();

            var tickable = target as ITickable;
            if (tickable != null)
                Tickables.Add(tickable);
        }
    }
}