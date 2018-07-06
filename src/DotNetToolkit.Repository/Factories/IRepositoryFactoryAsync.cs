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

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and a composite primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
        /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        IRepositoryAsync<TEntity, TKey1, TKey2> CreateAsync<TEntity, TKey1, TKey2>() where TEntity : class;

        /// <summary>
        /// Creates a new asynchronous repository for the specified entity and a composite primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
        /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
        /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
        /// <returns>The new asynchronous repository.</returns>
        IRepositoryAsync<TEntity, TKey1, TKey2, TKey3> CreateAsync<TEntity, TKey1, TKey2, TKey3>() where TEntity : class;
    }
}
