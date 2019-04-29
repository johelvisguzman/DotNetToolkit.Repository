namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Configuration.Logging;
    using System.Data.Common;
    using Transactions;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="ITransactionManager" />.
    /// </summary>
    /// <seealso cref="ITransactionManager" />
    internal class AdoNetTransactionManager : ITransactionManager
    {
        #region Fields

        private readonly ILogger _logger;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the underlying transaction.
        /// </summary>
        public DbTransaction Transaction { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetTransactionManager" /> class.
        /// </summary>
        /// <param name="transaction">The underlying transaction.</param>
        /// <param name="logger">The logger.</param>
        public AdoNetTransactionManager(DbTransaction transaction, ILogger logger)
        {
            _logger = Guard.NotNull(logger);
            _logger.Debug("Started transaction");

            Transaction = Guard.NotNull(transaction);
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Transaction.Dispose();

            _logger.Debug("Disposed transaction");
        }

        #endregion

        #region Implementation of ITransaction

        /// <summary>
        /// The transaction status.
        /// </summary>
        public TransactionStatus Status { get; private set; }

        /// <summary>
        /// Commits all changes made to the database in the current transaction.
        /// </summary>
        public void Commit()
        {
            Transaction.Commit();

            Status = TransactionStatus.Committed;

            _logger.Debug("Committed transaction");
        }

        /// <summary>
        /// Discards all changes made to the database in the current transaction.
        /// </summary>
        public void Rollback()
        {
            Transaction.Rollback();

            Status = TransactionStatus.Aborted;

            _logger.Debug("Rolled-back transaction");
        }

        #endregion
    }
}
