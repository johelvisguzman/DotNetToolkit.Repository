# How to Use?

```csharp
// unity's container
var container = new UnityContainer();

// new extension methods for the container, which will register all the repositories
container.RegisterRepositories(options => options.UseInMemoryDatabase());
```
