namespace DotNetToolkit.Repository.Json
{
    using Interceptors;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a repository for storing entities as an json formatted file.
    /// </summary>
    public class JsonRepository<TEntity, TKey> : JsonRepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public JsonRepository(string path) : base(path) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptor">The interceptor.</param>
        public JsonRepository(string path, IRepositoryInterceptor interceptor) : this(path, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptors">The interceptors.</param>
        public JsonRepository(string path, IEnumerable<IRepositoryInterceptor> interceptors) : base(path, interceptors) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for storing entities as an json formatted file with a default primary key value of type integer (for testing purposes).
    /// </summary>
    public class JsonRepository<TEntity> : JsonRepositoryBase<TEntity, int>, IRepository<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public JsonRepository(string path) : base(path) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptor">The interceptor.</param>
        public JsonRepository(string path, IRepositoryInterceptor interceptor) : this(path, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptors">The interceptors.</param>
        public JsonRepository(string path, IEnumerable<IRepositoryInterceptor> interceptors) : base(path, interceptors) { }

        #endregion
    }
}
