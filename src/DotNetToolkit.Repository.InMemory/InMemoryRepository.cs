namespace DotNetToolkit.Repository.InMemory
{

    /// <summary>
    /// Represents a repository for in-memory operations (for testing purposes).
    /// </summary>
    public class InMemoryRepository<TEntity, TKey> : InMemoryRepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity,TKey}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        public InMemoryRepository(string databaseName = null) : base(databaseName)
        {
        }

        #endregion
    }

    /// <summary>
    /// Represents a repository for in-memory operations with a default primary key value of type integer (for testing purposes).
    /// </summary>
    public class InMemoryRepository<TEntity> : InMemoryRepositoryBase<TEntity, int>, IRepository<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        public InMemoryRepository(string databaseName = null) : base(databaseName)
        {
        }

        #endregion
    }
}
