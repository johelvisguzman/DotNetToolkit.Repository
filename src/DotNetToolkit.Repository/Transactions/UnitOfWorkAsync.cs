namespace DotNetToolkit.Repository.Transactions
{
    using Factories;
    using Interceptors;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="IUnitOfWorkAsync" />.
    /// </summary>
    public class UnitOfWorkAsync : UnitOfWork, IUnitOfWorkAsync
    {
        #region Fields

        private readonly IRepositoryFactoryAsync _factory;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkAsync" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        public UnitOfWorkAsync(IRepositoryContextAsync context) : this(context, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkAsync" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public UnitOfWorkAsync(IRepositoryContextAsync context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkAsync" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public UnitOfWorkAsync(IRepositoryContextAsync context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _factory = new RepositoryFactoryAsync(() => context, interceptors);
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
            return _factory.CreateAsync<TEntity>();
        }

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity, TKey> CreateAsync<TEntity, TKey>() where TEntity : class
        {
            return _factory.CreateAsync<TEntity, TKey>();
        }

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and a composite primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
        /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity, TKey1, TKey2> CreateAsync<TEntity, TKey1, TKey2>() where TEntity : class
        {
            return _factory.CreateAsync<TEntity, TKey1, TKey2>();
        }

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and a composite primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
        /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
        /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepositoryAsync<TEntity, TKey1, TKey2, TKey3> CreateAsync<TEntity, TKey1, TKey2, TKey3>() where TEntity : class
        {
            return _factory.CreateAsync<TEntity, TKey1, TKey2, TKey3>();
        }

        #endregion
    }
}
