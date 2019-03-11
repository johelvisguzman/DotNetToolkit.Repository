# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
    .UseXmlDatabase(...) // for a XML formatted database
    .Options;
    
var repo = new Repository<Customer>(options);
```
