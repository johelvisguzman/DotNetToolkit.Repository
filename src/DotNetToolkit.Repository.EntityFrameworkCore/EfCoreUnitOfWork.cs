namespace DotNetToolkit.Repository.EntityFrameworkCore
{
    using Interceptors;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using System;
    using System.Collections.Generic;
    using Transactions;

    /// <summary>
    /// Represents a unit of work for entity framework core.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Transactions.UnitOfWorkBaseAsync" />
    public class EfCoreUnitOfWork : UnitOfWorkBaseAsync
    {
        #region Fields

        private bool _disposed;
        private IDbContextTransaction _transaction;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreUnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="interceptors">The interceptors.</param>
        public EfCoreUnitOfWork(DbContext context, IEnumerable<IRepositoryInterceptor> interceptors = null)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _transaction = context.Database.BeginTransaction();
            Factory = new EfCoreRepositoryFactory(() => context, interceptors);
        }

        #endregion

        #region Overrides of UnitOfWorkBase

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_transaction != null)
                {
                    _transaction.Rollback();
                    _transaction.Dispose();
                    _transaction = null;
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// Commits all changes made in this unit of work.
        /// </summary>
        public override void Commit()
        {
            if (_transaction == null)
                throw new InvalidOperationException("The transaction has already been committed or disposed.");

            _transaction.Commit();
            _transaction = null;
        }

        #endregion
    }
}
