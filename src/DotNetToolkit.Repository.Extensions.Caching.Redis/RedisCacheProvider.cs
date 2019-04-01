namespace DotNetToolkit.Repository.Extensions.Caching.Redis
{
    using Configuration.Caching;
    using StackExchange.Redis;
    using System;

    /// <summary>
    /// An implementation of <see cref="ICacheProvider" />.
    /// </summary>
    public class RedisCacheProvider : ICacheProvider
    {
        #region Fields

        private readonly RedisCache _redis;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the redis connection multiplexer.
        /// </summary>
        public ConnectionMultiplexer Connection
        {
            get { return _redis.Connection; }
        }

        /// <summary>
        /// Gets the redis server.
        /// </summary>
        public IServer Server
        {
            get { return Connection.GetServer(Connection.GetEndPoints()[0]); }
        }

        #endregion

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
        public RedisCacheProvider(TimeSpan? expiry)
        {
            _redis = new RedisCache();

            Expiry = expiry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class. 
        /// </summary>
        /// <param name="allowAdmin">Indicates whether admin operations should be allowed.</param>
        /// <param name="defaultDatabase">Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters.</param>
        /// <param name="expiry">The the caching expiration time.</param>
        /// <remarks>This will connect to a single server on the local machine using the default redis port (6379).</remarks>
        public RedisCacheProvider(bool allowAdmin, int? defaultDatabase, TimeSpan? expiry)
        {
            _redis = new RedisCache(allowAdmin, defaultDatabase);

            Expiry = expiry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class.
        /// </summary>
        /// <param name="configuration">The string configuration to use for the redis multiplexer.</param>
        /// <param name="expiry">The the caching expiration time.</param>
        public RedisCacheProvider(string configuration, TimeSpan? expiry)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _redis = new RedisCache(configuration);

            Expiry = expiry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class.
        /// </summary>
        /// <param name="options">The configuration options to use for the redis multiplexer.</param>
        /// <param name="expiry">The the caching expiration time.</param>
        public RedisCacheProvider(ConfigurationOptions options, TimeSpan? expiry)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _redis = new RedisCache(options);

            Expiry = expiry;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedisCacheProvider" /> class.
        /// </summary>
        /// <param name="host">The host name.</param>
        /// <param name="ssl">Specifies that SSL encryption should be used.</param>
        /// <param name="allowAdmin">Indicates whether admin operations should be allowed.</param>
        /// <param name="defaultDatabase">Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters.</param>
        /// <param name="expiry">The the caching expiration time.</param>
        public RedisCacheProvider(string host, bool ssl, bool allowAdmin, int? defaultDatabase, TimeSpan? expiry)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            _redis = new RedisCache(host, ssl, allowAdmin, defaultDatabase);

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
        /// <param name="expiry">The the caching expiration time.</param>
        public RedisCacheProvider(string host, int port, string password, bool ssl, bool allowAdmin, int? defaultDatabase, TimeSpan? expiry)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            if (password == null)
                throw new ArgumentNullException(nameof(password));

            _redis = new RedisCache(host, port, password, ssl, allowAdmin, defaultDatabase);

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
        public ICache Cache { get { return _redis; } }

        #endregion
    }
}