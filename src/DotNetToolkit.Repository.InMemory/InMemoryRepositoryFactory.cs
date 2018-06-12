namespace DotNetToolkit.Repository.InMemory
{
    using Factories;
    using Interceptors;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactory" />.
    /// </summary>
    public class InMemoryRepositoryFactory : IRepositoryFactory
    {
        #region Fields

        private readonly string _databaseName;
        private readonly IEnumerable<IRepositoryInterceptor> _interceptors;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryFactory"/> class.
        /// </summary>
        public InMemoryRepositoryFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryFactory"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the database.</param>
        /// <param name="interceptors">The interceptors.</param>
        public InMemoryRepositoryFactory(string databaseName, IEnumerable<IRepositoryInterceptor> interceptors = null)
        {
            _databaseName = databaseName;
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
            return string.IsNullOrEmpty(_databaseName)
                ? new InMemoryRepository<TEntity>()
                : new InMemoryRepository<TEntity>(_databaseName, _interceptors);
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class
        {
            return string.IsNullOrEmpty(_databaseName)
                ? new InMemoryRepository<TEntity, TKey>()
                : new InMemoryRepository<TEntity, TKey>(_databaseName, _interceptors);
        }

        #endregion
    }
}
