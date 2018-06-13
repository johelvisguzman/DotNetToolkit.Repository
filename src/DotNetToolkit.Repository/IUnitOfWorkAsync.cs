namespace DotNetToolkit.Repository
{
    /// <summary>
    /// Maintains business objects in-memory which have been changed (inserted, updated, or deleted) in a single transaction.
    /// Once the transaction is completed, the changes will be sent to the database in one single asynchronous unit of work.
    /// </summary>
    /// <see cref="DotNetToolkit.Repository.IUnitOfWork"/>
    public interface IUnitOfWorkAsync : IUnitOfWork
    {
        /// <summary>
        /// Gets a new asynchronous repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>A new asynchronous repository for the specified entity type.</returns>
        IRepositoryAsync<TEntity> GetRepositoryAsync<TEntity>() where TEntity : class;

        /// <summary>
        /// Gets a new asynchronous repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>A new asynchronous repository for the specified entity and primary key type.</returns>
        IRepositoryAsync<TEntity, TKey> GetRepositoryAsync<TEntity, TKey>() where TEntity : class;
    }
}
