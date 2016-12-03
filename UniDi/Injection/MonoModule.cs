using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UniDi.Injection;
using UniDi.Interfaces;

namespace UniDi
{
    public abstract class MonoModule : MonoBehaviour, IModule
    {
        public Container Container { get; private set; }
        public bool InjectManually = false;
        private List<ITickable> _tickables = new List<ITickable>();

        public bool Eanbled
        {
            get { return enabled; }
        }

        private void Awake()
        {
            Container = new Container();
            SetupDependencies();
        }

        private void Update()
        {
            foreach (var tickable in _tickables)
            {
                tickable.Tick();
            }
        }

        private void Start()
        {
            if (!InjectManually)
                InjectDependencies();
        }

        public void InjectDependencies()
        {
            var injector = new Injector(Container);
            injector.Inject();
            _tickables.AddRange(injector.Tickables);
        }

        public void RegisterSubModule(IModule module)
        {
            module.Container.SetParentContainer(Container);
        }

        /// <summary>
        /// Method where all dependencies should be registered
        /// </summary>
        public abstract void SetupDependencies();
    }
}