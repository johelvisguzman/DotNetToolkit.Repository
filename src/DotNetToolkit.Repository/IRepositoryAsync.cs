namespace DotNetToolkit.Repository
{
    using Queries;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Wrappers;

    /// <summary>
    /// Represents an asynchronous repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepository{TEntity, TKey}" />
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepositoryAsync{TEntity}" />
    /// <seealso cref="System.IDisposable" />
    public interface IRepositoryAsync<TEntity, in TKey> : IRepository<TEntity, TKey>, IReadOnlyRepositoryAsync<TEntity, TKey>, IDisposable where TEntity : class
    {
        /// <summary>
        /// Returns a read-only <see cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepositoryAsync{TEntity}" /> wrapper for the current repository.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current repository.</returns>
        IReadOnlyRepositoryAsync<TEntity, TKey> AsReadOnlyAsync();

        /// <summary>
        /// Adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken());
    }

    /// <summary>
    /// Represents an asynchronous repository with a default primary key value of type integer.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepositoryAsync{TEntity, TKey}" />
    /// <seealso cref="DotNetToolkit.Repository.IRepository{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepositoryAsync{TEntity,TKey}" />
    public interface IRepositoryAsync<TEntity> : IRepositoryAsync<TEntity, int>, IRepository<TEntity>, IReadOnlyRepositoryAsync<TEntity> where TEntity : class
    {
    }
}
