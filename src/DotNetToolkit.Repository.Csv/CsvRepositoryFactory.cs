namespace DotNetToolkit.Repository.Csv
{
    using Factories;
    using Interceptors;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactory" />.
    /// </summary>
    public class CsvRepositoryFactory : IRepositoryFactory
    {
        #region Fields

        private const string FilePathOptionKey = "path";
        private const string InterceptorsKey = "interceptors";

        private readonly Dictionary<string, object> _options;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepositoryFactory"/> class.
        /// </summary>
        public CsvRepositoryFactory()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepositoryFactory"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public CsvRepositoryFactory(Dictionary<string, object> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options;
        }

        #endregion

        #region Private Methods

        private static void GetOptions(Dictionary<string, object> options, out string path, out IEnumerable<IRepositoryInterceptor> interceptors)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.Count == 0)
                throw new InvalidOperationException("The options dictionary does not contain any items.");

            object value = null;
            path = null;
            interceptors = null;

            if (options.ContainsKey(FilePathOptionKey))
            {
                value = options[FilePathOptionKey];
                path = value as string;

                if (value != null && path == null)
                    throw new ArgumentException($"The option value for the specified '{FilePathOptionKey}' key must be a valid 'System.String' type.");
            }
            else
            {
                throw new InvalidOperationException($"The '{FilePathOptionKey}' option is missing from the options dictionary.");
            }

            if (options.ContainsKey(InterceptorsKey))
            {
                value = options[InterceptorsKey];
                interceptors = value as IEnumerable<IRepositoryInterceptor>;

                if (value != null && interceptors == null)
                    throw new ArgumentException($"The option value for the specified '{InterceptorsKey}' key must be a valid 'System.Collections.Generic.IEnumerable<DotNetToolkit.Repository.IRepositoryInterceptor>' type.");
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
        public IRepository<TEntity> Create<TEntity>(Dictionary<string, object> options) where TEntity : class
        {
            GetOptions(options, out string path, out IEnumerable<IRepositoryInterceptor> interceptors);

            return new CsvRepository<TEntity>(path, interceptors);
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
            GetOptions(options, out string path, out IEnumerable<IRepositoryInterceptor> interceptors);

            return new CsvRepository<TEntity, TKey>(path, interceptors);
        }

        #endregion
    }
}
