namespace DotNetToolkit.Repository.Configuration.Interceptors
{
    using System.Threading;
    using System.Threading.Tasks;

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

        /// <summary>
        /// Asynchronously an activity method which is executed when adding an entity to the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task AddExecutingAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously an activity method which is executed when deleting an entity from the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteExecutingAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously an activity method which is executed when updating an entity in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task UpdateExecutingAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken());
    }
}
