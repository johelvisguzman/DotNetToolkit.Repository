namespace DotNetToolkit.Repository
{
    using System;
    using Traits;

    /// <summary>
    /// Represents an asynchronous repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepository{TEntity, TKey}" />
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanAggregateAsync{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanAddAsync{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanUpdateAsync{TEntity}" />
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanDeleteAsync{TEntity, TKey}" />
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanGetAsync{TEntity, TKey}" />
    /// <seealso cref="DotNetToolkit.Repository.Traits.ICanFindAsync{TEntity}" />
    /// <seealso cref="System.IDisposable" />
    public interface IRepositoryAsync<TEntity, in TKey> : IRepository<TEntity, TKey>, ICanAggregateAsync<TEntity>, ICanAddAsync<TEntity>, ICanUpdateAsync<TEntity>, ICanDeleteAsync<TEntity, TKey>, ICanGetAsync<TEntity, TKey>, ICanFindAsync<TEntity>, IDisposable
        where TEntity : class
    {
    }

    /// <summary>
    /// Represents an asynchronous repository with a default primary key value of type integer.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepositoryAsync{TEntity, TKey}" />
    /// <seealso cref="DotNetToolkit.Repository.IRepository{TEntity}" />
    public interface IRepositoryAsync<TEntity> : IRepositoryAsync<TEntity, int>, IRepository<TEntity> where TEntity : class
    {
    }
}
