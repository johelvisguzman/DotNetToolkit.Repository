namespace DotNetToolkit.Repository.InMemory
{
    using Interceptors;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a repository for in-memory operations with a composite primary key (for testing purposes).
    /// </summary>
    public class InMemoryRepository<TEntity, TKey1, TKey2, TKey3> : RepositoryBase<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        public InMemoryRepository() : this(string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        public InMemoryRepository(string databaseName) : base(new InMemoryContext(databaseName)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        public InMemoryRepository(IRepositoryInterceptor interceptor) : this(new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="interceptors">The interceptors.</param>
        public InMemoryRepository(IEnumerable<IRepositoryInterceptor> interceptors) : base(new InMemoryContext(null), interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public InMemoryRepository(string databaseName, IRepositoryInterceptor interceptor) : this(databaseName, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public InMemoryRepository(string databaseName, IEnumerable<IRepositoryInterceptor> interceptors) : base(new InMemoryContext(databaseName), interceptors) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for in-memory operations with a composite primary key (for testing purposes).
    /// </summary>
    public class InMemoryRepository<TEntity, TKey1, TKey2> : RepositoryBase<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        public InMemoryRepository() : this(string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        public InMemoryRepository(string databaseName) : base(new InMemoryContext(databaseName)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        public InMemoryRepository(IRepositoryInterceptor interceptor) : this(new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="interceptors">The interceptors.</param>
        public InMemoryRepository(IEnumerable<IRepositoryInterceptor> interceptors) : base(new InMemoryContext(null), interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public InMemoryRepository(string databaseName, IRepositoryInterceptor interceptor) : this(databaseName, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public InMemoryRepository(string databaseName, IEnumerable<IRepositoryInterceptor> interceptors) : base(new InMemoryContext(databaseName), interceptors) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for in-memory operations (for testing purposes).
    /// </summary>
    public class InMemoryRepository<TEntity, TKey> : RepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey}"/> class.
        /// </summary>
        public InMemoryRepository() : this(string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        public InMemoryRepository(string databaseName) : base(new InMemoryContext(databaseName)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        public InMemoryRepository(IRepositoryInterceptor interceptor) : this(new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="interceptors">The interceptors.</param>
        public InMemoryRepository(IEnumerable<IRepositoryInterceptor> interceptors) : base(new InMemoryContext(null), interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public InMemoryRepository(string databaseName, IRepositoryInterceptor interceptor) : this(databaseName, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public InMemoryRepository(string databaseName, IEnumerable<IRepositoryInterceptor> interceptors) : base(new InMemoryContext(databaseName), interceptors) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for in-memory operations with a default primary key value of type integer (for testing purposes).
    /// </summary>
    public class InMemoryRepository<TEntity> : RepositoryBase<TEntity, int>, IRepository<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity}"/> class.
        /// </summary>
        public InMemoryRepository() : this(string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        public InMemoryRepository(string databaseName) : base(new InMemoryContext(databaseName)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        public InMemoryRepository(IRepositoryInterceptor interceptor) : this(new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="interceptors">The interceptors.</param>
        public InMemoryRepository(IEnumerable<IRepositoryInterceptor> interceptors) : base(new InMemoryContext(null), interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public InMemoryRepository(string databaseName, IRepositoryInterceptor interceptor) : this(databaseName, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public InMemoryRepository(string databaseName, IEnumerable<IRepositoryInterceptor> interceptors) : base(new InMemoryContext(databaseName), interceptors) { }

        #endregion
    }
}
