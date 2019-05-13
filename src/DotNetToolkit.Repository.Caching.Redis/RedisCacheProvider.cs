namespace DotNetToolkit.Repository.Caching.Redis
{
    using Configuration.Caching;
    using JetBrains.Annotations;
    using StackExchange.Redis;
    using System;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="ICacheProvider{TCache}" />.
    /// </summary>
    public class RedisCacheProvider : ICacheProvider<RedisCache>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class. 
        /// </summary>
        /// <remarks>This will connect to a single server on the local machine using the default redis port (6379).</remarks>
        public RedisCacheProvider() : this((TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class. 
        /// </summary>
        /// <param name="expiry">The the caching expiration time.</param>
        /// <remarks>This will connect to a single server on the local machine using the default redis port (6379).</remarks>
        public RedisCacheProvider([CanBeNull] TimeSpan? expiry)
        {
            Cache = new RedisCache();
            Expiry = expiry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class. 
        /// </summary>
        /// <param name="allowAdmin">Indicates whether admin operations should be allowed.</param>
        /// <param name="defaultDatabase">Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters.</param>
        /// <remarks>This will connect to a single server on the local machine using the default redis port (6379).</remarks>
        public RedisCacheProvider(bool allowAdmin, [CanBeNull] int? defaultDatabase) : this(allowAdmin, defaultDatabase, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class. 
        /// </summary>
        /// <param name="allowAdmin">Indicates whether admin operations should be allowed.</param>
        /// <param name="defaultDatabase">Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters.</param>
        /// <param name="expiry">The the caching expiration time.</param>
        /// <remarks>This will connect to a single server on the local machine using the default redis port (6379).</remarks>
        public RedisCacheProvider(bool allowAdmin, [CanBeNull] int? defaultDatabase, [CanBeNull] TimeSpan? expiry)
        {
            Cache = new RedisCache(allowAdmin, defaultDatabase);
            Expiry = expiry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class.
        /// </summary>
        /// <param name="configuration">The string configuration to use for the redis multiplexer.</param>
        public RedisCacheProvider([NotNull] string configuration) : this(configuration, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class.
        /// </summary>
        /// <param name="configuration">The string configuration to use for the redis multiplexer.</param>
        /// <param name="expiry">The the caching expiration time.</param>
        public RedisCacheProvider([NotNull] string configuration, [CanBeNull] TimeSpan? expiry)
        {
            Cache = new RedisCache(Guard.NotNull(configuration, nameof(configuration)));
            Expiry = expiry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class.
        /// </summary>
        /// <param name="options">The configuration options to use for the redis multiplexer.</param>
        public RedisCacheProvider([NotNull] ConfigurationOptions options) : this(options, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class.
        /// </summary>
        /// <param name="options">The configuration options to use for the redis multiplexer.</param>
        /// <param name="expiry">The the caching expiration time.</param>
        public RedisCacheProvider([NotNull] ConfigurationOptions options, [CanBeNull] TimeSpan? expiry)
        {
            Cache = new RedisCache(Guard.NotNull(options, nameof(options)));
            Expiry = expiry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="ssl">Specifies that SSL encryption should be used.</param>
        /// <param name="allowAdmin">Indicates whether admin operations should be allowed.</param>
        /// <param name="defaultDatabase">Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters.</param>
        public RedisCacheProvider([NotNull] string host, bool ssl, bool allowAdmin, [CanBeNull] int? defaultDatabase) : this(host, ssl, allowAdmin, defaultDatabase, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="ssl">Specifies that SSL encryption should be used.</param>
        /// <param name="allowAdmin">Indicates whether admin operations should be allowed.</param>
        /// <param name="defaultDatabase">Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters.</param>
        /// <param name="expiry">The the caching expiration time.</param>
        public RedisCacheProvider([NotNull] string host, bool ssl, bool allowAdmin, [CanBeNull] int? defaultDatabase, [CanBeNull] TimeSpan? expiry)
        {
            Guard.NotEmpty(host, nameof(host));

            Cache = new RedisCache(host, ssl, allowAdmin, defaultDatabase);
            Expiry = expiry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port.</param>
        /// <param name="password">The password to use to authenticate with the server.</param>
        /// <param name="ssl">Specifies that SSL encryption should be used.</param>
        /// <param name="allowAdmin">Indicates whether admin operations should be allowed.</param>
        /// <param name="defaultDatabase">Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters.</param>
        public RedisCacheProvider([NotNull] string host, int port, [NotNull] string password, bool ssl, bool allowAdmin, [CanBeNull] int? defaultDatabase) : this(host, port, password, ssl, allowAdmin, defaultDatabase, (TimeSpan?)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="port">The port.</param>
        /// <param name="password">The password to use to authenticate with the server.</param>
        /// <param name="ssl">Specifies that SSL encryption should be used.</param>
        /// <param name="allowAdmin">Indicates whether admin operations should be allowed.</param>
        /// <param name="defaultDatabase">Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters.</param>
        /// <param name="expiry">The the caching expiration time.</param>
        public RedisCacheProvider([NotNull] string host, int port, [NotNull] string password, bool ssl, bool allowAdmin, [CanBeNull] int? defaultDatabase, [CanBeNull] TimeSpan? expiry)
        {
            Guard.NotEmpty(host, nameof(host));
            Guard.NotEmpty(password, nameof(password));

            Cache = new RedisCache(host, port, password, ssl, allowAdmin, defaultDatabase);
            Expiry = expiry;
        }

        #endregion

        #region Implementation of ICacheProvider

        /// <summary>
        /// Gets or sets the caching expiration time.
        /// </summary>
        public TimeSpan? Expiry { get; set; }

        /// <summary>
        /// Gets the cache.
        /// </summary>
        ICache ICacheProvider.Cache
        {
            get { return Cache; }
        }

        #endregion

        #region Implementation of ICacheProvider<TCache>

        /// <summary>
        /// Gets the cache.
        /// </summary>
        public RedisCache Cache { get; }

        #endregion
    }
}