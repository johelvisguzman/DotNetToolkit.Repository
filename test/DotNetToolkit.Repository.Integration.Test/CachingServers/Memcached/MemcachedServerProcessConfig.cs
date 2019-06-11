namespace DotNetToolkit.Repository.Integration.Test.CachingServers.Memcached
{
    public class MemcachedServerProcessConfig : IServerProcessConfig
    {
        public string Args { get; }
        public string BasePath { get; }
        public string Exe { get; }

        public MemcachedServerProcessConfig()
        {
            Args = $"-E default_engine.so -p 11211 -m 512";
            BasePath = "CachingServers\\Memcached\\tools\\memcached-server";
            Exe = "memcached.exe";
        }
    }
}
