# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
     .UseCachingProvider(new CouchbaseCacheProvider())
     .Options;

var repo = new Repository<Customer>(options);
```
