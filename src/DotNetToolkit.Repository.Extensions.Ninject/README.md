# How to Use?

```csharp
// kernel
var kernel = new StandardKernel();

// new extension methods for the kernel container, which will bind all the repositories
kernel.BindRepositories(options => options.UseInMemoryDatabase());
```
