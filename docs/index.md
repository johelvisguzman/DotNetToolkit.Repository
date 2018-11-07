![alt text](https://raw.githubusercontent.com/johelvisguzman/DotNetToolkit.Repository/master/Toolkit.png)

# DotNetToolkit.Repository [![Build status](https://ci.appveyor.com/api/projects/status/v02h9efjwev30sof?svg=true)](https://ci.appveyor.com/project/johelvisguzman/dotnettoolkit-repository)
A toolkit for using Repository pattern in C# .NET

**What is the Repository Pattern?**

An abstraction of the data layer and handling common data access functionality.

**How to use**

The repository needs to be manually configured with a RepositoryOptions object, which is built
with an ORM framework like Entity Framework. The RepositoryOptions object is built using the RepositoryOptionsBuilder.

```csharp
var options = new RepositoryOptionsBuilder().Options;
var repo = new Repository<Customer>(options);
```

**Configuration**

There are several ORM providers that are supported with the DotNetToolkit.Repository. Currently the following are supported:

|                                      Providers |
|----------------------------------------------- |
| Entity Framework 6                             |
| Entity Framework Core                          |
| Pure Ado.NET (for testing purposes only)       |
| In Memory Database (for testing purposes only) |


```csharp
var options = new RepositoryOptionsBuilder()
    .UseEntityFramework<AppDbContext>(...) // for entity framework 6
    .UseEntityFrameworkCore<AppDbContext>(...) // for entity framework core
    .UseAdoNet(...) // for pure ado.net (for testing purposes only)
    .UseInMemoryDatabase(...) // for an in-memory database (for testing purposes only)
    .Options;
```

Activity within a repository can be intercepted by registering an interceptor (this is usually if we need to update an object timestamp before adding it to the database or something)

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

We can also use interceptors defined in a configuration file

```csharp
var options = new RepositoryOptionsBuilder()
    .UseConfiguration() // for net451, it will use the App.config file
    .UseConfiguration(Microsoft.Extensions.Configuration.IConfiguration) // for .netstandard2_0
    .Options;
```

**Features**

The DotNetToolkit.Repository includes basic repository objects for handling the data access layer as well as services and even unit of work for transactions. The performance of all the data access functionality can be seen in the most current [DotNetToolkit.Repository.Performance benchmark results](https://github.com/johelvisguzman/DotNetToolkit.Repository/blob/dev/docs/benchmarks.md).
