# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
    .UseJsonDatabase(...) // for a JSON formatted database
    .Options;
    
var repo = new Repository<Customer>(options);
```
