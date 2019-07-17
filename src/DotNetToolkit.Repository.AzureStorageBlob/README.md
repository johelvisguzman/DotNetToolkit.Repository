# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
    .UseAzureStorageBlob(...) // for microsoft azure storage blob service
    .Options;
    
var repo = new Repository<Customer>(options);
```
