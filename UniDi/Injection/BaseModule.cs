namespace UniDi.Injection
{

    public abstract class BaseModule : IModule
    {
        public Container Container { get; private set; }

        protected BaseModule()
        {
            Container = new Container();
        }

        public void RegisterSubModule(IModule module)
        {
            throw new System.NotImplementedException();
        }

        public abstract void SetupDependencies();

        public void InjectDependencies()
        {
            var injector = new Injector(Container);
            injector.Inject();
        }
    }
}