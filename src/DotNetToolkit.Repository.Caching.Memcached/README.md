# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
     .UseMemcached(...)
     .Options;

var repo = new Repository<Customer>(options);
```
