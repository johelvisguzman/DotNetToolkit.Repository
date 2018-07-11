namespace DotNetToolkit.Repository.Wrappers
{
    using FetchStrategies;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an asynchronous read-only repository with a composite primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepositoryBaseAsync{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepository{TEntity, TKey1, TKey2, TKey3}" />
    /// <seealso cref="System.IDisposable" />
    public interface IReadOnlyRepositoryAsync<TEntity, in TKey1, in TKey2, in TKey3> : IReadOnlyRepositoryBaseAsync<TEntity>, IReadOnlyRepository<TEntity, TKey1, TKey2, TKey3>, IDisposable where TEntity : class
    {
        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        Task<bool> ExistsAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, TKey3 key3, IFetchStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken());
    }

    /// <summary>
    /// Represents an asynchronous read-only repository with a composite primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepositoryBaseAsync{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepository{TEntity, TKey1, TKey2}" />
    /// <seealso cref="System.IDisposable" />
    public interface IReadOnlyRepositoryAsync<TEntity, in TKey1, in TKey2> : IReadOnlyRepositoryBaseAsync<TEntity>, IReadOnlyRepository<TEntity, TKey1, TKey2>, IDisposable where TEntity : class
    {
        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        Task<bool> ExistsAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, IFetchStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken());
    }

    /// <summary>
    /// Represents an asynchronous repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepositoryBaseAsync{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepository{TEntity, TKey}" />
    /// <seealso cref="System.IDisposable" />
    public interface IReadOnlyRepositoryAsync<TEntity, in TKey> : IReadOnlyRepositoryBaseAsync<TEntity>, IReadOnlyRepository<TEntity, TKey>, IDisposable where TEntity : class
    {
        /// <summary>
        /// Asynchronously determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        Task<TEntity> GetAsync(TKey key, CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Asynchronously gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found.</returns>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task<TEntity> GetAsync(TKey key, IFetchStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken());
    }

    /// <summary>
    /// Represents an asynchronous repository with a default primary key value of type integer.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepositoryAsync{TEntity, TKey}" />
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepository{TEntity}" />
    public interface IReadOnlyRepositoryAsync<TEntity> : IReadOnlyRepositoryAsync<TEntity, int>, IReadOnlyRepository<TEntity> where TEntity : class
    {
    }
}
