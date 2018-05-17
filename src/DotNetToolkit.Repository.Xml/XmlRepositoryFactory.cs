namespace DotNetToolkit.Repository.Xml
{
    using System;
    using System.Linq;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactory" />.
    /// </summary>
    public class XmlRepositoryFactory : IRepositoryFactory
    {
        #region Fields

        private readonly IRepositoryFactoryOptions _options;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepositoryFactory"/> class.
        /// </summary>
        public XmlRepositoryFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepositoryFactory"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public XmlRepositoryFactory(IRepositoryFactoryOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
        }

        #endregion

        #region Private Methods

        private string GetFilePath(IRepositoryFactoryOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var arg = options.DbContextArgs.FirstOrDefault();
            var databaseName = arg as string;

            if (arg != null && databaseName == null)
                throw new ArgumentException($"The provided {nameof(options.DbContextArgs)} must be a valid string argument.");

            return databaseName;
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
        public IRepository<TEntity> Create<TEntity>(IRepositoryFactoryOptions options) where TEntity : class
        {
            return new XmlRepository<TEntity>(GetFilePath(options), options.Logger);
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <param name="options">The options.</param>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>(IRepositoryFactoryOptions options) where TEntity : class
        {
            return new XmlRepository<TEntity, TKey>(GetFilePath(options), options.Logger);
        }

        #endregion
    }
}
