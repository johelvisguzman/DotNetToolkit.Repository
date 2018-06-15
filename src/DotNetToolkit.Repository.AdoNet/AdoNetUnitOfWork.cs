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
        /// <param name="connectionString">The connection string.</param>
        public AdoNetUnitOfWork(string connectionString) : base(new AdoNetTransactionManager(connectionString))
        {
            Factory = new AdoNetRepositoryFactory(((AdoNetTransactionManager)TransactionManager).Transaction);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWork"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetUnitOfWork(string connectionString, IEnumerable<IRepositoryInterceptor> interceptors) : base(new AdoNetTransactionManager(connectionString))
        {
            Factory = new AdoNetRepositoryFactory(((AdoNetTransactionManager)TransactionManager).Transaction, interceptors);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWork"/> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetUnitOfWork(string providerName, string connectionString) : base(new AdoNetTransactionManager(providerName, connectionString))
        {
            Factory = new AdoNetRepositoryFactory(((AdoNetTransactionManager)TransactionManager).Transaction);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWork"/> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetUnitOfWork(string providerName, string connectionString, IEnumerable<IRepositoryInterceptor> interceptors) : base(new AdoNetTransactionManager(providerName, connectionString))
        {
            Factory = new AdoNetRepositoryFactory(((AdoNetTransactionManager)TransactionManager).Transaction, interceptors);
        }

        #endregion
    }
}
