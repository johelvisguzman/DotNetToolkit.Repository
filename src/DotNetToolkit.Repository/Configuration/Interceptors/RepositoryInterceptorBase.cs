namespace DotNetToolkit.Repository.Configuration.Interceptors
{
    /// <summary>
    /// An implementation of <see cref="IRepositoryInterceptor" />.
    /// </summary>
    /// <seealso cref="IRepositoryInterceptor" />
    public abstract class RepositoryInterceptorBase : IRepositoryInterceptor
    {
        #region Implementation of IRepositoryInterceptor

        /// <summary>
        /// An activity method which is executed when adding an entity to the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public virtual void AddExecuting<TEntity>(TEntity entity) { }

        /// <summary>
        /// An activity method which is executed when deleting an entity from the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public virtual void DeleteExecuting<TEntity>(TEntity entity) { }

        /// <summary>
        /// An activity method which is executed when updating an entity in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public virtual void UpdateExecuting<TEntity>(TEntity entity) { }

        #endregion
    }
}
