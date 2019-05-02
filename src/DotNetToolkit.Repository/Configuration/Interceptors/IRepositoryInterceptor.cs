namespace DotNetToolkit.Repository.Configuration.Interceptors
{
    /// <summary>
    /// Provides the ability to intercept the activity of the repository, which will allow to execute custom logic before and after any activity.
    /// </summary>
    public interface IRepositoryInterceptor
    {
        /// <summary>
        /// An activity method which is executed when adding an entity to the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void AddExecuting<TEntity>(TEntity entity);

        /// <summary>
        /// An activity method which is executed when deleting an entity from the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void DeleteExecuting<TEntity>(TEntity entity);

        /// <summary>
        /// An activity method which is executed when updating an entity in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void UpdateExecuting<TEntity>(TEntity entity);
    }
}
