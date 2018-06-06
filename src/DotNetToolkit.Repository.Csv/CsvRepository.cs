namespace DotNetToolkit.Repository.Csv
{
    using Interceptors;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a repository for storing entities as an csv formatted file.
    /// </summary>
    public class CsvRepository<TEntity, TKey> : CsvRepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public CsvRepository(string filePath) : base(filePath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="interceptors">The interceptors.</param>
        public CsvRepository(string filePath, IEnumerable<IRepositoryInterceptor> interceptors) : base(filePath, interceptors)
        {
        }

        #endregion
    }

    /// <summary>
    /// Represents a repository for storing entities as an csv formatted file with a default primary key value of type integer (for testing purposes).
    /// </summary>
    public class CsvRepository<TEntity> : CsvRepositoryBase<TEntity, int>, IRepository<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public CsvRepository(string filePath) : base(filePath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="interceptors">The interceptors.</param>
        public CsvRepository(string filePath, IEnumerable<IRepositoryInterceptor> interceptors) : base(filePath, interceptors)
        {
        }

        #endregion
    }
}
