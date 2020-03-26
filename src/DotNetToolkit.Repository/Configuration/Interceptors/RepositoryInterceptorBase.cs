namespace DotNetToolkit.Repository.Configuration.Interceptors
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An implementation of <see cref="IRepositoryInterceptor" />.
    /// </summary>
    /// <seealso cref="IRepositoryInterceptor" />
    public abstract class RepositoryInterceptorBase : IRepositoryInterceptor
    {
        /// <summary>
        /// An activity method which is executed when adding an entity to the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="interceptionContext">The interception context which includes information for the current operation.</param>
        public virtual void AddExecuting<TEntity>(RepositoryInterceptionContext<TEntity> interceptionContext) where TEntity : class {}

        /// <summary>
        /// An activity method which is executed when deleting an entity from the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="interceptionContext">The interception context which includes information for the current operation.</param>
        public virtual void DeleteExecuting<TEntity>(RepositoryInterceptionContext<TEntity> interceptionContext) where TEntity : class {}

        /// <summary>
        /// An activity method which is executed when updating an entity in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="interceptionContext">The interception context which includes information for the current operation.</param>
        public virtual void UpdateExecuting<TEntity>(RepositoryInterceptionContext<TEntity> interceptionContext) where TEntity : class {}

        /// <summary>
        /// Asynchronously an activity method which is executed when adding an entity to the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="interceptionContext">The interception context which includes information for the current operation.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task AddExecutingAsync<TEntity>(RepositoryInterceptionContext<TEntity> interceptionContext, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class => Task.FromResult(0);

        /// <summary>
        /// Asynchronously an activity method which is executed when deleting an entity from the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="interceptionContext">The interception context which includes information for the current operation.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task DeleteExecutingAsync<TEntity>(RepositoryInterceptionContext<TEntity> interceptionContext, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class => Task.FromResult(0);

        /// <summary>
        /// Asynchronously an activity method which is executed when updating an entity in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="interceptionContext">The interception context which includes information for the current operation.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public virtual Task UpdateExecutingAsync<TEntity>(RepositoryInterceptionContext<TEntity> interceptionContext, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class => Task.FromResult(0);
    }
}
