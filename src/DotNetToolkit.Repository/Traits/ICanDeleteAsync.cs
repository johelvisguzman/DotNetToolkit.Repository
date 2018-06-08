namespace DotNetToolkit.Repository.Traits
{
    using Queries;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an asynchronous trait for deleting items from a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanDelete{TEntity, TKey}" />
    public interface ICanDeleteAsync<TEntity, in TKey> : ICanDelete<TEntity, TKey> where TEntity : class
    {
        /// <summary>
        /// Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(TKey key, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
    }
}
