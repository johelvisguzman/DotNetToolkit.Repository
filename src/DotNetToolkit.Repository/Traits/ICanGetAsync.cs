namespace DotNetToolkit.Repository.Traits
{
    using FetchStrategies;
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a trait for getting items from a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanGet{TEntity, TKey}" />
    public interface ICanGetAsync<TEntity, in TKey> : ICanGet<TEntity, TKey> where TEntity : class
    {
        /// <summary>
        /// Asynchronously gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <return>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</return>
        Task<TEntity> GetAsync(TKey key, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <return>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</return>
        Task<TEntity> GetAsync(TKey key, IFetchStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        Task<TResult> GetAsync<TResult>(TKey key, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        Task<TResult> GetAsync<TResult>(TKey key, Expression<Func<TEntity, TResult>> selector, IFetchStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = default(CancellationToken));
    }
}

namespace DotNetToolkit.Repository.Transactions.Traits
{
    using FetchStrategies;
    using System;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a trait for getting items from a unit of work repository.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Transactions.Traits.ICanGet" />
    public interface ICanGetAsync : ICanGet
    {
        /// <summary>
        /// Asynchronously gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <return>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</return>
        Task<TEntity> GetAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;

        /// <summary>
        /// Asynchronously gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <return>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the  entity found.</return>
        Task<TEntity> GetAsync<TEntity, TKey>(TKey key, IFetchStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;

        /// <summary>
        /// Asynchronously gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <return>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</return>
        Task<TResult> GetAsync<TEntity, TKey, TResult>(TKey key, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;

        /// <summary>
        /// Asynchronously gets a specific projected entity result with the given primary key value in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        /// <typeparam name="TResult">The type of the projected result.</typeparam>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <return>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</return>
        Task<TResult> GetAsync<TEntity, TKey, TResult>(TKey key, Expression<Func<TEntity, TResult>> selector, IFetchStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the primary key.</typeparam>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        Task<bool> ExistsAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class;
    }
}
