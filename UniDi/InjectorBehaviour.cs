using UnityEngine;
using System.Collections;
using System.Diagnostics;

namespace UniDi
{
    public class InjectorBehaviour : MonoBehaviour
    {
        public bool Inject;
        protected Context Context;

        private void Awake()
        {
            Context = new Context();
            Initialize();
            if (Inject)
            {
                Context.Inject();
            }
        }

        private void Update()
        {
            foreach (var tickable in Context.Tickables)
            {
                tickable.Tick();
            }
        }

        /// <summary>
        /// Method where all dependencies should be registered
        /// </summary>
        protected virtual void Initialize()
        {
            
        }
    }
}