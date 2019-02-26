namespace DotNetToolkit.Repository.Wpf.Demo.Services
{
    public interface IServiceLocator
    {
        T GetService<T>();
    }
}
