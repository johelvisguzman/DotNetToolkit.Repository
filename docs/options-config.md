**Repository Options Configuration**

The repositories can be configure using the RepositoryOptions object, which can use ORM frameworks like Entity Framework.
The RepositoryOptions object is built using the RepositoryOptionsBuilder. 

Currently the following are supported:

| Package | NuGet Stable  |
| ------- | ------------- |
[DotNetToolkit.Repository.EntityFramework](https://www.nuget.org/packages/DotNetToolkit.Repository.EntityFramework/) | [![DotNetToolkit.Repository.EntityFramework](https://img.shields.io/nuget/v/DotNetToolkit.Repository.EntityFramework.svg)](https://www.nuget.org/packages/DotNetToolkit.Repository.EntityFramework/) |
[DotNetToolkit.Repository.EntityFrameworkCore](https://www.nuget.org/packages/DotNetToolkit.Repository.EntityFrameworkCore/) | [![DotNetToolkit.Repository.EntityFrameworkCore](https://img.shields.io/nuget/v/DotNetToolkit.Repository.EntityFrameworkCore.svg)](https://www.nuget.org/packages/DotNetToolkit.Repository.EntityFrameworkCore/) |
[DotNetToolkit.Repository.AdoNet](https://www.nuget.org/packages/DotNetToolkit.Repository.AdoNet/) | [![DotNetToolkit.Repository.AdoNet](https://img.shields.io/nuget/v/DotNetToolkit.Repository.AdoNet.svg)](https://www.nuget.org/packages/DotNetToolkit.Repository.AdoNet/) |
[DotNetToolkit.Repository.InMemory](https://www.nuget.org/packages/DotNetToolkit.Repository.InMemory/) | [![DotNetToolkit.Repository.InMemory](https://img.shields.io/nuget/v/DotNetToolkit.Repository.InMemory.svg)](https://www.nuget.org/packages/DotNetToolkit.Repository.InMemory/) |









```csharp
var options = new RepositoryOptionsBuilder()
    .UseEntityFramework<AppDbContext>(...) // for entity framework 6
    .UseEntityFrameworkCore<AppDbContext>(...) // for entity framework core
    .UseAdoNet(...) // for pure ado.net (for testing purposes only)
    .UseInMemoryDatabase(...) // for an in-memory database (for testing purposes only)
    .Options;
```

Activity within a repository can be intercepted by registering an interceptor (This would be helpful if we need to update an object timestamp before adding it to the database or something)

```csharp
var options = new RepositoryOptionsBuilder()
    .UseInterceptor(new AuditRepositoryInterceptor())
    .Options;
```

We can even register a logger provider which will output any logging being done in the repositories (this will include logging any raw SQL query string being executed)

```csharp
var options = new RepositoryOptionsBuilder()
    .UseLoggerProvider(new ConsoleLoggerProvider())
    .Options;
```

We can also use configuration files (which can be defined with interceptors)

```csharp
var options = new RepositoryOptionsBuilder()
    .UseConfiguration() // for net451, it will use the App.config file
    .UseConfiguration(Microsoft.Extensions.Configuration.IConfiguration) // for .netstandard2_0
    .Options;
```

For more configuration details, please check out the [Config File Setup](https://github.com/johelvisguzman/DotNetToolkit.Repository/blob/dev/docs/config-file-setup.md) guide.
