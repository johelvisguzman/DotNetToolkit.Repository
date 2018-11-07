![alt text](https://raw.githubusercontent.com/johelvisguzman/DotNetToolkit.Repository/master/Toolkit.png)

# DotNetToolkit.Repository [![Build status](https://ci.appveyor.com/api/projects/status/v02h9efjwev30sof?svg=true)](https://ci.appveyor.com/project/johelvisguzman/dotnettoolkit-repository)
A toolkit for using Repository pattern in C# .NET

**What is the Repository Pattern?**

An abstraction of the data layer and handling common data access functionality.

**How to use**

The repository needs to be manually configured with a RepositoryOptions object, which is built
with an ORM framework like Entity Framework.

The RepositoryOptions object is built using the RepositoryOptionsBuilder

```csharp
var options = new RepositoryOptionsBuilder()
    .UseEntityFramework<AppDbContext>()
    .Options;

var repo = new Repository<Customer>(options);
```

**Features**

The DotNetToolkit.Repository includes basic repository objects for handling the data access layer as well as services and even unit of work for transactions.