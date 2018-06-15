namespace DotNetToolkit.Repository.Factories
{
    /// <summary>
    /// Represents a repository factory.
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Creates a new repository for the specified entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns>The new repository.</returns>
        IRepository<TEntity> Create<TEntity>() where TEntity : class;

        /// <summary>
        /// Creates a new repository for the specified entity and primary key type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TKey">The type of the key primary key value.</typeparam>
        /// <returns>The new repository.</returns>
        IRepository<TEntity, TKey> Create<TEntity, TKey>() where TEntity : class;

        /// <summary>
        /// Creates a new repository for the specified type.
        /// </summary>
        /// <returns>The new repository.</returns>
        T CreateInstance<T>() where T : class;
    }
}
