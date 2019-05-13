# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
     .UseCachingProvider(new RedisCacheProvider())
     .Options;

var repo = new Repository<Customer>(options);
```
