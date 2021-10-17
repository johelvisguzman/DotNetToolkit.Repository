# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
     .UseInMemoryCache(...) // for microsoft in-memory cache
     .Options;

var repo = new Repository<Customer>(options);
```
