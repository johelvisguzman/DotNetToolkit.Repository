namespace DotNetToolkit.Repository.EntityFrameworkCore
{
    using Interceptors;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using Transactions;

    /// <summary>
    /// Represents a unit of work for entity framework.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Transactions.UnitOfWorkBaseAsync" />
    public class EfCoreUnitOfWork : UnitOfWorkBaseAsync
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public EfCoreUnitOfWork(DbContext context, IEnumerable<IRepositoryInterceptor> interceptors = null) : base(new EfCoreTransactionManager(context))
        {
            Factory = new EfCoreRepositoryFactory(() => context, interceptors);
        }

        #endregion
    }
}
