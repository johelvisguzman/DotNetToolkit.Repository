#if NETFULL
namespace DotNetToolkit.Repository.Integration.Test.Helpers
{
    using Enyim.Caching;
    using Enyim.Caching.Configuration;

    public static class MemcachedHelper
    {
        public static void ClearDatabase(string host, int port)
        {
            var config = new MemcachedClientConfiguration();

            config.AddServer(host, port);

            using (var client = new MemcachedClient(config))
            {
                client.FlushAll();
            }
        }
    }
}
#endif