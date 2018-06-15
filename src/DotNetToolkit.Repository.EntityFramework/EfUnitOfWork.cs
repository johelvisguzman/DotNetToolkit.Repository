namespace DotNetToolkit.Repository.EntityFramework
{
    using Interceptors;
    using System.Collections.Generic;
    using System.Data.Entity;
    using Transactions;

    /// <summary>
    /// Represents a unit of work for entity framework.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Transactions.UnitOfWorkBaseAsync" />
    public class EfUnitOfWork : UnitOfWorkBaseAsync
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public EfUnitOfWork(DbContext context) : base(new EfTransactionManager(context))
        {
            Factory = new EfRepositoryFactory(() => context);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public EfUnitOfWork(DbContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(new EfTransactionManager(context))
        {
            Factory = new EfRepositoryFactory(() => context, interceptors);
        }

        #endregion
    }
}
