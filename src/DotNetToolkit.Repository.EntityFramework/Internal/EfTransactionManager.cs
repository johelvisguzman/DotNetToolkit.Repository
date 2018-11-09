namespace DotNetToolkit.Repository.EntityFramework.Internal
{
    using System;
    using System.Data.Entity;
    using Transactions;

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
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            Transaction = transaction;
        }

        #endregion

        #region Implementation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Transaction.Dispose();
        }

        #endregion

        #region Implementation of ITransactionManager

        /// <inheritdoc />
        public void Commit()
        {
            Transaction.Commit();
        }

        /// <inheritdoc />
        public void Rollback()
        {
            Transaction.Rollback();
        }

        #endregion
    }
}
