using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UniDi
{
    public interface IFactory
    {
        object Create();
    }

    public interface IProvider
    {
        object Create();
    }

    #region BaseProviders

    public class SingleProvider<T> : IProvider where T : new()
    {
        private T _instance;

        public object Create()
        {
            if (_instance == null)
                _instance = new T();
            return _instance;
        }
    }

    public class TransientProvider<T> : IProvider where T : new()
    {
        public object Create()
        {
            return new T();
        }
    }

    public class FactoryProvider<T> : IProvider where T : IFactory, new()
    {
        private readonly IFactory _factory;

        public FactoryProvider()
        {
            _factory = new T();
        }

        public object Create()
        {
            return _factory.Create();
        }
    }

    public class InstanceProvider<T> : IProvider where T : class
    {
        private readonly T _instance;

        public InstanceProvider(T instance)
        {
            _instance = instance;
        }

        public object Create()
        {
            return _instance;
        }
    }

    #endregion

    #region UnityProviders

    public class SingleComponentProvider<T> : IProvider
    {
        private GameObject _target;
        private T _instance;

        public SingleComponentProvider()
        {
        }

        public SingleComponentProvider(GameObject target)
        {
            _target = target;
        }

        public object Create()
        {
            var componentType = typeof(T);
            if (componentType is Component)
                return (_target ?? new GameObject(componentType + "GameObject")).AddComponent(componentType);
            else
                throw new ArgumentException(componentType + " is not derived from Component");
        }
    }

    public class MultipleComponentProvider<T> : IProvider
    {
        public object Create()
        {
            var componentType = typeof(T);
            return (new GameObject(componentType + "GameObject")).AddComponent(componentType);
        }
    }

    public class PrefabProvider<T> : IProvider
    {
        private readonly GameObject _prefab;
        private readonly string _prefabResource;

        public PrefabProvider(GameObject prefab)
        {
            _prefab = prefab;
        }

        public PrefabProvider(string prefabResource)
        {
            _prefabResource = prefabResource;
        }

        public object Create()
        {
            var type = typeof(T);

            if (!type.IsMonoBehaviour() && !type.IsComponent())
                throw new ArgumentException(type + " is not a unity component.");


            if (_prefab != null)
            {
                return UnityObject.Instantiate(_prefab).GetComponent<T>();
            }

            if (!string.IsNullOrEmpty(_prefabResource))
            {
                return ((GameObject)UnityObject.Instantiate(Resources.Load(_prefabResource))).GetComponent<T>();
            }

            return null;
        }
    }

    public class ResourceProvider<T> : IProvider
    {
        private readonly string _resourceName;
        private T _resourceCache;

        public ResourceProvider(string resourceName)
        {
            _resourceName = resourceName;
        }

        public object Create()
        {
            if (typeof(T).IsAssignableFrom(typeof(UnityObject)))
                return Resources.Load(_resourceName);
            else
                throw new ArgumentException(typeof(T) + " is not a unity type.");
        }
    }
    #endregion
}