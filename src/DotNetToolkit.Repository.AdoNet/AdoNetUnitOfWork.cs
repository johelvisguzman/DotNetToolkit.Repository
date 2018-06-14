namespace DotNetToolkit.Repository.AdoNet
{
    using Interceptors;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Common;
    using Transactions;


    /// <summary>
    /// Represents a unit of work for ado.net.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Transactions.UnitOfWorkBaseAsync" />
    public class AdoNetUnitOfWork : UnitOfWorkBaseAsync
    {
        #region Fields

        private bool _disposed;
        private DbTransaction _transaction;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWork"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetUnitOfWork(string connectionString, IEnumerable<IRepositoryInterceptor> interceptors = null)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            var ccs = ConfigurationManager.ConnectionStrings[connectionString];
            if (ccs == null)
                throw new ArgumentException(Resources.ConnectionStringDoestNotExistInConfigFile);

            _transaction = BeginTransaction(ccs.ProviderName, connectionString);
            Factory = new AdoNetRepositoryFactory(_transaction, interceptors);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWork"/> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetUnitOfWork(string providerName, string connectionString, IEnumerable<IRepositoryInterceptor> interceptors = null)
        {
            if (providerName == null)
                throw new ArgumentNullException(nameof(providerName));

            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            _transaction = BeginTransaction(providerName, connectionString);
            Factory = new AdoNetRepositoryFactory(_transaction, interceptors);
        }

        #endregion

        #region Private Methods

        private DbTransaction BeginTransaction(string providerName, string connectionString)
        {
            if (providerName == null)
                throw new ArgumentNullException(nameof(providerName));

            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            var dbProviderFactory = Internal.DbProviderFactories.GetFactory(providerName);
            var connection = dbProviderFactory.CreateConnection();

            connection.ConnectionString = connectionString;
            connection.Open();

            return connection.BeginTransaction();
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
