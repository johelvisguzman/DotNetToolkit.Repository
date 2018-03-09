namespace DotNetToolkit.Repository.Traits
{
    using Specifications;
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an asynchronous trait for aggregating items from a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface ICanAggregateAsync<TEntity> : ICanAggregate<TEntity> where TEntity : class
    {
        /// <summary>
        /// Asynchronously returns the number of entities contained in the repository.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities contained in the repository.</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="criteria" /> in the repository.
        /// </summary>
        /// <param name="criteria">The specification criteria that is used for matching entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities that satisfied the criteria specified by the <paramref name="criteria" /> in the repository..</returns>
        Task<int> CountAsync(ISpecification<TEntity> criteria, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository..</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));
    }
}
