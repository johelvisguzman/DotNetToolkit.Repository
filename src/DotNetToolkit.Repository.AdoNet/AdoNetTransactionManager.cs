namespace DotNetToolkit.Repository.AdoNet
{
    using Properties;
    using System;
    using System.Configuration;
    using System.Data.Common;
    using Transactions;

    /// <summary>
    /// An implementation of <see cref="ITransactionManager" />.
    /// </summary>
    /// <seealso cref="ITransactionManager" />
    internal class AdoNetTransactionManager : ITransactionManager
    {
        #region Properties

        /// <summary>
        /// Gets the underlying transaction.
        /// </summary>
        public DbTransaction Transaction { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetTransactionManager"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetTransactionManager(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            var ccs = ConfigurationManager.ConnectionStrings[connectionString];
            if (ccs == null)
                throw new ArgumentException(Resources.ConnectionStringDoestNotExistInConfigFile);

            Transaction = BeginTransaction(ccs.ProviderName, connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetTransactionManager"/> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetTransactionManager(string providerName, string connectionString)
        {
            if (providerName == null)
                throw new ArgumentNullException(nameof(providerName));

            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            Transaction = BeginTransaction(providerName, connectionString);
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

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Transaction.Dispose();
        }

        #endregion

        #region Implementation of ITransaction

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
