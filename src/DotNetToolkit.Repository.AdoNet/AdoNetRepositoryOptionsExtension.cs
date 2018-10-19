namespace DotNetToolkit.Repository.AdoNet
{
    using Configuration.Options;
    using Factories;
    using System;

    /// <summary>
    /// An implementation of <see cref="IRepositoryOptionsContextFactoryExtensions" />.
    /// </summary>
    public class AdoNetRepositoryOptionsExtension : IRepositoryOptionsContextFactoryExtensions
    {
        private string _nameOrConnectionString;
        private string _providerName;
        private AdoNetRepositoryContextFactory _contextFactory;

        /// <summary>
        /// Returns the extension instance with a connection string.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        public AdoNetRepositoryOptionsExtension WithConnectionString(string nameOrConnectionString)
        {
            if (nameOrConnectionString == null)
                throw new ArgumentNullException(nameof(nameOrConnectionString));

            _nameOrConnectionString = nameOrConnectionString;

            return this;
        }

        /// <summary>
        /// Returns the extension instance with a provider and a connection string.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetRepositoryOptionsExtension WithProvider(string providerName, string connectionString)
        {
            if (providerName == null)
                throw new ArgumentNullException(nameof(providerName));

            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            _providerName = providerName;
            _nameOrConnectionString = connectionString;

            return this;
        }

        /// <summary>
        /// Gets the configured repository context factory.
        /// </summary>
        public IRepositoryContextFactory ContextFactory
        {
            get
            {
                if (_contextFactory == null)
                {
                    _contextFactory = !string.IsNullOrEmpty(_providerName)
                        ? new AdoNetRepositoryContextFactory(_providerName, _nameOrConnectionString)
                        : new AdoNetRepositoryContextFactory(_nameOrConnectionString);
                }

                return _contextFactory;
            }

        }

        /// <summary>
        /// Gets the type of the repository context factory.
        /// </summary>
        public Type ContextFactoryType { get { return typeof(AdoNetRepositoryContextFactory); } }
    }
}
