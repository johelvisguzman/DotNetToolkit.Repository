namespace DotNetToolkit.Repository.Integration.Test.CachingServers.Redis
{
    using System;
    using System.IO;

    public class RedisServerProcessConfig : IServerProcessConfig
    {
        public string Args { get; }
        public string BasePath { get; }
        public string Exe { get; }
        public bool IsHidden { get; }

        public RedisServerProcessConfig()
        {
            Args = "";
            BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CachingServers\\Redis\\tools\\redis-server");
            Exe = "redis-server.exe";
            IsHidden = true;
        }
    }
}
