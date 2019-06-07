namespace DotNetToolkit.Repository.Caching.Memcached
{
    using Configuration.Caching;
    using Enyim.Caching;
    using Enyim.Caching.Configuration;
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
        /// <param name="expiry">The caching expiration time.</param>
        public MemcachedCacheProvider([CanBeNull] TimeSpan? expiry) : this(new MemcachedClient(), expiry) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class using the specified client.
        /// </summary>
        /// <param name="client">The memcached client.</param>
        public MemcachedCacheProvider([NotNull] IMemcachedClient client) : this(client, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class using the specified configuration section.
        /// </summary>
        /// <param name="sectionName">The name of the configuration section to be used for configuring the behavior of the client.</param>
        public MemcachedCacheProvider([NotNull] string sectionName) : this(sectionName, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class using the specified configuration section.
        /// </summary>
        /// <param name="sectionName">The name of the configuration section to be used for configuring the behavior of the client.</param>
        /// <param name="expiry">The caching expiration time.</param>
        public MemcachedCacheProvider([NotNull] string sectionName, [CanBeNull] TimeSpan? expiry) : this(new MemcachedClient(Guard.NotEmpty(sectionName, nameof(sectionName))), expiry) { }

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
        /// <param name="expiry">The caching expiration time.</param>
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
        /// <param name="expiry">The caching expiration time.</param>
        public MemcachedCacheProvider([NotNull] string host, int port, MemcachedProtocol protocol, [CanBeNull] TimeSpan? expiry) : this(GetConfiguredClient(host, port, protocol), expiry) { }

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
        /// <param name="expiry">The caching expiration time.</param>
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
        /// <param name="expiry">The caching expiration time.</param>
        /// <param name="username">The username to use to authenticate with the server.</param>
        /// <param name="password">The password to use to authenticate with the server.</param>
        /// <param name="protocol">The type of the communication between client and server.</param>
        public MemcachedCacheProvider([NotNull] string host, int port, [NotNull] string username, [NotNull] string password, MemcachedProtocol protocol, [CanBeNull] TimeSpan? expiry) : this(GetConfiguredClient(host, port, username, password, protocol), expiry) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemcachedCacheProvider" /> class using the specified client.
        /// </summary>
        /// <param name="client">The memcached client.</param>
        /// <param name="expiry">The caching expiration time.</param>
        public MemcachedCacheProvider([NotNull] IMemcachedClient client, [CanBeNull] TimeSpan? expiry)
        {
            Cache = new MemcachedCache(Guard.NotNull(client, nameof(client)));
            Expiry = expiry;
        }

        #endregion

        #region Private Methods

        private static IMemcachedClient GetConfiguredClient(string host, int port, MemcachedProtocol protocol)
        {
            Guard.NotEmpty(host, nameof(host));

            var config = new MemcachedClientConfiguration();

            config.AddServer(host, port);
            config.Protocol = protocol;

            return new MemcachedClient(config);
        }

        private static IMemcachedClient GetConfiguredClient(string host, int port, string username, string password, MemcachedProtocol protocol)
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

            return new MemcachedClient(config);
        }

        #endregion
    }
}
