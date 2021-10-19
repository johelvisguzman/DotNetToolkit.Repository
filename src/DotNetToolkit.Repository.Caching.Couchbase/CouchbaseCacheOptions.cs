namespace DotNetToolkit.Repository.Caching.Couchbase
{
    using global::Couchbase.Core.Serialization;
    using JetBrains.Annotations;
    using System;
    using Utility;

    /// <summary>
    /// The options to be used by the couchbase caching provider.
    /// </summary>
    public class CouchbaseCacheOptions
    {
        private string _host;
        private string _bucketName;
        private string _username;
        private string _password;
        private TimeSpan? _expiry;
        private Func<ITypeSerializer> _serializer;

        /// <summary>
        /// Gets the host.
        /// </summary>
        public string Host { get { return _host; } }

        /// <summary>
        /// Gets the bucket name.
        /// </summary>
        public string BucketName { get { return _bucketName; } }

        /// <summary>
        /// Gets the username.
        /// </summary>
        public string Username { get { return _username; } }

        /// <summary>
        /// Gets the password.
        /// </summary>
        public string Password { get { return _password; } }

        /// <summary>
        /// Gets the expiration time.
        /// </summary>
        public TimeSpan? Expiry { get { return _expiry; } }

        /// <summary>
        /// Gets the serializer.
        /// </summary>
        public Func<ITypeSerializer> Serializer { get { return _serializer; } }

        /// <summary>
        /// Adds the giving bucketname to the options.
        /// </summary>
        /// <param name="bucketname">The bucketname to be added.</param>
        public CouchbaseCacheOptions WithBucketName([NotNull] string bucketname)
        {
            _bucketName = Guard.NotEmpty(bucketname, nameof(bucketname));

            return this;
        }

        /// <summary>
        /// Adds the giving username to the options.
        /// </summary>
        /// <param name="username">The username to be added.</param>
        public CouchbaseCacheOptions WithUsername([NotNull] string username)
        {
            _username = Guard.NotEmpty(username, nameof(username));

            return this;
        }

        /// <summary>
        /// Adds the giving password to the options.
        /// </summary>
        /// <param name="password">The password to be added.</param>
        public CouchbaseCacheOptions WithPassword([NotNull] string password)
        {
            _password = Guard.NotEmpty(password, nameof(password));

            return this;
        }

        /// <summary>
        /// Adds the giving endpoint to the options.
        /// </summary>
        /// <param name="host">The host to be added.</param>
        /// <param name="port">The port to be added.</param>
        public CouchbaseCacheOptions WithEndPoint([NotNull] string host, int port)
        {
            Guard.NotEmpty(host, nameof(host));

            _host = string.Format("{0}:{1}", host, port);

            return this;
        }

        /// <summary>
        /// Adds the giving endpoint to the options.
        /// </summary>
        /// <param name="hostAndPort">The address and the port of the server in the format 'host:port' to be added.</param>
        public CouchbaseCacheOptions WithEndPoint([NotNull] string hostAndPort)
        {
            _host = Guard.NotEmpty(hostAndPort, nameof(hostAndPort));

            return this;
        }

        /// <summary>
        /// Adds the giving caching expiration time to the options.
        /// </summary>
        /// <param name="expiry">The caching expiration time to be added.</param>
        public CouchbaseCacheOptions WithExpiry([NotNull] TimeSpan expiry)
        {
            _expiry = Guard.NotNull(expiry, nameof(expiry));

            return this;
        }

        /// <summary>
        /// Adds the giving json serializer settings to the options.
        /// </summary>
        /// <param name="serializer">The serializer to be added.</param>
        /// <returns>The new options instance with the given json serializer settings added.</returns>
        public CouchbaseCacheOptions WithSerializer([NotNull] Func<ITypeSerializer> serializer)
        {
            _serializer = Guard.NotNull(serializer, nameof(serializer));

            return this;
        }
    }
}
