namespace DotNetToolkit.Repository.AdoNet
{
    using Factories;
    using Interceptors;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactoryAsync" />.
    /// </summary>
    public class AdoNetRepositoryFactory : IRepositoryFactoryAsync
    {
        #region Fields

        private readonly string _connectionString;
        private readonly string _providerName;
        private readonly IEnumerable<IRepositoryInterceptor> _interceptors;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryFactory"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetRepositoryFactory(string connectionString, IEnumerable<IRepositoryInterceptor> interceptors = null)
        {
            _connectionString = connectionString;
            _interceptors = interceptors ?? Enumerable.Empty<IRepositoryInterceptor>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryFactory"/> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetRepositoryFactory(string providerName, string connectionString, IEnumerable<IRepositoryInterceptor> interceptors = null)
        {
            _connectionString = connectionString;
            _providerName = providerName;
            _interceptors = interceptors ?? Enumerable.Empty<IRepositoryInterceptor>();
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

        #endregion

        #region Implementation of IRepositoryFactoryAsync

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity> CreateAsync<TEntity>() where TEntity : class
        {
            return string.IsNullOrEmpty(_providerName)
                ? new AdoNetRepository<TEntity>(_connectionString, _interceptors)
                : new AdoNetRepository<TEntity>(_providerName, _connectionString, _interceptors);
        }

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity, TKey> CreateAsync<TEntity, TKey>() where TEntity : class
        {
            return string.IsNullOrEmpty(_providerName)
                ? new AdoNetRepository<TEntity, TKey>(_connectionString, _interceptors)
                : new AdoNetRepository<TEntity, TKey>(_providerName, _connectionString, _interceptors);
        }

        #endregion
    }
}