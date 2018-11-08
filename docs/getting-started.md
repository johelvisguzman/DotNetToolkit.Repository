**Getting Started**

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
For more configuration details, please check out the [Repository Options Configuration](https://github.com/johelvisguzman/DotNetToolkit.Repository/blob/dev/docs/options-config.md) guide.


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