namespace DotNetToolkit.Repository.Traits
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an asynchronous trait for updating items in a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanUpdate{TEntity}" />
    public interface ICanUpdateAsync<in TEntity> : ICanUpdate<TEntity> where TEntity : class
    {
        /// <summary>
        /// Asynchronously updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
    }

    /// <summary>
    /// Represents a trait for updating items in a repository.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanUpdate" />
    public interface ICanUpdateAsync : ICanUpdate
    {
        /// <summary>
        /// Asynchronously updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task UpdateAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;
    }
}
