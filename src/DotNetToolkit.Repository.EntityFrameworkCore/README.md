# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
    .UseEntityFrameworkCore<AppDbContext>(...) // for entity framework core
    .Options;
    
var repo = new Repository<Customer>(options);
```
