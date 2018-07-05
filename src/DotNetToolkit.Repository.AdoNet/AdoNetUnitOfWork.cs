namespace DotNetToolkit.Repository.AdoNet
{
    using Interceptors;
    using System.Collections.Generic;
    using Transactions;

    /// <summary>
    /// Represents a unit of work for ado.net.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Transactions.UnitOfWorkBaseAsync" />
    public class AdoNetUnitOfWork : UnitOfWorkBaseAsync
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public AdoNetUnitOfWork(AdoNetContext context) : base(new AdoNetTransactionManager(context), new AdoNetRepositoryFactory(() => context)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public AdoNetUnitOfWork(AdoNetContext context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetUnitOfWork(AdoNetContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(new AdoNetTransactionManager(context), new AdoNetRepositoryFactory(() => context, interceptors)) {  }

        #endregion
    }
}
