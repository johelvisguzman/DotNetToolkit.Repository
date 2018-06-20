namespace DotNetToolkit.Repository.EntityFramework
{
    using Factories;
    using Interceptors;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactoryAsync" />.
    /// </summary>
    public class EfRepositoryFactory : IRepositoryFactoryAsync
    {
        #region Fields

        private readonly Func<DbContext> _dbContextFactory;
        private readonly IEnumerable<IRepositoryInterceptor> _interceptors;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryFactory"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The database context factory.</param>
        public EfRepositoryFactory(Func<DbContext> dbContextFactory) : this(dbContextFactory, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryFactory"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The database context factory.</param>
        /// <param name="interceptor">The interceptor.</param>
        public EfRepositoryFactory(Func<DbContext> dbContextFactory, IRepositoryInterceptor interceptor) : this(dbContextFactory, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryFactory"/> class.
        /// </summary>
        /// <param name="dbContextFactory">The database context factory.</param>
        /// <param name="interceptors">The interceptors.</param>
        public EfRepositoryFactory(Func<DbContext> dbContextFactory, IEnumerable<IRepositoryInterceptor> interceptors)
        {
            if (dbContextFactory == null)
                throw new ArgumentNullException(nameof(dbContextFactory));

            _dbContextFactory = dbContextFactory;
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

        /// <summary>
        /// Creates a new repository for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The new repository.</returns>
        public T CreateInstance<T>() where T : class
        {
            var args = new List<object> { _dbContextFactory() };

            if (_interceptors.Any())
                args.Add(_interceptors);

            return (T)Activator.CreateInstance(typeof(T), args.ToArray());
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
            return CreateInstance<EfRepository<TEntity>>();
        }

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity, TKey> CreateAsync<TEntity, TKey>() where TEntity : class
        {
            return CreateInstance<EfRepository<TEntity, TKey>>();
        }

        #endregion
    }
}
