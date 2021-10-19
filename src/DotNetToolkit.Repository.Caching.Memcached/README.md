# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
     .UseMemcached(...)
     .Options;

var repo = new Repository<Customer>(options);
```

# Configuration Options

|Action|Description|
|------|-----------|
|WithUserName|The user name|
|WithPassword|The password|
|WithEndPoint|The host name or IP address of the server|
|WithProtocal|The protocal|
|WithAuthType|The authentication type|
|WithExpiry|Expiration time|
|WithJsonSerializerSettings|The json serializer settings|