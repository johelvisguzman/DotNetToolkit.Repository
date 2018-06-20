﻿namespace DotNetToolkit.Repository.EntityFramework
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
        /// Initializes a new instance of the <see cref="EfTransactionManager"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public EfTransactionManager(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Transaction = context.Database.BeginTransaction();
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
        /// Commits all changes made to the database in the current transaction.
        /// </summary>
        public void Commit()
        {
            Transaction.Commit();
        }

        /// <summary>
        /// Discards all changes made to the database in the current transaction.
        /// </summary>
        public void Rollback()
        {
            Transaction.Rollback();
        }

        #endregion
    }
}