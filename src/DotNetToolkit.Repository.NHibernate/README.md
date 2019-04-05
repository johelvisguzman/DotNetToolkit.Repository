# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
    .UseNHibernate(...) // for NHibernate
    .Options;
    
var repo = new Repository<Customer>(options);
```
