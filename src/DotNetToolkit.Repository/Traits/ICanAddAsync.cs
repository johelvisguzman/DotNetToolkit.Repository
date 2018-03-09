namespace DotNetToolkit.Repository.Traits
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an asynchronous trait for adding items to a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface ICanAddAsync<in TEntity> where TEntity : class
    {
        /// <summary>
        /// Asynchronously adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
    }
}
