namespace DotNetToolkit.Repository.Wpf.Demo.Services
{
    using Unity;

    public class UnityServiceLocator : IServiceLocator
    {
        private readonly IUnityContainer _container;

        public UnityServiceLocator(IUnityContainer container)
        {
            _container = container;
        }

        public T GetService<T>()
        {
            return _container.Resolve<T>();
        }
    }
}
