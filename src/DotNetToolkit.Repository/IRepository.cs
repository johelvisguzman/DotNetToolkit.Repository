namespace DotNetToolkit.Repository
{
    using Traits;

    /// <summary>
    /// Represents a repository.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the primary key.</typeparam>
    public interface IRepository<TEntity, in TKey> : ICanAggregate<TEntity>, ICanAdd<TEntity>, ICanUpdate<TEntity>, ICanDelete<TEntity, TKey>, ICanGet<TEntity, TKey>, ICanFind<TEntity>
        where TEntity : class
    {
    }
}
