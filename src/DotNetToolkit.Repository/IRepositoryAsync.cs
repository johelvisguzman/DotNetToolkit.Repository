namespace DotNetToolkit.Repository
{
    using System;
    using Traits;

    /// <summary>
    /// Represents an asynchronous repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    public interface IRepositoryAsync<TEntity, in TKey> : IRepository<TEntity, TKey>, ICanAggregateAsync<TEntity>, ICanAddAsync<TEntity>, ICanUpdateAsync<TEntity>, ICanDeleteAsync<TEntity, TKey>, ICanGetAsync<TEntity, TKey>, ICanFindAsync<TEntity>, IDisposable
        where TEntity : class
    {
    }

    /// <summary>
    /// Represents an asynchronous repository with a default primary key value of type integer.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepositoryAsync<TEntity> : IRepositoryAsync<TEntity, int> where TEntity : class
    {
    }
}
