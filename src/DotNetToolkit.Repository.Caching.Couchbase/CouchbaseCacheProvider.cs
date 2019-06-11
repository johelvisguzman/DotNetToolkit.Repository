namespace DotNetToolkit.Repository.Caching.Couchbase
{
    using Configuration.Caching;
    using global::Couchbase.Configuration.Client;
    using JetBrains.Annotations;
    using System;
    using System.Collections.Generic;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="ICacheProvider{TCache}" />.
    /// </summary>
    public class CouchbaseCacheProvider : CacheProviderBase<CouchbaseCache>
    {
        #region Constructors

#if NETFULL
        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseCacheProvider" /> class. 
        /// </summary>
        /// <param name="sectionName">The name of the configuration section to be used for configuring the behavior of the couchbase client.</param>
        public CouchbaseCacheProvider([NotNull] string sectionName) : this(sectionName, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseCacheProvider" /> class. 
        /// </summary>
        /// <param name="sectionName">The name of the configuration section to be used for configuring the behavior of the couchbase client.</param>
        /// <param name="expiry">The caching expiration time.</param>
        public CouchbaseCacheProvider([NotNull] string sectionName, [CanBeNull] TimeSpan? expiry)
        {
            Cache = new CouchbaseCache(Guard.NotEmpty(sectionName, nameof(sectionName)));
            Expiry = expiry;
        } 
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseCacheProvider" /> class. 
        /// </summary>
        /// <remarks>This will connect to a single server on the local machine.</remarks>
        public CouchbaseCacheProvider() : this((TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseCacheProvider" /> class. 
        /// </summary>
        /// <param name="expiry">The caching expiration time.</param>
        /// <remarks>This will connect to a single server on the local machine.</remarks>
        public CouchbaseCacheProvider([CanBeNull] TimeSpan? expiry) : this(new ClientConfiguration(), expiry) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="username">The username to use to authenticate with the server.</param>
        /// <param name="password">The password to use to authenticate with the server.</param>
        public CouchbaseCacheProvider([NotNull] string host, [NotNull] string username, [NotNull] string password) : this(host, username, password, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="username">The username to use to authenticate with the server.</param>
        /// <param name="password">The password to use to authenticate with the server.</param>
        /// <param name="expiry">The caching expiration time.</param>
        public CouchbaseCacheProvider([NotNull] string host, [NotNull] string username, [NotNull] string password, [CanBeNull] TimeSpan? expiry) : this(host, null, username, password, expiry) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="bucketName">The bucket name.</param>
        /// <param name="username">The username to use to authenticate with the server.</param>
        /// <param name="password">The password to use to authenticate with the server.</param>
        public CouchbaseCacheProvider([NotNull] string host, [NotNull] string bucketName, [NotNull] string username, [NotNull] string password) : this(host, bucketName, username, password, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="bucketName">The bucket name.</param>
        /// <param name="username">The username to use to authenticate with the server.</param>
        /// <param name="password">The password to use to authenticate with the server.</param>
        /// <param name="expiry">The caching expiration time.</param>
        public CouchbaseCacheProvider([NotNull] string host, string bucketName, [NotNull] string username, [NotNull] string password, [CanBeNull] TimeSpan? expiry) : this(GetConfigurationOptions(host, username, password, bucketName), expiry) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseCacheProvider" /> class.
        /// </summary>
        /// <param name="configAction">The configuration options action.</param>
        public CouchbaseCacheProvider([NotNull] Action<ClientConfiguration> configAction) : this(configAction, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseCacheProvider" /> class.
        /// </summary>
        /// <param name="configAction">The configuration options action.</param>
        /// <param name="expiry">The caching expiration time.</param>
        public CouchbaseCacheProvider([NotNull] Action<ClientConfiguration> configAction, [CanBeNull] TimeSpan? expiry) : this(GetConfigurationOptions(configAction), expiry) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseCacheProvider" /> class.
        /// </summary>
        /// <param name="config">The configuration options to use for the couchbase client.</param>
        public CouchbaseCacheProvider([NotNull] ClientConfiguration config) : this(config, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CouchbaseCacheProvider" /> class.
        /// </summary>
        /// <param name="config">The configuration options to use for the couchbase client.</param>
        /// <param name="expiry">The caching expiration time.</param>
        public CouchbaseCacheProvider([NotNull] ClientConfiguration config, [CanBeNull] TimeSpan? expiry)
        {
            Cache = new CouchbaseCache(Guard.NotNull(config, nameof(config)));
            Expiry = expiry;
        }

        #endregion

        #region Private Methods

        private static ClientConfiguration GetConfigurationOptions(Action<ClientConfiguration> configAction)
        {
            Guard.NotNull(configAction, nameof(configAction));

            var config = new ClientConfiguration();

            configAction(config);

            return config;
        }

        private static ClientConfiguration GetConfigurationOptions(string host, string username, string password, string bucketName)
        {
            Guard.NotEmpty(host, nameof(host));
            Guard.NotEmpty(username, nameof(username));
            Guard.NotEmpty(password, nameof(password));

            if (string.IsNullOrEmpty(bucketName))
                bucketName = "default";

            return new ClientConfiguration
            {
                Servers = new List<Uri> { new Uri(host) },
                BucketConfigs = new Dictionary<string, BucketConfiguration>
                {
                    { bucketName, new BucketConfiguration
                        {
                            Password = password,
                            Username = username,
                            BucketName = bucketName
                        }
                    }
                }
            };
        }

        #endregion
    }
}
