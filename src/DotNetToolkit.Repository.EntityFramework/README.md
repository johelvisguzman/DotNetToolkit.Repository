# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
    .UseEntityFramework<AppDbContext>(...) // for entity framework 6
    .Options;
    
var repo = new Repository<Customer>(options);
```
