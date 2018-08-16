namespace DotNetToolkit.Repository.Configuration.Interceptors
{
    using System;

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
        /// An activity method which is executed when an entity has been added to the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void AddExecuted<TEntity>(TEntity entity);

        /// <summary>
        /// An activity method which is executed when deleting an entity from the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void DeleteExecuting<TEntity>(TEntity entity);

        /// <summary>
        /// An activity method which is executed when an entity has been deleted from the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void DeleteExecuted<TEntity>(TEntity entity);

        /// <summary>
        /// An activity method which is executed when updating an entity in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void UpdateExecuting<TEntity>(TEntity entity);

        /// <summary>
        /// An activity method which is executed when an entity has been updated in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void UpdateExecuted<TEntity>(TEntity entity);

        /// <summary>
        /// An activity method which is executed when an error/exception occurs in the repository.
        /// </summary>
        /// <param name="ex">The ex.</param>
        void Error(Exception ex);
    }
}
