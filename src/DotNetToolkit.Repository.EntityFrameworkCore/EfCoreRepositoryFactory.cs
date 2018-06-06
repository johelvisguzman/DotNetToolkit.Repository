namespace DotNetToolkit.Repository.EntityFrameworkCore
{
    using Factories;
    using Interceptors;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactoryAsync" />.
    /// </summary>
    public class EfCoreRepositoryFactory : IRepositoryFactoryAsync
    {
        #region Fields

        private const string DbContextTypeKey = "dbContextType";
        private const string DbContextOptionsKey = "dbContextOptions";
        private const string InterceptorsKey = "interceptors";

        private readonly Dictionary<string, object> _options;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepositoryFactory"/> class.
        /// </summary>
        public EfCoreRepositoryFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepositoryFactory"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public EfCoreRepositoryFactory(Dictionary<string, object> options)
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
            DbContextOptions dbContextOptions = null;
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

            if (options.ContainsKey(DbContextOptionsKey))
            {
                value = options[DbContextOptionsKey];
                dbContextOptions = value as DbContextOptions;

                if (value != null && dbContextOptions == null)
                    throw new ArgumentException($"The option value for the specified '{DbContextOptionsKey}' key must be a valid 'Microsoft.EntityFrameworkCore.DbContextOptions' type.");
            }

            if (options.ContainsKey(InterceptorsKey))
            {
                value = options[InterceptorsKey];
                interceptors = value as IEnumerable<IRepositoryInterceptor>;

                if (value != null && interceptors == null)
                    throw new ArgumentException($"The option value for the specified '{InterceptorsKey}' key must be a valid 'System.Collections.Generic.IEnumerable<DotNetToolkit.Repository.IRepositoryInterceptor>' type.");
            }

            if (dbContextOptions == null)
                context = (DbContext)Activator.CreateInstance(contextType);
            else
                context = (DbContext)Activator.CreateInstance(contextType, dbContextOptions);
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

            return new EfCoreRepository<TEntity>(context, interceptors);
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

            return new EfCoreRepository<TEntity, TKey>(context, interceptors);
        }

        #endregion
    }
}
