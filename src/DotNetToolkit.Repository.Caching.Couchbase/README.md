# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
     .UseCouchbase(...)
     .Options;

var repo = new Repository<Customer>(options);
```
