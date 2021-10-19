# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
     .UseRedis(...)
     .Options;

var repo = new Repository<Customer>(options);
```

# Configuration Options

|Action|Description|
|------|-----------|
|WithUserName|The user name|
|WithPassword|The password|
|WithSsl|Specifies that SSL encryption should be used|
|WithEndPoint|The host name or IP address of the server|
|WithAllowAdmin|Indicates whether admin operations should be allowed|
|WithDefaultDatabase|Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters|
|WithExpiry|Expiration time|
|WithJsonSerializerSettings|The json serializer settings|