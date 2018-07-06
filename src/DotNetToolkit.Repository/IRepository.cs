namespace DotNetToolkit.Repository
{
    using System;
    using Wrappers;

    /// <summary>
    /// Represents a repository with a composite primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepositoryBase{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepository{TEntity, TKey1, TKey2, TKey3}" />
    /// <seealso cref="System.IDisposable" />
    public interface IRepository<TEntity, in TKey1, in TKey2, in TKey3> : IRepositoryBase<TEntity>, IReadOnlyRepository<TEntity, TKey1, TKey2, TKey3>, IDisposable where TEntity : class
    {
        /// <summary>
        /// Returns a read-only <see cref="IReadOnlyRepository{TEntity, TKey1, TKey2, TKey3}" /> wrapper for the current repository.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current repository.</returns>
        IReadOnlyRepository<TEntity, TKey1, TKey2, TKey3> AsReadOnly();

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        void Delete(TKey1 key1, TKey2 key2, TKey3 key3);
    }

    /// <summary>
    /// Represents a repository with a composite primary key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepositoryBase{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepository{TEntity, TKey1, TKey2}" />
    /// <seealso cref="System.IDisposable" />
    public interface IRepository<TEntity, in TKey1, in TKey2> : IRepositoryBase<TEntity>, IReadOnlyRepository<TEntity, TKey1, TKey2>, IDisposable where TEntity : class
    {
        /// <summary>
        /// Returns a read-only <see cref="IReadOnlyRepository{TEntity, TKey1, TKey2}" /> wrapper for the current repository.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current repository.</returns>
        IReadOnlyRepository<TEntity, TKey1, TKey2> AsReadOnly();

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        void Delete(TKey1 key1, TKey2 key2);
    }

    /// <summary>
    /// Represents a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepositoryBase{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepository{TEntity, TKey}" />
    /// <seealso cref="System.IDisposable" />
    public interface IRepository<TEntity, in TKey> : IRepositoryBase<TEntity>, IReadOnlyRepository<TEntity, TKey>, IDisposable where TEntity : class
    {
        /// <summary>
        /// Returns a read-only <see cref="IReadOnlyRepository{TEntity, TKey}" /> wrapper for the current repository.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current repository.</returns>
        IReadOnlyRepository<TEntity, TKey> AsReadOnly();

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        void Delete(TKey key);
    }

    /// <summary>
    /// Represents a repository with a default primary key value of type integer.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.Wrappers.IReadOnlyRepository{TEntity}" />
    public interface IRepository<TEntity> : IRepository<TEntity, int>, IReadOnlyRepository<TEntity> where TEntity : class
    {
    }
}
