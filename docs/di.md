# Dependency Injection

The DotNetToolkit.Repository provides dependency injection packages which will register all repositories, interceptors and services automatically. The dependency  injection packages contain an extension method for its specified container which will scan for all IRepository<...>, IService<...> and IRepositoryInterceptor implementations in all the executing assemblies, and will register them so that you don't have to.

Currently, the following containers are supported by the DotNetToolkit.Repository:

**Microsoft DependencyInjection**

The [DotNetToolkit.Repository.Extensions.Microsoft.DependencyInjection](/src/DotNetToolkit.Repository.Extensions.Microsoft.DependencyInjection) provides an extension method for the ServiceCollection called AddRepositories, which will register all repositories.

```csharp
public void ConfigureServices(IServiceCollection services)
{
  services.AddMvc();
	
  services.AddRepositories(options => 
  {
    options.UseInMemoryDatabase();
  });
}
```
