# How to Use?

```csharp
// microsoft's dependency injection service collection
var services new ServiceCollection();

// new extension methods for the services, which will register all the repositories
services.AddRepositories(options => options.UseInMemoryDatabase());
```
