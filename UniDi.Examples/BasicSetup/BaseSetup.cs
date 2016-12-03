using UnityEngine;
using System.Collections;
using UniDi;
using UniDi.Interfaces;

public class BaseSetup : MonoModule
{
    public override void SetupDependencies()
    {
        Container.Register<TestDependency>().AsSingle();
        Container.Register<TestClass>();
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