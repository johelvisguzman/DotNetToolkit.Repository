# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
     .UseCachingProvider(new MemcachedCacheProvider())
     .Options;

var repo = new Repository<Customer>(options);
```
