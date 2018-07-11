namespace DotNetToolkit.Repository.Interceptors
{
    using System;
    using Logging;

    /// <summary>
    /// An implementation of <see cref="IRepositoryInterceptor" />.
    /// </summary>
    /// <seealso cref="IRepositoryInterceptor" />
    public class RepositoryInterceptor : IRepositoryInterceptor
    {
        #region Fields

        private readonly ILogger _logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryInterceptor" /> class.
        /// </summary>
        public RepositoryInterceptor() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryInterceptor" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public RepositoryInterceptor(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        #endregion

        #region Implementation of IRepositoryInterceptor

        /// <summary>
        /// An activity method which is executed when adding an entity to the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public virtual void AddExecuting<TEntity>(TEntity entity)
        {
            _logger?.Write($"Adding '{typeof(TEntity).FullName}' entity", entity);
        }

        /// <summary>
        /// An activity method which is executed when an entity has been added to the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public virtual void AddExecuted<TEntity>(TEntity entity)
        {
            _logger?.Write($"Added '{typeof(TEntity).FullName}' entity", entity);
        }

        /// <summary>
        /// An activity method which is executed when deleting an entity from the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public virtual void DeleteExecuting<TEntity>(TEntity entity)
        {
            _logger?.Write($"Deleting '{typeof(TEntity).FullName}' entity", entity);
        }

        /// <summary>
        /// An activity method which is executed when an entity has been deleted from the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public virtual void DeleteExecuted<TEntity>(TEntity entity)
        {
            _logger?.Write($"Deleted '{typeof(TEntity).FullName}' entity", entity);
        }

        /// <summary>
        /// An activity method which is executed when updating an entity in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public virtual void UpdateExecuting<TEntity>(TEntity entity)
        {
            _logger?.Write($"Updating '{typeof(TEntity).FullName}' entity", entity);
        }

        /// <summary>
        /// An activity method which is executed when an entity has been updated in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public virtual void UpdateExecuted<TEntity>(TEntity entity)
        {
            _logger?.Write($"Updated '{typeof(TEntity).FullName}' entity", entity);
        }

        /// <summary>
        /// An activity method which is executed when an error/exception occurs in the repository.
        /// </summary>
        /// <param name="ex">The ex.</param>
        public virtual void Error(Exception ex)
        {
            _logger?.Write(ex);
        }

        #endregion
    }
}
