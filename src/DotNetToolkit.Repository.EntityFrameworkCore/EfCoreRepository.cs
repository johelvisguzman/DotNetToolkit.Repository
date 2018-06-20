namespace DotNetToolkit.Repository.EntityFrameworkCore
{
    using Interceptors;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a repository for entity framework core.
    /// </summary>
    public class EfCoreRepository<TEntity, TKey> : EfCoreRepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public EfCoreRepository(DbContext context) : base(context) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public EfCoreRepository(DbContext context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public EfCoreRepository(DbContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for entity framework core with a default primary key value of type integer.
    /// </summary>
    public class EfCoreRepository<TEntity> : EfCoreRepositoryBase<TEntity, int>, IRepositoryAsync<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public EfCoreRepository(DbContext context) : base(context) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public EfCoreRepository(DbContext context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public EfCoreRepository(DbContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion
    }
}
