# Disclaimer
This is intended for testing purposes only and should not be used in production code.

# Issues
There is currently an issue with join nested properties that has not yet been resolved, and because of this, it is not recommended to use this context in production code. For more info, see related issue [#556](https://github.com/johelvisguzman/DotNetToolkit.Repository/issues/565)

# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
    .UseAdoNet(...) // for pure ado.net (for testing purposes only)
    .Options;
    
var repo = new Repository<Customer>(options);
```
