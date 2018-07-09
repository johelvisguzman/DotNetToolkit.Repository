namespace DotNetToolkit.Repository.Csv
{
    using Interceptors;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a repository for storing entities as an csv formatted file with a composite primary key.
    /// </summary>
    public class CsvRepository<TEntity, TKey1, TKey2, TKey3> : RepositoryBase<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public CsvRepository(string path) : this(path, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptor">The interceptor.</param>
        public CsvRepository(string path, IRepositoryInterceptor interceptor) : this(path, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptors">The interceptors.</param>
        public CsvRepository(string path, IEnumerable<IRepositoryInterceptor> interceptors) : base(new CsvContext<TEntity>(path), interceptors) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for storing entities as an csv formatted file with a composite primary key.
    /// </summary>
    public class CsvRepository<TEntity, TKey1, TKey2> : RepositoryBase<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public CsvRepository(string path) : this(path, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptor">The interceptor.</param>
        public CsvRepository(string path, IRepositoryInterceptor interceptor) : this(path, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptors">The interceptors.</param>
        public CsvRepository(string path, IEnumerable<IRepositoryInterceptor> interceptors) : base(new CsvContext<TEntity>(path), interceptors) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for storing entities as an csv formatted file.
    /// </summary>
    public class CsvRepository<TEntity, TKey> : RepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public CsvRepository(string path) : this(path, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptor">The interceptor.</param>
        public CsvRepository(string path, IRepositoryInterceptor interceptor) : this(path, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptors">The interceptors.</param>
        public CsvRepository(string path, IEnumerable<IRepositoryInterceptor> interceptors) : base(new CsvContext<TEntity>(path), interceptors) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for storing entities as an csv formatted file with a default primary key value of type integer (for testing purposes).
    /// </summary>
    public class CsvRepository<TEntity> : RepositoryBase<TEntity, int>, IRepository<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public CsvRepository(string path) : this(path, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptor">The interceptor.</param>
        public CsvRepository(string path, IRepositoryInterceptor interceptor) : this(path, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptors">The interceptors.</param>
        public CsvRepository(string path, IEnumerable<IRepositoryInterceptor> interceptors) : base(new CsvContext<TEntity>(path), interceptors) { }

        #endregion
    }
}
