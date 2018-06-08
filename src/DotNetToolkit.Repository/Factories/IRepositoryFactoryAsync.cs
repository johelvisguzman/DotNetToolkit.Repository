namespace DotNetToolkit.Repository.Factories
{
    /// <summary>
    /// Represents an asynchronous repository factory.
    /// </summary>
    /// <seealso cref="IRepositoryFactory" />
    public interface IRepositoryFactoryAsync : IRepositoryFactory
    {
        /// <summary>
        /// Creates a new asynchronous repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        IRepositoryAsync<TEntity> CreateAsync<TEntity>() where TEntity : class;

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        IRepositoryAsync<TEntity, TKey> CreateAsync<TEntity, TKey>() where TEntity : class;
    }
}
