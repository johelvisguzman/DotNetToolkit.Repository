namespace DotNetToolkit.Repository.EntityFrameworkCore.Internal
{
    using Microsoft.EntityFrameworkCore.Storage;
    using System;
    using Transactions;

    /// <summary>
    /// An implementation of <see cref="ITransactionManager" />.
    /// </summary>
    /// <seealso cref="ITransactionManager" />
    internal class EfCoreTransactionManager : ITransactionManager
    {
        #region Properties

        /// <summary>
        /// Gets the underlying transaction.
        /// </summary>
        public IDbContextTransaction Transaction { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreTransactionManager" /> class.
        /// </summary>
        /// <param name="transaction">The underlying transaction.</param>
        public EfCoreTransactionManager(IDbContextTransaction transaction)
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
