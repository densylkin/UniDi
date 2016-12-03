using UnityEngine;
using System.Collections;

namespace UniDi.Examples
{
    public class BenchmarkInjector : MonoBehaviour
    {
        private Container _container = new Container();

        public int Iterations = 1000;

        [Inject]
        public TestDependency Dependency1 { get; set; }
        [Inject]
        public TestDependency Dependency2 { get; set; }
        [Inject]
        public TestDependency Dependency3 { get; set; }
        [Inject]
        public TestDependency Dependency4 { get; set; }
        [Inject]
        public TestDependency Dependency5 { get; set; }
        [Inject]
        public TestDependency Dependency6 { get; set; }

        private void Start()
        {
            var watch = new System.Diagnostics.Stopwatch();
            _container.Register<TestDependency>().AsTransient();
            _container.Register(this);
            watch.Start();
            for (int i = 0; i < Iterations; i++)
            {
                //_container.Inject();
                Dependency1 = null;
                Dependency2 = null;
                Dependency3 = null;
                Dependency4 = null;
                Dependency5 = null;
                Dependency6 = null;

            }
            watch.Stop();
            Debug.Log(watch.Elapsed);
        }
    }
}