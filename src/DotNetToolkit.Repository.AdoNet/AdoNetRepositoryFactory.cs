namespace DotNetToolkit.Repository.AdoNet
{
    using System;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactory" />.
    /// </summary>
    public class AdoNetRepositoryFactory : IRepositoryFactory
    {
        #region Fields

        private readonly IRepositoryFactoryOptions _options;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryFactory"/> class.
        /// </summary>
        public AdoNetRepositoryFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryFactory"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public AdoNetRepositoryFactory(IRepositoryFactoryOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
        }

        #endregion

        #region Private Methods

        private void GetProviderAndConnectionString(IRepositoryFactoryOptions options, out string providerName, out string connectionString)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.DbContextArgs == null || options.DbContextArgs.Length == 0)
                throw new InvalidOperationException($"The repository options must provide a '{nameof(options.DbContextArgs)}'.");

            if (options.DbContextArgs.Length == 1)
            {
                var arg1 = options.DbContextArgs[0];
                connectionString = arg1 as string;

                if (arg1 != null && connectionString == null)
                    throw new ArgumentException($"The provided '{nameof(options.DbContextArgs)}' must be a valid string argument to be used as a connection string.");

                providerName = null;
            }
            else
            {
                var arg1 = options.DbContextArgs[0];
                providerName = arg1 as string;

                if (arg1 != null && providerName == null)
                    throw new ArgumentException($"The provided '{nameof(options.DbContextArgs)}' must be a valid string argument to be used as a provider name.");

                var arg2 = options.DbContextArgs[1];
                connectionString = arg1 as string;

                if (arg2 != null && connectionString == null)
                    throw new ArgumentException($"The provided '{nameof(options.DbContextArgs)}' must be a valid string argument to be used as a connection string.");
            }
        }

        #endregion

        #region Implementation of IRepositoryFactory

        /// <summary>
        /// Creates a new repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity> Create<TEntity>() where TEntity : class
        {
            if (_options == null)
                throw new InvalidOperationException("No options have been provided.");

            return Create<TEntity>(_options);
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class
        {
            if (_options == null)
                throw new InvalidOperationException("No options have been provided.");

            return Create<TEntity, TKey>(_options);
        }

        /// <summary>
        /// Creates a new repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="options">The options.</param>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity> Create<TEntity>(IRepositoryFactoryOptions options) where TEntity : class
        {
            GetProviderAndConnectionString(options, out string providerName, out string connectionString);

            return string.IsNullOrEmpty(providerName)
                ? new AdoNetRepository<TEntity>(connectionString, options.Logger)
                : new AdoNetRepository<TEntity>(providerName, connectionString, options.Logger);
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <param name="options">The options.</param>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>(IRepositoryFactoryOptions options) where TEntity : class
        {
            GetProviderAndConnectionString(options, out string providerName, out string connectionString);

            return string.IsNullOrEmpty(providerName)
                ? new AdoNetRepository<TEntity, TKey>(connectionString, options.Logger)
                : new AdoNetRepository<TEntity, TKey>(providerName, connectionString, options.Logger);
        }

        #endregion
    }
}
