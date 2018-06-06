namespace DotNetToolkit.Repository.EntityFramework
{
    using Factories;
    using Interceptors;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactoryAsync" />.
    /// </summary>
    public class EfRepositoryFactory : IRepositoryFactoryAsync
    {
        #region Fields

        private const string DbContextTypeKey = "dbContextType";
        private const string ConnectionStringKey = "connectionString";
        private const string InterceptorsKey = "interceptors";

        private readonly Dictionary<string, object> _options;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryFactory"/> class.
        /// </summary>
        public EfRepositoryFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryFactory"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public EfRepositoryFactory(Dictionary<string, object> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
        }

        #endregion

        #region Private Methods

        private static void GetOptions(Dictionary<string, object> options, out DbContext context, out IEnumerable<IRepositoryInterceptor> interceptors)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.Count == 0)
                throw new InvalidOperationException("The options dictionary does not contain any items.");

            object value = null;
            string connectionString = null;
            Type contextType = null;
            context = null;
            interceptors = null;

            if (options.ContainsKey(DbContextTypeKey))
            {
                value = options[DbContextTypeKey];
                contextType = value as Type;

                if (value != null && contextType == null)
                    throw new ArgumentException($"The option value for the specified '{DbContextTypeKey}' key must be a valid 'System.Type' type.");
            }
            else
            {
                throw new InvalidOperationException($"The '{DbContextTypeKey}' option is missing from the options dictionary.");
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

            if (!string.IsNullOrEmpty(connectionString))
                context = (DbContext)Activator.CreateInstance(contextType, connectionString);
            else
                context = (DbContext)Activator.CreateInstance(contextType);
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
            return CreateAsync<TEntity>();
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class
        {
            return CreateAsync<TEntity, TKey>();
        }

        /// <summary>
        /// Creates a new repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="options">The options.</param>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity> Create<TEntity>(Dictionary<string, object> options) where TEntity : class
        {
            return CreateAsync<TEntity>(options);
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
            return CreateAsync<TEntity, TKey>(options);
        }

        #endregion

        #region Implementation of IRepositoryFactoryAsync

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity> CreateAsync<TEntity>() where TEntity : class
        {
            if (_options == null)
                throw new InvalidOperationException("No options have been provided.");

            return CreateAsync<TEntity>(_options);
        }

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity, TKey> CreateAsync<TEntity, TKey>() where TEntity : class
        {
            if (_options == null)
                throw new InvalidOperationException("No options have been provided.");

            return CreateAsync<TEntity, TKey>(_options);
        }

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="options">The options.</param>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity> CreateAsync<TEntity>(Dictionary<string, object> options) where TEntity : class
        {
            GetOptions(options, out DbContext context, out IEnumerable<IRepositoryInterceptor> interceptors);

            return new EfRepository<TEntity>(context, interceptors);
        }

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <param name="options">The options.</param>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity, TKey> CreateAsync<TEntity, TKey>(Dictionary<string, object> options) where TEntity : class
        {
            GetOptions(options, out DbContext context, out IEnumerable<IRepositoryInterceptor> interceptors);

            return new EfRepository<TEntity, TKey>(context, interceptors);
        }

        #endregion
    }
}
