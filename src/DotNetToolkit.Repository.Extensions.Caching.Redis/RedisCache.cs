namespace DotNetToolkit.Repository.Extensions.Caching.Redis
{
    using Configuration.Caching;
    using Microsoft.Extensions.Caching.Memory;
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using System;

    /// <summary>
    /// An implementation of <see cref="ICache" />.
    /// </summary>
    public class RedisCache : ICache
    {
        #region Fields

        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;
        private IDatabase _redis;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the redis connection multiplexer.
        /// </summary>
        public ConnectionMultiplexer Connection { get { return _lazyConnection.Value; } }

        /// <summary>
        /// Gets the redis database.
        /// </summary>
        protected IDatabase Redis { get { return _redis ?? (_redis = Connection.GetDatabase()); } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCache" /> class.
        /// </summary>
        /// <param name="configuration">The string configuration to use for the redis multiplexer.</param>
        public RedisCache(string configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configuration));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCache" /> class.
        /// </summary>
        /// <param name="options">The configuration options to use for the redis multiplexer.</param>
        public RedisCache(ConfigurationOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCache" /> class. 
        /// </summary>
        /// <remarks>This will connect to a single server on the local machine using the default redis port (6379).</remarks>
        public RedisCache() : this(false, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCache" /> class. 
        /// </summary>
        /// <param name="allowAdmin">Indicates whether admin operations should be allowed.</param>
        /// <param name="defaultDatabase">Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters.</param>
        /// <remarks>This will connect to a single server on the local machine using the default redis port (6379).</remarks>
        public RedisCache(bool allowAdmin, int? defaultDatabase) : this("localhost", false, allowAdmin, defaultDatabase) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCache" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="ssl">Specifies that SSL encryption should be used.</param>
        /// <param name="allowAdmin">Indicates whether admin operations should be allowed.</param>
        /// <param name="defaultDatabase">Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters.</param>
        public RedisCache(string host, bool ssl, bool allowAdmin, int? defaultDatabase)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            var options = new ConfigurationOptions
            {
                EndPoints =
                {
                    { host }
                },
                Ssl = ssl,
                AllowAdmin = allowAdmin,
                DefaultDatabase = defaultDatabase
            };

            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCache" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port.</param>
        /// <param name="ssl">Specifies that SSL encryption should be used.</param>
        /// <param name="allowAdmin">Indicates whether admin operations should be allowed.</param>
        /// <param name="defaultDatabase">Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters.</param>
        public RedisCache(string host, int port, bool ssl, bool allowAdmin, int? defaultDatabase)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            var options = new ConfigurationOptions
            {
                EndPoints =
                {
                    { host, port }
                },
                Ssl = ssl,
                AllowAdmin = allowAdmin,
                DefaultDatabase = defaultDatabase
            };

            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCache" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port.</param>
        /// <param name="password">The password to use to authenticate with the server.</param>
        /// <param name="ssl">Specifies that SSL encryption should be used.</param>
        /// <param name="allowAdmin">Indicates whether admin operations should be allowed.</param>
        /// <param name="defaultDatabase">Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters.</param>
        public RedisCache(string host, int port, string password, bool ssl, bool allowAdmin, int? defaultDatabase)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            if (password == null)
                throw new ArgumentNullException(nameof(password));

            var options = new ConfigurationOptions
            {
                EndPoints =
                {
                    { host, port }
                },
                Password = password,
                Ssl = ssl,
                AllowAdmin = allowAdmin,
                DefaultDatabase = defaultDatabase
            };

            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
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

        #endregion

        #region Implementation of ICache

        /// <summary>
        /// Create or overwrite an entry in the cache.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="cacheExpiration">The cache expiration time.</param>
        /// <param name="cacheRemovedCallback">A callback function for a value is removed from the cache.</param>
        public void Set<T>(string key, T value, CacheItemPriority priority, TimeSpan? cacheExpiration, Action<string> cacheRemovedCallback = null)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

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

            Redis.StringSet(key, Serialize(value), cacheExpiration);
        }

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        public void Remove(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            Redis.KeyDelete(key);
        }

        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The object found in the cache.</param>
        /// <returns>c<c>true</c> if the an object was found with the specified key; otherwise, <c>false</c>.</returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            value = Deserialize<T>(Redis.StringGet(key));

            return value != null;
        }

        /// <summary>
        /// Increments the number stored at key by one
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="incrementValue">The increment value.</param>
        /// <param name="priority">The priority.</param>
        /// <returns>The value of key after the increment.</returns>
        public int Increment(string key, int defaultValue, int incrementValue, CacheItemPriority priority = CacheItemPriority.Normal)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var value = Redis.StringIncrement(key, incrementValue);

            return Convert.ToInt32(value);
        }

        #endregion
    }
}
