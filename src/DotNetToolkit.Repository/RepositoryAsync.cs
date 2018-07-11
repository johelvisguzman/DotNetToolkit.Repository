namespace DotNetToolkit.Repository
{
    using Interceptors;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="IRepositoryAsync{TEntity, TKey1, TKey2, TKey3}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepositoryAsync{TEntity, TKey1, TKey2}" />
    public class RepositoryAsync<TEntity, TKey1, TKey2, TKey3> : RepositoryBaseAsync<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryAsync{TEntity, TKey1, TKey2, TKey3}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        public RepositoryAsync(IRepositoryContextAsync context) : this(context, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryAsync{TEntity, TKey1, TKey2, TKey3}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public RepositoryAsync(IRepositoryContextAsync context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryAsync{TEntity, TKey1, TKey2, TKey3}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public RepositoryAsync(IRepositoryContextAsync context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IRepositoryAsync{TEntity, TKey1, TKey2}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepositoryAsync{TEntity, TKey1, TKey2}" />
    public class RepositoryAsync<TEntity, TKey1, TKey2> : RepositoryBaseAsync<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryAsync{TEntity, TKey1, TKey2}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        public RepositoryAsync(IRepositoryContextAsync context) : this(context, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryAsync{TEntity, TKey1, TKey2}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public RepositoryAsync(IRepositoryContextAsync context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryAsync{TEntity, TKey1, TKey2}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public RepositoryAsync(IRepositoryContextAsync context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IRepositoryAsync{TEntity, TKey}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepositoryAsync{TEntity, TKey}" />
    public class RepositoryAsync<TEntity, TKey> : RepositoryBaseAsync<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryAsync{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        public RepositoryAsync(IRepositoryContextAsync context) : this(context, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryAsync{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public RepositoryAsync(IRepositoryContextAsync context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryAsync{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public RepositoryAsync(IRepositoryContextAsync context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IRepositoryAsync{TEntity}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepositoryAsync{TEntity}" />
    public class RepositoryAsync<TEntity> : RepositoryBaseAsync<TEntity, int>, IRepositoryAsync<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryAsync{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        public RepositoryAsync(IRepositoryContextAsync context) : this(context, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryAsync{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public RepositoryAsync(IRepositoryContextAsync context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryAsync{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public RepositoryAsync(IRepositoryContextAsync context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion
    }
}
