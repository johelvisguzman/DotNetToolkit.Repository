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

    internal class MemcachedCacheProvider : ICacheProvider
    {
        #region Properties

        public IMemcachedClient Client { get; }
        public TimeSpan? Expiry { get; set; }

        #endregion

        #region Constructors

        public MemcachedCacheProvider(string host, string username, string password, MemcachedProtocol protocol, Type authType, TimeSpan? expiry)
            : this(GetClientConfiguration(host, username, password, protocol, authType), expiry) { }

        private MemcachedCacheProvider(IMemcachedClientConfiguration config, TimeSpan? expiry)
        {
            Client = new MemcachedClient(Guard.NotNull(config, nameof(config)));
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

        private static IMemcachedClientConfiguration GetClientConfiguration(string host, string username, string password, MemcachedProtocol protocol, Type authType = null)
        {
            var config = new MemcachedClientConfiguration();

            if (!string.IsNullOrEmpty(host))
            {
                config.AddServer(host);
            }

            config.Protocol = protocol;
            config.Authentication.Type = authType;
            
            if (!string.IsNullOrEmpty(username))
            {
                config.Authentication.Parameters["userName"] = username;
            }

            if (!string.IsNullOrEmpty(password))
            {
                config.Authentication.Parameters["password"] = password;
            }

            return config;
        }

        private void Set<T>(string key, T value, TimeSpan? expiry)
        {
            if (expiry.HasValue)
            {
                Client.Store(StoreMode.Set, key, Serialize(value), expiry.Value);
            }
            else
            {
                Client.Store(StoreMode.Set, key, Serialize(value));
            }
        }

        #endregion

        #region Implementation of ICacheProvider

        public void Set<T>(string key, T value, TimeSpan? expiry = null, Action<string> cacheRemovedCallback = null)
        {
            Guard.NotEmpty(key, nameof(key));

            Set(key, value, expiry ?? Expiry);
        }

        public void Remove([NotNull] string key)
        {
            Client.Remove(Guard.NotEmpty(key, nameof(key)));
        }

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

        public int Increment(string key, int defaultValue, int incrementValue)
        {
            Guard.NotEmpty(key, nameof(key));

            var value = Client.Increment(key, Convert.ToUInt64(defaultValue), Convert.ToUInt64(incrementValue));

            return Convert.ToInt32(value);
        }

        #endregion
    }
}
