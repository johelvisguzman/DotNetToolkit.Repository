namespace DotNetToolkit.Repository.Caching.Redis
{
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using System;
    using Utility;

    /// <summary>
    /// The options to be used by the redis caching provider.
    /// </summary>
    public class RedisCacheOptions
    {
        private string _host;
        private string _username;
        private string _password;
        private bool _ssl;
        private bool _allowAdmin;
        private int? _defaultDatabase;
        private TimeSpan? _expiry;
        private JsonSerializerSettings _serializerSettings;

        /// <summary>
        /// Gets the host.
        /// </summary>
        public string Host { get { return _host; } }

        /// <summary>
        /// Gets the username.
        /// </summary>
        public string Username { get { return _username; } }

        /// <summary>
        /// Gets the password.
        /// </summary>
        public string Password { get { return _password; } }

        /// <summary>
        /// Gets a value indicating that ssl should be used.
        /// </summary>
        public bool Ssl { get { return _ssl; } }

        /// <summary>
        /// Gets a value indicating that admin operations should be allowed.
        /// </summary>
        public bool AllowAdmin { get { return _allowAdmin; } }

        /// <summary>
        /// Gets the default database.
        /// </summary>
        public int? DefaultDatabase { get { return _defaultDatabase; } }

        /// <summary>
        /// Gets the expiration time.
        /// </summary>
        public TimeSpan? Expiry { get { return _expiry; } }

        /// <summary>
        /// Gets the json serializer settings.
        /// </summary>
        public JsonSerializerSettings SerializerSettings {  get { return _serializerSettings; } }

        /// <summary>
        /// Adds the giving username to the options.
        /// </summary>
        /// <param name="username">The user name to be added.</param>
        public RedisCacheOptions WithUserName([NotNull] string username)
        {
            _username = Guard.NotEmpty(username, nameof(username));

            return this;
        }

        /// <summary>
        /// Adds the giving password to the options.
        /// </summary>
        /// <param name="password">The password to be added.</param>
        public RedisCacheOptions WithPassword([NotNull] string password)
        {
            _password = Guard.NotEmpty(password, nameof(password));

            return this;
        }

        /// <summary>
        /// Adds the giving endpoint to the options.
        /// </summary>
        /// <param name="host">The host name or IP address of the server to be added.</param>
        /// <param name="port">The port to be added.</param>
        public RedisCacheOptions WithEndPoint([NotNull] string host, int port)
        {
            Guard.NotEmpty(host, nameof(host));

            _host = string.Format("{0}:{1}", host, port);

            return this;
        }

        /// <summary>
        /// Adds the giving endpoint to the options.
        /// </summary>
        /// <param name="hostAndPort">The address and the port of the server in the format 'host:port' to be added.</param>
        public RedisCacheOptions WithEndPoint([NotNull] string hostAndPort)
        {
            _host = Guard.NotEmpty(hostAndPort, nameof(hostAndPort));

            return this;
        }

        /// <summary>
        /// Adds a value to the options specifying that SSL encryption should be used.
        /// </summary>
        /// <returns>The new options instance with the given ssl value added.</returns>
        public RedisCacheOptions WithSsl()
        {
            _ssl = true;

            return this;
        }

        /// <summary>
        /// Adds a value to the options indicating whether admin operations should be allowed.
        /// </summary>
        /// <returns>The new options instance with the given allow adming value added.</returns>
        public RedisCacheOptions WithAllowAdmin()
        {
            _allowAdmin = true;

            return this;
        }

        /// <summary>
        /// Adds the giving default database to the options.
        /// </summary>
        /// <param name="defaultDatabase">Specifies the default database to be used when calling ConnectionMultiplexer.GetDatabase() without any parameters.</param>
        /// <returns>The new options instance with the given default database added.</returns>
        public RedisCacheOptions WithDefaultDatabase(int defaultDatabase)
        {
            _defaultDatabase = defaultDatabase;

            return this;
        }

        /// <summary>
        /// Adds the giving caching expiration time to the options.
        /// </summary>
        /// <param name="expiry">The caching expiration time to be added.</param>
        /// <returns>The new options instance with the given caching expiration time added.</returns>
        public RedisCacheOptions WithExpiry([NotNull] TimeSpan expiry)
        {
            _expiry = Guard.NotNull(expiry, nameof(expiry));

            return this;
        }

        /// <summary>
        /// Adds the giving json serializer settings to the options.
        /// </summary>
        /// <param name="serializerSettings">The json srialzer settings to be added.</param>
        /// <returns>The new options instance with the given json serializer settings added.</returns>
        public RedisCacheOptions WithJsonSerializerSettings([NotNull] JsonSerializerSettings serializerSettings)
        {
            _serializerSettings = Guard.NotNull(serializerSettings, nameof(serializerSettings));

            return this;
        }
    }
}
