# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
     .UseInMemoryCache(...) // for microsoft in-memory cache
     .Options;

var repo = new Repository<Customer>(options);
```

# Configuration Options

|Action|Description|
|------|-----------|
|WithClock|System clock|
|WithExpirationScanFrequency|Minimum length of time between successive scans for expired items|
|WithExpiry|Expiration time|