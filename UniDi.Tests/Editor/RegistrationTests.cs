using UnityEngine;
using UnityEditor;
using NUnit.Framework;

namespace UniDi.Tests
{
    public class RegistrationTests
    {
        [Test]
        public void InstanceOnly_CanRegister()
        {
            var container = new Container();
            container.Register<TestClass>();
        }

        [Test]
        public void InstanceOnly_Named_CanRegister()
        {
            var container = new Container();
            container.Register<TestClass>("test");
        }

        [Test]
        public void SameUnnamed_Isntances_CanRegister()
        {
            var container = new Container();
            Assert.Catch(() =>
            {
                container.Register<TestClass>();
                container.Register<TestClass>();
            });
        }

        [Test]
        public void SameNamed_Instances_CanRegister()
        {
            var container = new Container();
            Assert.Catch(() =>
            {
                container.Register<TestClass>("test");
                container.Register<TestClass>("test");
            });
        }

        [Test]
        public void Implementation_CanRegister()
        {
            var container = new Container();
            container.Register<ITestInterface, TestImplementation>();
        }

        [Test]
        public void Implementation_Named_CanRegister()
        {
            var container = new Container();
            container.Register<ITestInterface, TestImplementation>("test");
        }

        [Test]
        public void SameUnnamed_implementation_CanRegister()
        {
            var container = new Container();
            Assert.Catch(() =>
            {
                container.Register<ITestInterface, TestImplementation>();
                container.Register<ITestInterface, TestImplementation>();
            });
        }

        [Test]
        public void SameNamed_Implementation_CanRegister()
        {
            var container = new Container();
            Assert.Catch(() =>
            {
                container.Register<ITestInterface, TestImplementation>("test");
                container.Register<ITestInterface, TestImplementation>("test");
            });
        }
    }
}