namespace UniDi.Tests
{
    public class TestClass
    {
        [Inject]
        public TestImplementation Dependency { get; set; }
    }

    public class TestConstructorClass
    {
        private TestDependency Dependency;

        public TestConstructorClass()
        {
        }

        [Inject]
        public TestConstructorClass(TestDependency dependency)
        {
            Dependency = dependency;
        }
    }

    public class TestMethodClass
    {
        public TestDependency Dependency;

        [Inject]
        public void TestMethod(TestDependency dependency)
        {
            Dependency = dependency;
        }
    }

    public interface ITestInterface
    {

    }

    public class TestImplementation : ITestInterface
    {

    }

    public class TestFactory : IFactory
    {
        public object Create()
        {
            return new TestClass();
        }
    }
}