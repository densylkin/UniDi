using UnityEngine;
using System.Collections;
using UniDi;

public class BaseSetup : InjectorBehaviour 
{
    protected override void Initialize()
    {
        Context.Register<TestDependency>().AsSingle();
        Context.Register<TestClass>();
    }
}

public class TestClass : IInitializable, ITickable
{
    private TestDependency _testDependency;

    public TestClass()
    {
    }

    [Inject]
    public TestClass(TestDependency testDependency)
    {
        _testDependency = testDependency;
    }

    public void Initialize()
    {
        Debug.Log("initialize");
        Debug.Log(_testDependency.text);
    }

    public void Tick()
    {
        Debug.Log("Aatat");
    }
}

public class TestDependency
{
    public string text = "Hello world!";
}