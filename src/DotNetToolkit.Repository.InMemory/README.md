# Disclaimer
This is intended for testing purposes only and should not be used in production code.

# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
    .UseInMemoryDatabase(...) // for an in-memory database (for testing purposes only)
    .Options;
    
var repo = new Repository<Customer>(options);
```
