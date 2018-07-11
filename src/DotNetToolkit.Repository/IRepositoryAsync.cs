namespace DotNetToolkit.Repository
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Wrappers;

    /// <summary>
    /// Represents an asynchronous repository with a composite primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepositoryBaseAsync{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepositoryAsync{TEntity, TKey1, TKey2, TKey3}" />
    /// <seealso cref="System.IDisposable" />
    public interface IRepositoryAsync<TEntity, in TKey1, in TKey2, in TKey3> : IRepositoryBaseAsync<TEntity>, IReadOnlyRepositoryAsync<TEntity, TKey1, TKey2, TKey3>, IRepository<TEntity, TKey1, TKey2, TKey3>, IDisposable where TEntity : class
    {
        /// <summary>
        /// Returns a read-only asynchronous <see cref="IReadOnlyRepository{TEntity, TKey1, TKey2, TKey3}" /> wrapper for the current repository.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current repository.</returns>
        IReadOnlyRepositoryAsync<TEntity, TKey1, TKey2, TKey3> AsReadOnlyAsync();

        /// <summary>
        /// Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken());
    }

    /// <summary>
    /// Represents an asynchronous repository with a composite primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepositoryBaseAsync{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepositoryAsync{TEntity, TKey1, TKey2}" />
    /// <seealso cref="System.IDisposable" />
    public interface IRepositoryAsync<TEntity, in TKey1, in TKey2> : IRepositoryBaseAsync<TEntity>, IReadOnlyRepositoryAsync<TEntity, TKey1, TKey2>, IRepository<TEntity, TKey1, TKey2>, IDisposable where TEntity : class
    {
        /// <summary>
        /// Returns a read-only asynchronous <see cref="IReadOnlyRepository{TEntity, TKey1, TKey2}" /> wrapper for the current repository.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current repository.</returns>
        IReadOnlyRepositoryAsync<TEntity, TKey1, TKey2> AsReadOnlyAsync();

        /// <summary>
        /// Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken());
    }

    /// <summary>
    /// Represents a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepositoryBaseAsync{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepositoryAsync{TEntity, TKey}" />
    /// <seealso cref="System.IDisposable" />
    public interface IRepositoryAsync<TEntity, in TKey> : IRepositoryBaseAsync<TEntity>, IReadOnlyRepositoryAsync<TEntity, TKey>, IRepository<TEntity, TKey>, IDisposable where TEntity : class
    {
        /// <summary>
        /// Returns a read-only asynchronous <see cref="IReadOnlyRepository{TEntity, TKey}" /> wrapper for the current repository.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current repository.</returns>
        IReadOnlyRepositoryAsync<TEntity, TKey> AsReadOnlyAsync();

        /// <summary>
        /// Asynchronously deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        Task DeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken());
    }

    /// <summary>
    /// Represents a repository with a default primary key value of type integer.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepositoryAsync{TEntity}" />
    public interface IRepositoryAsync<TEntity> : IRepositoryAsync<TEntity, int>, IRepository<TEntity>, IReadOnlyRepositoryAsync<TEntity> where TEntity : class
    {
    }
}
