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
        /// <param name="filePath">The file path.</param>
        public JsonRepository(string filePath) : base(filePath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="interceptors">The interceptors.</param>
        public JsonRepository(string filePath, IEnumerable<IRepositoryInterceptor> interceptors) : base(filePath, interceptors)
        {
        }

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
        /// <param name="filePath">The file path.</param>
        public JsonRepository(string filePath) : base(filePath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="interceptors">The interceptors.</param>
        public JsonRepository(string filePath, IEnumerable<IRepositoryInterceptor> interceptors) : base(filePath, interceptors)
        {
        }

        #endregion
    }
}
