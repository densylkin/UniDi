using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using UniDi.Injection;
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
            var container = new Container();
            container.Register<TestImplementation>().AsInstance(testdpendency);
            container.Register<TestClass>().AsInstance(testClass);
            new Injector(container).Inject();
            Assert.NotNull(testClass.Dependency);
            Assert.AreSame(testClass.Dependency, testdpendency);
        }

        [Test]
        public void ToConstructor()
        {
            var container = new Container();
            container.Register<TestConstructorClass>().AsSingle();
            container.Register<TestDependency>().AsSingle();
            Assert.DoesNotThrow(() =>
            {
                new Injector(container).Inject();
            });
        }

        [Test]
        public void ToMethod()
        {
            var container = new Container();
            var testDependency = new TestDependency();
            var test = new TestMethodClass();
            container.Register<TestDependency>().AsInstance(testDependency);
            container.Register<TestMethodClass>().AsInstance(test);
            new Injector(container).Inject();
            Assert.NotNull(test.Dependency);
            Assert.AreEqual(test.Dependency, testDependency);
        }
    }
}