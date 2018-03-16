namespace DotNetToolkit.Repository.Xml
{
    using System;
    using System.Linq;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactory" />.
    /// </summary>
    public class XmlRepositoryFactory : IRepositoryFactory
    {
        #region Private Methods

        private string GetFilePath(IRepositoryOptions options)
        {
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
        /// <param name="options">The options.</param>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity> Create<TEntity>(IRepositoryOptions options) where TEntity : class
        {
            return new XmlRepository<TEntity>(GetFilePath(options));
        }

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <param name="options">The options.</param>
        /// <returns>The new repository.</returns>
        public IRepository<TEntity, TKey> Create<TEntity, TKey>(IRepositoryOptions options) where TEntity : class
        {
            return new XmlRepository<TEntity, TKey>(GetFilePath(options));
        }

        #endregion
    }
}
