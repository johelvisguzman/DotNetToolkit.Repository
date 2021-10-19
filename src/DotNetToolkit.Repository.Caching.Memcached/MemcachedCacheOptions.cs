namespace DotNetToolkit.Repository.Caching.Memcached
{
    using Enyim.Caching.Memcached;
    using JetBrains.Annotations;
    using Newtonsoft.Json;
    using System;
    using Utility;

    /// <summary>
    /// The options to be used by the memcached caching provider.
    /// </summary>
    public class MemcachedCacheOptions
    {
        private string _host;
        private string _username;
        private string _password;
        private MemcachedProtocol _protocal;
        private Type _authType;
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
        /// Gets the protocal.
        /// </summary>
        public MemcachedProtocol Protocal { get { return _protocal; } }

        /// <summary>
        /// Gets the authentication type.
        /// </summary>
        public Type AuthenticationType { get { return _authType; } }

        /// <summary>
        /// Gets the expiration time.
        /// </summary>
        public TimeSpan? Expiry { get { return _expiry; } }

        /// <summary>
        /// Gets the json serializer settings.
        /// </summary>
        public JsonSerializerSettings SerializerSettings { get { return _serializerSettings; } }

        /// <summary>
        /// Adds the giving username to the options.
        /// </summary>
        /// <param name="username">The user name to be added.</param>
        public MemcachedCacheOptions WithUserName([NotNull] string username)
        {
            _username = Guard.NotEmpty(username, nameof(username));

            return this;
        }

        /// <summary>
        /// Adds the giving password to the options.
        /// </summary>
        /// <param name="password">The password to be added.</param>
        public MemcachedCacheOptions WithPassword([NotNull] string password)
        {
            _password = Guard.NotEmpty(password, nameof(password));

            return this;
        }

        /// <summary>
        /// Adds the giving endpoint to the options.
        /// </summary>
        /// <param name="host">The host name or IP address of the server to be added.</param>
        /// <param name="port">The port to be added.</param>
        public MemcachedCacheOptions WithEndPoint([NotNull] string host, int port)
        {
            Guard.NotEmpty(host, nameof(host));

            _host = string.Format("{0}:{1}", host, port);

            return this;
        }

        /// <summary>
        /// Adds the giving endpoint to the options.
        /// </summary>
        /// <param name="hostAndPort">The address and the port of the server in the format 'host:port' to be added.</param>
        public MemcachedCacheOptions WithEndPoint([NotNull] string hostAndPort)
        {
            _host = Guard.NotEmpty(hostAndPort, nameof(hostAndPort));

            return this;
        }

        /// <summary>
        /// Adds the giving protocal to the options.
        /// </summary>
        /// <param name="protocal">The protocal to be added.</param>
        public MemcachedCacheOptions WithProtocal([NotNull] MemcachedProtocol protocal)
        {
            _protocal = Guard.NotNull(protocal, nameof(protocal));

            return this;
        }

        /// <summary>
        /// Adds the giving authentication type to the options.
        /// </summary>
        public MemcachedCacheOptions WithAuthType<T>()
        {
            _authType = typeof(T);

            return this;
        }

        /// <summary>
        /// Adds the giving caching expiration time to the options.
        /// </summary>
        /// <param name="expiry">The caching expiration time to be added.</param>
        public MemcachedCacheOptions WithExpiry([NotNull] TimeSpan expiry)
        {
            _expiry = Guard.NotNull(expiry, nameof(expiry));

            return this;
        }

        /// <summary>
        /// Adds the giving json serializer settings to the options.
        /// </summary>
        /// <param name="serializerSettings">The json srialzer settings to be added.</param>
        public MemcachedCacheOptions WithJsonSerializerSettings([NotNull] JsonSerializerSettings serializerSettings)
        {
            _serializerSettings = Guard.NotNull(serializerSettings, nameof(serializerSettings));

            return this;
        }
    }
}
