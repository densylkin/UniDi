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
            var context = new Context();
            context.Register<TestClass>().AsSingle();

            var resolvedObj1 = context.Resolve<TestClass>();
            var resolvedObj2 = context.Resolve<TestClass>();
            
            Assert.AreSame(resolvedObj1, resolvedObj2);
        }

        [Test]
        public void Resolve_Instance()
        {
            var context = new Context();
            var instance = new TestClass();

            context.Register(instance);

            var resolvedInstance = context.Resolve<TestClass>();
            Assert.AreSame(instance, resolvedInstance);
        }

        [Test]
        public void Resolve_Multi()
        {
            var context = new Context();
            context.Register<TestClass>().AsTransient();

            var resolvedObj1 = context.Resolve<TestClass>();
            var resolvedObj2 = context.Resolve<TestClass>();

            Assert.NotNull(resolvedObj1);
            Assert.NotNull(resolvedObj2);
            Assert.AreNotSame(resolvedObj1, resolvedObj2); 
        }

        [Test]
        public void Resolve_Factory()
        {
            var context = new Context();
            context.Register<TestClass>().AsFactory<TestFactory>();

            var resolvedObj1 = context.Resolve<TestClass>();
            var resolvedObj2 = context.Resolve<TestClass>();

            Assert.NotNull(resolvedObj1);
            Assert.NotNull(resolvedObj2);
            Assert.AreNotSame(resolvedObj1, resolvedObj2);
        }
    }
}