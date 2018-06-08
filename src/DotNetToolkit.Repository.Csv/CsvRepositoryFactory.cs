namespace DotNetToolkit.Repository.Csv
{
    using Factories;
    using Interceptors;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactory" />.
    /// </summary>
    public class CsvRepositoryFactory : IRepositoryFactory
    {
        #region Fields

        private readonly string _path;
        private readonly IEnumerable<IRepositoryInterceptor> _interceptors;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepositoryFactory"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="interceptors">The interceptors.</param>
        public CsvRepositoryFactory(string path, IEnumerable<IRepositoryInterceptor> interceptors = null)
        {
            _path = path;
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
            return new CsvRepository<TEntity>(_path, _interceptors);
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class
        {
            return new CsvRepository<TEntity, TKey>(_path, _interceptors);
        }

        #endregion
    }
}
