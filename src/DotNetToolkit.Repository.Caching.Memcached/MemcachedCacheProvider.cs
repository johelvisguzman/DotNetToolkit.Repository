namespace DotNetToolkit.Repository.Caching.Memcached
{
    using Configuration.Caching;
    using Enyim.Caching;
    using Enyim.Caching.Memcached;
    using JetBrains.Annotations;
    using System;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="ICacheProvider{TCache}" />.
    /// </summary>
    public class MemcachedCacheProvider : CacheProviderBase<MemcachedCache>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class using the default configuration section (enyim/memcached). 
        /// </summary>
        public MemcachedCacheProvider() : this((TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class using the default configuration section (enyim/memcached). 
        /// </summary>
        /// <param name="expiry">The the caching expiration time.</param>
        public MemcachedCacheProvider([CanBeNull] TimeSpan? expiry)
        {
            Cache = new MemcachedCache();
            Expiry = expiry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class using the specified client.
        /// </summary>
        /// <param name="client">The memcached client.</param>
        public MemcachedCacheProvider([NotNull] IMemcachedClient client) : this(client, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class using the specified client.
        /// </summary>
        /// <param name="client">The memcached client.</param>
        /// <param name="expiry">The the caching expiration time.</param>
        public MemcachedCacheProvider([NotNull] IMemcachedClient client, [CanBeNull] TimeSpan? expiry)
        {
            Cache = new MemcachedCache(Guard.NotNull(client, nameof(client)));
            Expiry = expiry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCache" /> class using the specified configuration section.
        /// </summary>
        /// <param name="sectionName">The name of the configuration section to be used for configuring the behavior of the client.</param>
        public MemcachedCacheProvider([NotNull] string sectionName) : this(sectionName, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCache" /> class using the specified configuration section.
        /// </summary>
        /// <param name="sectionName">The name of the configuration section to be used for configuring the behavior of the client.</param>
        /// <param name="expiry">The the caching expiration time.</param>
        public MemcachedCacheProvider([NotNull] string sectionName, [CanBeNull] TimeSpan? expiry)
        {
            Cache = new MemcachedCache(Guard.NotEmpty(sectionName, nameof(sectionName)));
            Expiry = expiry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port.</param>
        public MemcachedCacheProvider([NotNull] string host, int port) : this(host, port, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port.</param>
        /// <param name="expiry">The the caching expiration time.</param>
        public MemcachedCacheProvider([NotNull] string host, int port, [CanBeNull] TimeSpan? expiry) : this(host, port, MemcachedProtocol.Binary, expiry) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port.</param>
        /// <param name="protocol">The type of the communication between client and server.</param>
        public MemcachedCacheProvider([NotNull] string host, int port, MemcachedProtocol protocol) : this(host, port, protocol, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port.</param>
        /// <param name="protocol">The type of the communication between client and server.</param>
        /// <param name="expiry">The the caching expiration time.</param>
        public MemcachedCacheProvider([NotNull] string host, int port, MemcachedProtocol protocol, [CanBeNull] TimeSpan? expiry)
        {
            Guard.NotEmpty(host, nameof(host));

            Cache = new MemcachedCache(host, port, protocol);
            Expiry = expiry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port.</param>
        /// <param name="username">The username to use to authenticate with the server.</param>
        /// <param name="password">The password to use to authenticate with the server.</param>
        public MemcachedCacheProvider([NotNull] string host, int port, [NotNull] string username, [NotNull] string password) : this(host, port, username, password, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port.</param>
        /// <param name="username">The username to use to authenticate with the server.</param>
        /// <param name="password">The password to use to authenticate with the server.</param>
        /// <param name="expiry">The the caching expiration time.</param>
        public MemcachedCacheProvider([NotNull] string host, int port, [NotNull] string username, [NotNull] string password, [CanBeNull] TimeSpan? expiry) : this(host, port, username, password, MemcachedProtocol.Binary, expiry) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port.</param>
        /// <param name="username">The username to use to authenticate with the server.</param>
        /// <param name="password">The password to use to authenticate with the server.</param>
        /// <param name="protocol">The type of the communication between client and server.</param>
        public MemcachedCacheProvider([NotNull] string host, int port, [NotNull] string username, [NotNull] string password, MemcachedProtocol protocol) : this(host, port, username, password, protocol, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port.</param>
        /// <param name="expiry">The the caching expiration time.</param>
        /// <param name="username">The username to use to authenticate with the server.</param>
        /// <param name="password">The password to use to authenticate with the server.</param>
        /// <param name="protocol">The type of the communication between client and server.</param>
        public MemcachedCacheProvider([NotNull] string host, int port, [NotNull] string username, [NotNull] string password, MemcachedProtocol protocol, [CanBeNull] TimeSpan? expiry)
        {
            Guard.NotEmpty(host, nameof(host));
            Guard.NotEmpty(username, nameof(username));
            Guard.NotEmpty(password, nameof(password));

            Cache = new MemcachedCache(host, port, username, password, protocol);
            Expiry = expiry;
        }

        #endregion
    }
}
