namespace DotNetToolkit.Repository.Caching.Memcached
{
    using Configuration.Caching;
    using Enyim.Caching;
    using Enyim.Caching.Configuration;
    using Enyim.Caching.Memcached;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using System;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="ICache" />.
    /// </summary>
    public class MemcachedCache : ICache
    {
        #region Properties

        /// <summary>
        /// Gets the memcached client.
        /// </summary>
        public IMemcachedClient Client { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCache" /> class using the default configuration section (enyim/memcached).
        /// </summary>
        public MemcachedCache()
        {
            Client = new MemcachedClient();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCache" /> class using the specified configuration section.
        /// </summary>
        /// <param name="sectionName">The name of the configuration section to be used for configuring the behavior of the client.</param>
        public MemcachedCache([NotNull] string sectionName)
        {
            Client = new MemcachedClient(Guard.NotEmpty(sectionName, nameof(sectionName)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCache" /> class using the specified client.
        /// </summary>
        /// <param name="client">The memcached client.</param>
        public MemcachedCache([NotNull] IMemcachedClient client)
        {
            Client = Guard.NotNull(client, nameof(client));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCache" /> class.
        /// </summary>
        /// <param name="configuration">The configuration to use for the memcached client.</param>
        public MemcachedCache([NotNull] IMemcachedClientConfiguration configuration)
        {
            Client = new MemcachedClient(Guard.NotNull(configuration, nameof(configuration)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCache" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port.</param>
        /// <param name="protocol">The type of the communication between client and server.</param>
        public MemcachedCache([NotNull] string host, int port, MemcachedProtocol protocol)
        {
            Guard.NotEmpty(host, nameof(host));

            var config = new MemcachedClientConfiguration();

            config.AddServer(host, port);
            config.Protocol = protocol;

            Client = new MemcachedClient(config);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCache" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port.</param>
        /// <param name="username">The username to use to authenticate with the server.</param>
        /// <param name="password">The password to use to authenticate with the server.</param>
        /// <param name="protocol">The type of the communication between client and server.</param>
        public MemcachedCache([NotNull] string host, int port, [NotNull] string username, [NotNull] string password, MemcachedProtocol protocol)
        {
            Guard.NotEmpty(host, nameof(host));
            Guard.NotEmpty(username, nameof(username));
            Guard.NotEmpty(password, nameof(password));

            var config = new MemcachedClientConfiguration();

            config.AddServer(host, port);
            config.Protocol = protocol;
            config.Authentication.Type = typeof(PlainTextAuthenticator);
            config.Authentication.Parameters["userName"] = username;
            config.Authentication.Parameters["password"] = password;

            Client = new MemcachedClient(config);
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
        /// <typeparam name="T">The type of the cache value.</typeparam>
        /// <param name="key">An object identifying the entry.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="expiry">The cache expiration time.</param>
        /// <param name="cacheRemovedCallback">A callback function for a value is removed from the cache.</param>
        public void Set<T>([NotNull] string key, [CanBeNull] T value, [CanBeNull] TimeSpan? expiry, [CanBeNull] Action<string> cacheRemovedCallback = null)
        {
            Guard.NotEmpty(key, nameof(key));

            if (expiry.HasValue)
            {
                Client.Store(StoreMode.Set, key, Serialize(value), expiry.Value);
            }
            else
            {
                Client.Store(StoreMode.Set, key, Serialize(value));
            }
        }

        /// <summary>
        /// Removes the object associated with the given key.
        /// </summary>
        /// <param name="key">An object identifying the entry.</param>
        public void Remove([NotNull] string key)
        {
            Client.Remove(Guard.NotEmpty(key, nameof(key)));
        }

        /// <summary>
        /// Gets the item associated with this key if present.
        /// </summary>
        /// <typeparam name="T">The type of the cache value.</typeparam>
        /// <param name="key">An object identifying the requested entry.</param>
        /// <param name="value">The object found in the cache.</param>
        /// <returns>c<c>true</c> if the an object was found with the specified key; otherwise, <c>false</c>.</returns>
        public bool TryGetValue<T>(string key, out T value)
        {
            try
            {
                value = Deserialize<T>(Client.Get<string>(Guard.NotEmpty(key, nameof(key))));

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

        /// <summary>
        /// Increments the number stored at key by one
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="incrementValue">The increment value.</param>
        /// <returns>The value of key after the increment.</returns>
        public int Increment(string key, int defaultValue, int incrementValue)
        {
            Guard.NotEmpty(key, nameof(key));

            var value = Client.Increment(key, Convert.ToUInt64(defaultValue), Convert.ToUInt64(incrementValue));

            return Convert.ToInt32(value);
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Client.Dispose();
        }

        #endregion
    }
}
