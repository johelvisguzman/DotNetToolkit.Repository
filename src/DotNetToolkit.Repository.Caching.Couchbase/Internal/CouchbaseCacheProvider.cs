namespace DotNetToolkit.Repository.Caching.Couchbase.Internal
{
    using Configuration.Caching;
    using global::Couchbase;
    using global::Couchbase.Configuration.Client;
    using global::Couchbase.Core.Serialization;
    using System;
    using System.Collections.Generic;
    using Utility;

    internal class CouchbaseCacheProvider : ICacheProvider
    {
        #region Fields

        private const string DefaultBucketName = "default";

        #endregion

        #region Properties

        public Cluster Cluster { get; }
        public TimeSpan? Expiry { get; set; }

        #endregion

        #region Constructors

        public CouchbaseCacheProvider(string host, string bucketName, string username, string password, TimeSpan? expiry, Func<ITypeSerializer> serializer)
            : this(GetConfigurationOptions(host, bucketName, username, password, serializer), expiry) { }

        private CouchbaseCacheProvider(ClientConfiguration config, TimeSpan? expiry)
        {
            Cluster = new Cluster(Guard.NotNull(config, nameof(config)));
            Expiry = expiry;
        }

        #endregion

        #region Private Methods

        private static ClientConfiguration GetConfigurationOptions(string host, string username, string password, string bucketName, Func<ITypeSerializer> serializer)
        {
            var config = new ClientConfiguration();
            var bucket = !string.IsNullOrEmpty(bucketName) ? bucketName : DefaultBucketName;

            if (!string.IsNullOrEmpty(host))
            {
                config.Servers = new List<Uri>
                {
                    { new Uri(host) }
                };
            }

            config.BucketConfigs = new Dictionary<string, BucketConfiguration>
            {
                { bucket, new BucketConfiguration
                    {
                        Username = username,
                        Password = password,
                        BucketName = bucket
                    }
                }
            };

            if (serializer != null)
            {
                config.Serializer = serializer;
            }

            return config;
        }

        private void Set<T>(string key, T value, TimeSpan? expiry)
        {
            using (var bucket = Cluster.OpenBucket())
            {
                if (expiry.HasValue)
                {
                    bucket.Insert(key, value, expiry.Value);
                }
                else
                {
                    bucket.Insert(key, value);
                }
            }
        }

        #endregion

        #region Implementation of ICacheProvider

        public void Set<T>(string key, T value, TimeSpan? expiry, Action<string> cacheRemovedCallback = null)
        {
            Guard.NotEmpty(key, nameof(key));

            Set(key, value, expiry ?? Expiry);
        }

        public void Remove(string key)
        {
            using (var bucket = Cluster.OpenBucket())
            {
                bucket.Remove(Guard.NotEmpty(key, nameof(key)));
            }
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            Guard.NotEmpty(key, nameof(key));

            using (var bucket = Cluster.OpenBucket())
            {
                var operationResult = bucket.Get<T>(key);

                value = operationResult.Value;

                return operationResult.Success;
            }
        }

        public int Increment(string key, int defaultValue, int incrementValue)
        {
            Guard.NotEmpty(key, nameof(key));

            using (var bucket = Cluster.OpenBucket())
            {
                var operationResult = bucket.Increment(key, Convert.ToUInt64(defaultValue), Convert.ToUInt64(incrementValue));

                return Convert.ToInt32(operationResult.Value);
            }
        }

        #endregion
    }
}
