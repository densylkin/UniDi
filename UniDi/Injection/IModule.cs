using System.ComponentModel;

namespace UniDi.Injection
{
    public interface IModule
    {
        Container Container { get; }
        void RegisterSubModule(IModule module);
        void SetupDependencies();
        void InjectDependencies();
    }
}