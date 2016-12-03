using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace UniDi.Tests
{
    public class ResolvingTests
    {
        [Test]
        public void Resolve_Single()
        {
            var container = new Container();
            container.Register<TestClass>().AsSingle();

            var resolvedObj1 = container.Resolve<TestClass>();
            var resolvedObj2 = container.Resolve<TestClass>();
            
            Assert.AreSame(resolvedObj1, resolvedObj2);
        }

        [Test]
        public void Resolve_Instance()
        {
            var container = new Container();
            var instance = new TestClass();

            container.Register(instance);

            var resolvedInstance = container.Resolve<TestClass>();
            Assert.AreSame(instance, resolvedInstance);
        }

        [Test]
        public void Resolve_Multi()
        {
            var container = new Container();
            container.Register<TestClass>().AsTransient();

            var resolvedObj1 = container.Resolve<TestClass>();
            var resolvedObj2 = container.Resolve<TestClass>();

            Assert.NotNull(resolvedObj1);
            Assert.NotNull(resolvedObj2);
            Assert.AreNotSame(resolvedObj1, resolvedObj2); 
        }

        [Test]
        public void Resolve_Factory()
        {
            var container = new Container();
            container.Register<TestClass>().AsFactory<TestFactory>();

            var resolvedObj1 = container.Resolve<TestClass>();
            var resolvedObj2 = container.Resolve<TestClass>();

            Assert.NotNull(resolvedObj1);
            Assert.NotNull(resolvedObj2);
            Assert.AreNotSame(resolvedObj1, resolvedObj2);
        }
    }
}