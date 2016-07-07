using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace UniDi
{
    public interface IDependencyRegistration
    {
        IProvider Provider { get; }
        Type GetDependencyType { get; }

        void AsSingle();
        void AsTransient();
        void AsFactory<T>() where T : IFactory, new();
        void AsPrefab(string prefabName);
        void AsPrefab(GameObject prefab);
        void AsResource(string resourceName);
        void AsInstance<T>(T instance) where T : class;
    }

    public class DependencyRegistration<T> : IDependencyRegistration where T : class, new()
    {
        public IProvider Provider { get; private set; }
        public Type GetDependencyType { get { return typeof (T); } }

        public void AsSingle()
        {
            if (GetDependencyType.IsComponent() || GetDependencyType.IsMonoBehaviour())
                Provider = new SingleComponentProvider<T>();
            else
                Provider = new SingleProvider<T>();
        }

        public void AsTransient()
        {
            if (GetDependencyType.IsComponent() || GetDependencyType.IsMonoBehaviour())
                Provider = new MultipleComponentProvider<T>();
            else
                Provider = new TransientProvider<T>();
        }

        public void AsFactory<F>() where F : IFactory, new()
        {
            Provider = new FactoryProvider<F>();
        }

        public void AsInstance<T>(T instance) where T : class
        {
            Provider = new InstanceProvider<T>((T)instance);
        }

        public void AsPrefab(string prefabName)
        {
            Provider = new PrefabProvider<T>(prefabName);
        }

        public void AsPrefab(GameObject prefab)
        {
            Provider = new PrefabProvider<T>(prefab);
        }

        public void AsResource(string resourceName)
        {
            Provider = new ResourceProvider<T>(resourceName);
        }
    }
}