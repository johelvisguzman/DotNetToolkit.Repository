namespace DotNetToolkit.Repository.EntityFrameworkCore
{
    using Interceptors;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using Transactions;

    /// <summary>
    /// Represents a unit of work for entity framework core.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Transactions.UnitOfWorkBaseAsync" />
    public class EfCoreUnitOfWork : UnitOfWorkBaseAsync
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public EfCoreUnitOfWork(DbContext context) : base(new EfCoreTransactionManager(context))
        {
            Factory = new EfCoreRepositoryFactory(() => context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="interceptor">The interceptor.</param>
        public EfCoreUnitOfWork(DbContext context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public EfCoreUnitOfWork(DbContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(new EfCoreTransactionManager(context))
        {
            Factory = new EfCoreRepositoryFactory(() => context, interceptors);
        }

        #endregion
    }
}
