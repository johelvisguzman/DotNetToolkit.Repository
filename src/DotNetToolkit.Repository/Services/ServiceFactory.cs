namespace DotNetToolkit.Repository.Services
{
    using JetBrains.Annotations;
    using System;
    using Transactions;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IServiceFactory" />.
    /// </summary>
    /// <seealso cref="IServiceFactory" />
    public class ServiceFactory : IServiceFactory
    {
        #region Fields

        private readonly IUnitOfWorkFactory _uowFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceFactory"/> class.
        /// </summary>
        /// <param name="unitOfWorkFactory">The unit of work factory.</param>
        public ServiceFactory([NotNull] IUnitOfWorkFactory unitOfWorkFactory)
        {
            _uowFactory = Guard.NotNull(unitOfWorkFactory, nameof(unitOfWorkFactory));
        }

        #endregion

        #region Implementation of IServiceFactory

        /// <summary>
        /// Creates a new service for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The new service.</returns>
        public IService<TEntity> Create<TEntity>() where TEntity : class
        {
            return new Service<TEntity>(_uowFactory);
        }

        /// <summary>
        /// Creates a new service for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new service.</returns>
        public IService<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class
        {
            return new Service<TEntity, TKey>(_uowFactory);
        }

        /// <summary>
        /// Creates a new service for the specified entity and a composite primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
        /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
        /// <returns>The new service.</returns>
        public IService<TEntity, TKey1, TKey2> Create<TEntity, TKey1, TKey2>() where TEntity : class
        {
            return new Service<TEntity, TKey1, TKey2>(_uowFactory);
        }

        /// <summary>
        /// Creates a new service for the specified entity and a composite primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
        /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
        /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
        /// <returns>The new service.</returns>
        public IService<TEntity, TKey1, TKey2, TKey3> Create<TEntity, TKey1, TKey2, TKey3>() where TEntity : class
        {
            return new Service<TEntity, TKey1, TKey2, TKey3>(_uowFactory);
        }

        /// <summary>
        /// Creates a new service for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The new service.</returns>
        public T CreateInstance<T>() where T : class
        {
            return (T)Activator.CreateInstance(typeof(T), new object[] { _uowFactory });
        }

        #endregion
    }
}
