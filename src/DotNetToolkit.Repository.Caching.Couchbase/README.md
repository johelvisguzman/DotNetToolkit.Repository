# How to Use?

```csharp
var options = new RepositoryOptionsBuilder()
     .UseCouchbase(...)
     .Options;

var repo = new Repository<Customer>(options);
```

# Configuration Options

|Action|Description|
|------|-----------|
|WithUserName|The user name|
|WithPassword|The password|
|WithBucketName|The bucket name|
|WithEndPoint|The host name or IP address of the server|
|WithExpiry|Expiration time|
|WithSerializer|The serializer|