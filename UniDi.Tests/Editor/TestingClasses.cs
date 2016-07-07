namespace UniDi.Tests
{
    public class TestClass
    {
        
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