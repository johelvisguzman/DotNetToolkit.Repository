namespace DotNetToolkit.Repository.Wpf.Demo
{
    using DotNetToolkit.Wpf.Mvvm;
    using Extensions.Unity;
    using InMemory;
    using Services;
    using System.Windows;
    using Unity;
    using Unity.Injection;
    using ViewModels;
    using Views;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public App()
        {
            var container = new UnityContainer();

            container.RegisterType<MainWindowViewModel>();
            container.RegisterType<CustomerWorkspaceViewModel>();
            container.RegisterType<IServiceLocator, UnityServiceLocator>(new InjectionConstructor(container));
            container.RegisterRepositories(options => options.UseInMemoryDatabase());

            ViewModelLocator.SetDefaultViewModelFactory(type => container.Resolve(type));
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            new MainWindowView().Show();
        }
    }
}
