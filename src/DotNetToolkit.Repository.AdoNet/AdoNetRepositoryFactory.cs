namespace DotNetToolkit.Repository.AdoNet
{
    using Interceptors;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactory" />.
    /// </summary>
    public class AdoNetRepositoryFactory : IRepositoryFactory
    {
        #region Fields

        private const string ProviderNameKey = "providerName";
        private const string ConnectionStringKey = "connectionString";
        private const string InterceptorsKey = "interceptors";

        private readonly Dictionary<string, object> _options;

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
        public AdoNetRepositoryFactory(Dictionary<string, object> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
        }

        #endregion

        #region Private Methods

        private static void GetOptions(Dictionary<string, object> options, out string providerName, out string connectionString, out IEnumerable<IRepositoryInterceptor> interceptors)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.Count == 0)
                throw new InvalidOperationException("The options dictionary does not contain any items.");

            object value = null;
            providerName = null;
            connectionString = null;
            interceptors = null;

            if (options.ContainsKey(ProviderNameKey))
            {
                value = options[ProviderNameKey];
                providerName = value as string;

                if (value != null && providerName == null)
                    throw new ArgumentException($"The option value for the specified '{ProviderNameKey}' key must be a valid 'System.String' type.");
            }

            if (options.ContainsKey(ConnectionStringKey))
            {
                value = options[ConnectionStringKey];
                connectionString = value as string;

                if (value != null && connectionString == null)
                    throw new ArgumentException($"The option value for the specified '{ConnectionStringKey}' key must be a valid 'System.String' type.");
            }
            else
            {
                throw new InvalidOperationException($"The '{ConnectionStringKey}' option is missing from the options dictionary.");
            }

            if (options.ContainsKey(InterceptorsKey))
            {
                value = options[InterceptorsKey];
                interceptors = value as IEnumerable<IRepositoryInterceptor>;

                if (value != null && interceptors == null)
                    throw new ArgumentException($"The option value for the specified '{InterceptorsKey}' key must be a valid 'System.Collections.Generic.IEnumerable<DotNetToolkit.Repository.IRepositoryInterceptor>' type.");
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
        public IRepository<TEntity> Create<TEntity>(Dictionary<string, object> options) where TEntity : class
        {
            GetOptions(options, out string providerName, out string connectionString, out IEnumerable<IRepositoryInterceptor> interceptors);

            return string.IsNullOrEmpty(providerName)
                ? new AdoNetRepository<TEntity>(connectionString, interceptors)
                : new AdoNetRepository<TEntity>(providerName, connectionString, interceptors);
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <param name="options">The options.</param>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>(Dictionary<string, object> options) where TEntity : class
        {
            GetOptions(options, out string providerName, out string connectionString, out IEnumerable<IRepositoryInterceptor> interceptors);

            return string.IsNullOrEmpty(providerName)
                ? new AdoNetRepository<TEntity, TKey>(connectionString, interceptors)
                : new AdoNetRepository<TEntity, TKey>(providerName, connectionString, interceptors);
        }

        #endregion
    }
}