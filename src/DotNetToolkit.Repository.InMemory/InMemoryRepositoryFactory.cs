namespace DotNetToolkit.Repository.InMemory
{
    using Factories;
    using Interceptors;
    using System;
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
            return CreateInstance<InMemoryRepository<TEntity>>();
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class
        {
            return CreateInstance<InMemoryRepository<TEntity, TKey>>();
        }

        /// <summary>
        /// Creates a new repository for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The new repository.</returns>
        public T CreateInstance<T>() where T : class
        {
            if (string.IsNullOrEmpty(_databaseName))
                return (T)Activator.CreateInstance(typeof(T));

            var args = new List<object> { _databaseName };

            if (_interceptors.Any())
                args.Add(_interceptors);

            return (T)Activator.CreateInstance(typeof(T), args.ToArray());
        }

        #endregion
    }
}
