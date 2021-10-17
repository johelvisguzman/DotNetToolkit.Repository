namespace DotNetToolkit.Repository.Integration.Test.Helpers
{
    using StackExchange.Redis;

    public static class RedisHelper
    {
        public static void ClearDatabase(string host, int defaultDatabase)
        {
            var options = new ConfigurationOptions()
            {
                AllowAdmin = true,
                DefaultDatabase = defaultDatabase,
                EndPoints =
                {
                    { host }
                }
            };

            using (var connection = ConnectionMultiplexer.Connect(options))
            {
                var server = connection.GetServer(connection.GetEndPoints()[0]);
                server.FlushAllDatabases();
            }
        }
    }
}
