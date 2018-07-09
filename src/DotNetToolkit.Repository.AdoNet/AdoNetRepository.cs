namespace DotNetToolkit.Repository.AdoNet
{
    using Interceptors;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a repository for ado.net with a composite primary key.
    /// </summary>
    public class AdoNetRepository<TEntity, TKey1, TKey2, TKey3> : RepositoryBaseAsync<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey1, TKey2, TKey3}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public AdoNetRepository(AdoNetContext context) : base(context) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey1, TKey2, TKey3}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public AdoNetRepository(AdoNetContext context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey1, TKey2, TKey3}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetRepository(AdoNetContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for ado.net with a composite primary key.
    /// </summary>
    public class AdoNetRepository<TEntity, TKey1, TKey2> : RepositoryBaseAsync<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey1, TKey2}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public AdoNetRepository(AdoNetContext context) : base(context) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey1, TKey2}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public AdoNetRepository(AdoNetContext context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey1, TKey2}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetRepository(AdoNetContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for ado.net.
    /// </summary>
    public class AdoNetRepository<TEntity, TKey> : RepositoryBaseAsync<TEntity, TKey> where TEntity : class
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
    /// Represents a repository for ado.net with a default primary key value of type integer.
    /// </summary>
    public class AdoNetRepository<TEntity> : RepositoryBaseAsync<TEntity, int>, IRepositoryAsync<TEntity> where TEntity : class
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
