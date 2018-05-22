namespace DotNetToolkit.Repository.InMemory
{
    using Logging;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactory" />.
    /// </summary>
    public class InMemoryRepositoryFactory : IRepositoryFactory
    {
        #region Fields

        private const string DatabaseNameKey = "databaseName";
        private const string LoggerKey = "logger";

        private readonly Dictionary<string, object> _options;

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
        /// <param name="options">The options.</param>
        public InMemoryRepositoryFactory(Dictionary<string, object> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
        }

        #endregion

        #region Private Methods

        private static void GetOptions(Dictionary<string, object> options, out string databaseName, out ILogger logger)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.Count == 0)
                throw new InvalidOperationException("The options dictionary does not contain any items.");

            object value = null;
            databaseName = null;
            logger = null;

            if (options.ContainsKey(DatabaseNameKey))
            {
                value = options[DatabaseNameKey];
                databaseName = value as string;

                if (value != null && databaseName == null)
                    throw new ArgumentException($"The option value for the specified '{DatabaseNameKey}' key must be a valid '{typeof(string).Name}' type.");
            }

            if (options.ContainsKey(LoggerKey))
            {
                value = options[LoggerKey];
                logger = value as ILogger;

                if (value != null && logger == null)
                    throw new ArgumentException($"The option value for the specified '{LoggerKey}' key must be a valid '{typeof(ILogger).Name}' type.");
            }
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
            return Create<TEntity, TKey>(_options);
        }

        /// <summary>
        /// Creates a new repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="options">The options.</param>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity> Create<TEntity>(Dictionary<string, object> options) where TEntity : class
        {
            if (options == null)
                return new InMemoryRepository<TEntity>();

            GetOptions(options, out string databaseName, out ILogger logger);

            return new InMemoryRepository<TEntity>(databaseName, logger);
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
            if (options == null)
                return new InMemoryRepository<TEntity, TKey>();

            GetOptions(options, out string databaseName, out ILogger logger);

            return new InMemoryRepository<TEntity, TKey>(databaseName, logger);
        }

        #endregion
    }
}
