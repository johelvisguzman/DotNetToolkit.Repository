namespace DotNetToolkit.Repository
{
    using Queries;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Wrappers;

    /// <summary>
    /// Represents an internal repository base.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepositoryBase{TEntity}" />
    /// <seealso cref="System.IDisposable" />
    [ComVisible(false)]
#if !NETSTANDARD1_3
    [Browsable(false)] 
#endif
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IRepositoryBase<TEntity> : IReadOnlyRepositoryBase<TEntity> where TEntity : class
    {
        /// <summary>
        /// Adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(TEntity entity);

        /// <summary>
        /// Adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        void Add(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        void Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        void Delete(IQueryOptions<TEntity> options);

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        void Delete(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        void Update(IEnumerable<TEntity> entities);

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entity" /> in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously updates the specified <paramref name="entities" /> collection in the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken());
    }
}
