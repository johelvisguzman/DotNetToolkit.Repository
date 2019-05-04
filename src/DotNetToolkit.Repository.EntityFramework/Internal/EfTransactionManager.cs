namespace DotNetToolkit.Repository.EntityFramework.Internal
{
    using System.Data.Entity;
    using Transactions;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="ITransactionManager" />.
    /// </summary>
    /// <seealso cref="ITransactionManager" />
    internal class EfTransactionManager : ITransactionManager
    {
        #region Properties

        /// <summary>
        /// Gets the underlying transaction.
        /// </summary>
        public DbContextTransaction Transaction { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfTransactionManager" /> class.
        /// </summary>
        /// <param name="transaction">The underlying transaction.</param>
        public EfTransactionManager(DbContextTransaction transaction)
        {
            Transaction = Guard.NotNull(transaction, nameof(transaction));
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Transaction.Dispose();
        }

        #endregion

        #region Implementation of ITransactionManager

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
        }

        /// <summary>
        /// Discards all changes made to the database in the current transaction.
        /// </summary>
        public void Rollback()
        {
            Transaction.Rollback();

            Status = TransactionStatus.Aborted;
        }

        #endregion
    }
}
