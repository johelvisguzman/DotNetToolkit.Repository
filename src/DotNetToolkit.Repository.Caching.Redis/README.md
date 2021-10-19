# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
     .UseRedis(...)
     .Options;

var repo = new Repository<Customer>(options);
```
