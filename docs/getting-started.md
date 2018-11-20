# Getting Started

To get started with DotNetToolkit.Repository, please follow these steps.

**Installation**

```
PM> Install-Package DotNetToolkit.Repository.InMemory
```

**How to use**

First we need to define an entity type which will be managed by the repository.

```csharp
namespace MyApplicationDemo
{
	public class Customer
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}
```

Create a RepositoryOptions object with a registered ORM provider to be used (in this example we will be using the in-memory database)
For more configuration details, please check out the [Repository Options Configuration](/docs/options-config.md) guide.

```csharp
using DotNetToolkit.Repository;
using DotNetToolkit.Repository.Configuration.Options;
using DotNetToolkit.Repository.InMemory;

namespace MyApplicationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase()
                .Options;

            var repo = new Repository<Customer>(options);
        }
    }
}
```

Now we can start using the functionality from the repository to create, update, delete, and find entities.

```csharp
using DotNetToolkit.Repository;
using DotNetToolkit.Repository.Configuration.Options;
using DotNetToolkit.Repository.InMemory;

namespace MyApplicationDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // **** Configure Repository **** 
            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase()
                .Options;

            var repo = new Repository<Customer>(options);
			
            // **** Create Items **** 

            var customer = new Customer() { Id = 1, Name = "Random Name" }
			
            repo.Add(customer);
			
            // **** Update Items **** 

            customer.Name = "New Random Name";
			
            repo.Update(customer);
			
            // **** Delete Items **** 
            
            repo.Delete(customer);
			
            // **** Find Items **** 
            
            repo.Find(predicate => predicate.Name.Equals("Random Name"));
        }
    }
}
```

**QueryOptions / QueryResults**

The DotNetToolkit.Repository supports [fetch query strategy](https://blog.willbeattie.com/2011/02/specification-pattern-entity-framework.html), [specification query strategy](https://en.wikipedia.org/wiki/Specification_pattern) and even pagination when it comes to fetching data. These strategies can be setup using a QueryOptions, and the data is returned as a QueryResults. For information, please check out the [QueryOptions and QueryResults](/docs/queries.md) guide.

**Dependency Injection**

The DotNetToolkit.Repository also supports dependency injection. For information regarding dependency injection, please check out the [Dependency Injection](/docs/di.md) guide.
