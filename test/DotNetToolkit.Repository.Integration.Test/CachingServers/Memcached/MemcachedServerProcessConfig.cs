namespace DotNetToolkit.Repository.Integration.Test.CachingServers.Memcached
{
    using System;
    using System.IO;

    public class MemcachedServerProcessConfig : IServerProcessConfig
    {
        public string Args { get; }
        public string BasePath { get; }
        public string Exe { get; }
        public bool IsHidden { get; }

        public MemcachedServerProcessConfig()
        {
            Args = $"-E default_engine.so -p 11211 -m 512";
            BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CachingServers\\Memcached\\tools\\memcached-server");
            Exe = "memcached.exe";
            IsHidden = true;
        }
    }
}
