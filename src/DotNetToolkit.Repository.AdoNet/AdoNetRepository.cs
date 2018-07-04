namespace DotNetToolkit.Repository.AdoNet
{
    using Interceptors;
    using System.Collections.Generic;
    using System.Data.Common;

    /// <summary>
    /// Represents a repository for entity framework.
    /// </summary>
    public class AdoNetRepository<TEntity, TKey> : AdoNetRepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public AdoNetRepository(AdoNetContext context) : base(context) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public AdoNetRepository(AdoNetContext context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetRepository(AdoNetContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for entity framework with a default primary key value of type integer.
    /// </summary>
    public class AdoNetRepository<TEntity> : AdoNetRepositoryBase<TEntity, int>, IRepositoryAsync<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public AdoNetRepository(AdoNetContext context) : base(context) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public AdoNetRepository(AdoNetContext context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetRepository(AdoNetContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion
    }
}
