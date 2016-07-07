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
                Stopwatch watch = new Stopwatch();
                watch.Start();
                Context.Inject();
                watch.Stop();
                UnityEngine.Debug.Log(watch.Elapsed);
            }
        }

        protected virtual void Initialize()
        {
            
        }
    }
}