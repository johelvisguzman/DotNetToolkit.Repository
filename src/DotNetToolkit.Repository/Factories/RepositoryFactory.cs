namespace DotNetToolkit.Repository.Factories
{
    using Configuration;
    using Configuration.Interceptors;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Factories.IRepositoryFactory" />
    public class RepositoryFactory : IRepositoryFactory
    {
        #region Fields

        private readonly IRepositoryContextFactory _factory;
        private readonly IEnumerable<IRepositoryInterceptor> _interceptors;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory" /> class.
        /// </summary>
        /// <param name="factory">The repository context factory.</param>
        public RepositoryFactory(IRepositoryContextFactory factory) : this(new RepositoryConfigurationOptions(factory)) { }

#if !NETSTANDARD
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory" /> class and uses the data from the App.config file to configure the repositories.
        /// </summary>
        public RepositoryFactory() : this(Internal.ConfigFile.ConfigurationHelper.GetRequiredConfigurationOptions()) { }
#endif
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory" /> class.
        /// </summary>
        /// <param name="configuration">The repository configuration.</param>
        internal RepositoryFactory(IRepositoryConfigurationOptions configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _factory = configuration.GetContextFactory();
            _interceptors = configuration.GetInterceptors();
        }

        #endregion

        #region Implementation of IRepositoryFactory

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepository<TEntity> Create<TEntity>() where TEntity : class
        {
            return CreateInstance<Repository<TEntity>>();
        }

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class
        {
            return CreateInstance<Repository<TEntity, TKey>>();
        }

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and a composite primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
        /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepository<TEntity, TKey1, TKey2> Create<TEntity, TKey1, TKey2>() where TEntity : class
        {
            return CreateInstance<Repository<TEntity, TKey1, TKey2>>();
        }

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and a composite primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
        /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
        /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        public IRepository<TEntity, TKey1, TKey2, TKey3> Create<TEntity, TKey1, TKey2, TKey3>() where TEntity : class
        {
            return CreateInstance<Repository<TEntity, TKey1, TKey2, TKey3>>();
        }

        /// <summary>
        /// Creates a new repository for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The new repository.</returns>
        public T CreateInstance<T>() where T : class
        {
            var args = new List<object> { _factory };

            if (_interceptors.Any())
                args.Add(_interceptors);

            try
            {
                return (T)Activator.CreateInstance(typeof(T), args.ToArray());
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        #endregion
    }
}
