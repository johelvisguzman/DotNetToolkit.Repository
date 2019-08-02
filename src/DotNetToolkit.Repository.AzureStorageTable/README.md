# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
    .UseAzureStorageTable(...) // for microsoft azure storage table service
    .Options;
    
var repo = new Repository<Customer>(options);
```
