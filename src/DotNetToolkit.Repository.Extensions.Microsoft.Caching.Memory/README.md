# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
     .UseCachingProvider(new InMemoryCacheProvider())
     .Options;

var repo = new Repository<Customer>(options);
```
