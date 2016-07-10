using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using UnityEngine.Networking;

namespace UniDi.Tests
{
    public class InjectionTests
    {
        [Test]
        public void ToInstance()
        {
            var testClass = new TestClass();
            var testdpendency = new TestImplementation();
            var context = new Context();
            context.Register<TestImplementation>().AsInstance(testdpendency);
            context.Register<TestClass>().AsInstance(testClass);
            context.Inject();
            Assert.NotNull(testClass.Dependency);
            Assert.AreSame(testClass.Dependency, testdpendency);
        }

        [Test]
        public void ToConstructor()
        {
            var context = new Context();
            context.Register<TestConstructorClass>().AsSingle();
            context.Register<TestDependency>().AsSingle();
            Assert.DoesNotThrow(() =>
            {
                context.Inject();
            });
        }

        [Test]
        public void ToMethod()
        {
            var context = new Context();
            var testDependency = new TestDependency();
            var test = new TestMethodClass();
            context.Register<TestDependency>().AsInstance(testDependency);
            context.Register<TestMethodClass>().AsInstance(test);
            context.Inject();
            Assert.NotNull(test.Dependency);
            Assert.AreEqual(test.Dependency, testDependency);
        }
    }
}