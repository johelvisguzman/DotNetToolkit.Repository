namespace DotNetToolkit.Repository.Caching.Redis
{
    using Configuration.Caching;
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using System;
    using Utility;

    internal class RedisCacheProvider : ICacheProvider
    {
        #region Fields

        private IDatabase _redis;
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;

        #endregion

        #region Properties

        public IDatabase Redis { get { return _redis ?? (_redis = Connection.GetDatabase()); } }
        public TimeSpan? Expiry { get; set; }
        public ConnectionMultiplexer Connection { get { return _lazyConnection.Value; } }

        #endregion

        #region Constructors

        public RedisCacheProvider()
            : this("localhost", null, null, false, false, null, null) { }

        public RedisCacheProvider(string host, string username, string password, bool ssl, bool allowAdmin, int? defaultDatabase, TimeSpan? expiry)
            : this(GetConfigurationOptions(host, username, password, ssl, allowAdmin, defaultDatabase), expiry) { }

        private RedisCacheProvider(ConfigurationOptions options, TimeSpan? expiry)
        {
            Guard.NotNull(options, nameof(options));

            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));

            Expiry = expiry;
        }

        #endregion

        #region Private Methods

        private static string Serialize(object o)
        {
            if (o == null)
                return null;

            return JsonConvert.SerializeObject(o);
        }

        private static T Deserialize<T>(string v)
        {
            if (string.IsNullOrEmpty(v))
                return default(T);

            return JsonConvert.DeserializeObject<T>(v);
        }

        private static ConfigurationOptions GetConfigurationOptions(string host, string username, string password, bool ssl, bool allowAdmin, int? defaultDatabase)
        {
            var options = new ConfigurationOptions
            {
                User = username,
                Password = password,
                Ssl = ssl,
                AllowAdmin = allowAdmin,
                DefaultDatabase = defaultDatabase
            };

            if (!string.IsNullOrEmpty(host))
            {
                options.EndPoints.Add(host);
            }

            return options;
        }

        #endregion

        #region Implementation of ICacheProvider

        public void Set<T>(string key, T value, TimeSpan? expiry = null, Action<string> cacheRemovedCallback = null)
        {
            Guard.NotEmpty(key, nameof(key));

            if (cacheRemovedCallback != null)
            {
                var subscriber = Connection.GetSubscriber();
                var notificationChannel = "__keyspace@" + Redis.Database + "__:" + key;

                subscriber.Subscribe(notificationChannel, (channel, notificationType) =>
                {
                    switch (notificationType) // use "Kxe" keyspace notification options to enable all of the below...
                    {
                        case "expired": // requires the "Kx" keyspace notification options to be enabled
                        case "evicted": // requires the "Ke" keyspace notification option to be enabled
                            cacheRemovedCallback(notificationType.ToString());
                            break;
                    }
                });
            }

            Redis.StringSet(key, Serialize(value), expiry ?? Expiry);
        }

        public void Remove(string key)
        {
            Redis.KeyDelete(Guard.NotEmpty(key, nameof(key)));
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            try
            {
                value = Deserialize<T>(Redis.StringGet(Guard.NotEmpty(key, nameof(key))));

                if (Equals(value, default(T)))
                {
                    value = default(T);

                    return false;
                }
            }
            catch
            {
                value = default(T);

                return false;
            }

            return true;
        }

        public int Increment(string key, int defaultValue, int incrementValue)
        {
            var value = Redis.StringIncrement(Guard.NotEmpty(key, nameof(key)), incrementValue);

            return Convert.ToInt32(value);
        }

        #endregion
    }
}