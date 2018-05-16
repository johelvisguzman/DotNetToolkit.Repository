namespace DotNetToolkit.Repository
{
    using System;
    using Traits;

    /// <summary>
    /// Represents a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    public interface IRepository<TEntity, in TKey> : ICanQuery<TEntity>, ICanAggregate<TEntity>, ICanAdd<TEntity>, ICanUpdate<TEntity>, ICanDelete<TEntity, TKey>, ICanGet<TEntity, TKey>, ICanFind<TEntity>, IDisposable
        where TEntity : class
    {
    }

    /// <summary>
    /// Represents a repository with a default primary key value of type integer.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class
    {
    }
}
