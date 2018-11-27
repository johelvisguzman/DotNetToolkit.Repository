# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
    .UseAdoNet(...) // for pure ado.net (for testing purposes only)
    .Options;
    
var repo = new Repository<Customer>(options);
```
