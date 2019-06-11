namespace DotNetToolkit.Repository.Integration.Test.CachingServers.Redis
{
    public class RedisServerProcessConfig : IServerProcessConfig
    {
        public string Args { get; }
        public string BasePath { get; }
        public string Exe { get; }

        public RedisServerProcessConfig()
        {
            BasePath = "CachingServers\\Redis\\tools\\redis-server";
            Exe = "redis-server.exe";
        }
    }
}
