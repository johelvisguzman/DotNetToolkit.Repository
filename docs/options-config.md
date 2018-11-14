**Repository Options Configuration**

The repositories can be configure using the RepositoryOptions object, which can use ORM frameworks like Entity Framework.
The RepositoryOptions object is built using the RepositoryOptionsBuilder. 

Currently the following are supported:

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

```csharp
namespace ExampleApplicationDemo
{
    using DotNetToolkit.Repository.Configuration.Interceptors;
    using System;

    public class AuditRepositoryInterceptor : RepositoryInterceptorBase
    {
        private readonly string _user;

        public AuditRepositoryInterceptor(string loggedInUser)
        {
            _user = loggedInUser;
        }

        public override void AddExecuting<TEntity>(TEntity entity)
        {
            if (entity is IHaveTimeStamp haveStamp)
            {
                var currentTime = DateTime.UtcNow;

                haveStamp.CreateTime = currentTime;
                haveStamp.CreateUser = _user;
                haveStamp.ModTime = currentTime;
                haveStamp.ModUser = _user;
            }
        }

        public override void UpdateExecuting<TEntity>(TEntity entity)
        {
            if (entity is IHaveTimeStamp haveStamp)
            {
                var currentTime = DateTime.UtcNow;

                haveStamp.ModTime = currentTime;
                haveStamp.ModUser = _user;
            }
        }
    }
}
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

For more configuration details, please check out the [Config File Setup](/docs/config-file-setup.md) guide.
