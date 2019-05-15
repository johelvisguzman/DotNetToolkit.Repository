﻿namespace DotNetToolkit.Repository.Caching.Redis
{
    using Configuration.Caching;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using System;
    using Utility;

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
        /// Gets the redis server.
        /// </summary>
        public IServer Server
        {
            get { return Connection.GetServer(Connection.GetEndPoints()[0]); }
        }

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
        public RedisCache([NotNull] string configuration)
        {
            Guard.NotEmpty(configuration, nameof(configuration));

            _lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configuration));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCache" /> class.
        /// </summary>
        /// <param name="options">The configuration options to use for the redis multiplexer.</param>
        public RedisCache(ConfigurationOptions options)
        {
            Guard.NotNull(options, nameof(options));

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
        public RedisCache(bool allowAdmin, [CanBeNull] int? defaultDatabase) : this("localhost", false, allowAdmin, defaultDatabase) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCache" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="ssl">Specifies that SSL encryption should be used.</param>
        /// <param name="allowAdmin">Indicates whether admin operations should be allowed.</param>
        /// <param name="defaultDatabase">Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters.</param>
        public RedisCache([NotNull] string host, bool ssl, bool allowAdmin, [CanBeNull] int? defaultDatabase)
        {
            Guard.NotEmpty(host, nameof(host));

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
        public RedisCache([NotNull] string host, int port, bool ssl, bool allowAdmin, [CanBeNull] int? defaultDatabase)
        {
            Guard.NotEmpty(host, nameof(host));

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
        public RedisCache([NotNull] string host, int port, [NotNull] string password, bool ssl, bool allowAdmin, [CanBeNull] int? defaultDatabase)
        {
            Guard.NotEmpty(host, nameof(host));
            Guard.NotEmpty(password, nameof(password));

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
        /// <param name="expiry">The cache expiration time.</param>
        /// <param name="cacheRemovedCallback">A callback function for a value is removed from the cache.</param>
        public void Set<T>([NotNull] string key, [CanBeNull] T value, [CanBeNull] TimeSpan? expiry, [CanBeNull] Action<string> cacheRemovedCallback = null)
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

            Redis.StringSet(key, Serialize(value), expiry);
        }

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        public void Remove([NotNull] string key)
        {
            Redis.KeyDelete(Guard.NotEmpty(key, nameof(key)));
        }

        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The object found in the cache.</param>
        /// <returns>c<c>true</c> if the an object was found with the specified key; otherwise, <c>false</c>.</returns>
        public bool TryGetValue<T>([NotNull] string key, out T value)
        {
            value = Deserialize<T>(Redis.StringGet(Guard.NotEmpty(key, nameof(key))));

            return value != null;
        }

        /// <summary>
        /// Increments the number stored at key by one
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="incrementValue">The increment value.</param>
        /// <returns>The value of key after the increment.</returns>
        public int Increment([NotNull] string key, int defaultValue, int incrementValue)
        {
            var value = Redis.StringIncrement(Guard.NotEmpty(key, nameof(key)), incrementValue);

            return Convert.ToInt32(value);
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_lazyConnection.IsValueCreated)
            {
                var conn = _lazyConnection.Value;

                conn.Close(false);
                conn.Dispose();
            }
        }

        #endregion
    }
}