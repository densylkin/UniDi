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
            var context = new Context();
            context.Register<TestClass>();
        }

        [Test]
        public void InstanceOnly_Named_CanRegister()
        {
            var context = new Context();
            context.Register<TestClass>("test");
        }

        [Test]
        public void SameUnnamed_Isntances_CanRegister()
        {
            var context = new Context();
            Assert.Catch(() =>
            {
                context.Register<TestClass>();
                context.Register<TestClass>();
            });
        }

        [Test]
        public void SameNamed_Instances_CanRegister()
        {
            var context = new Context();
            Assert.Catch(() =>
            {
                context.Register<TestClass>("test");
                context.Register<TestClass>("test");
            });
        }

        [Test]
        public void Implementation_CanRegister()
        {
            var context = new Context();
            context.Register<ITestInterface, TestImplementation>();
        }

        [Test]
        public void Implementation_Named_CanRegister()
        {
            var context = new Context();
            context.Register<ITestInterface, TestImplementation>("test");
        }

        [Test]
        public void SameUnnamed_implementation_CanRegister()
        {
            var context = new Context();
            Assert.Catch(() =>
            {
                context.Register<ITestInterface, TestImplementation>();
                context.Register<ITestInterface, TestImplementation>();
            });
        }

        [Test]
        public void SameNamed_Implementation_CanRegister()
        {
            var context = new Context();
            Assert.Catch(() =>
            {
                context.Register<ITestInterface, TestImplementation>("test");
                context.Register<ITestInterface, TestImplementation>("test");
            });
        }
    }
}