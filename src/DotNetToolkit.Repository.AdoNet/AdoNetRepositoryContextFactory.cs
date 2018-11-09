namespace DotNetToolkit.Repository.AdoNet
{
    using Configuration;
    using Factories;
    using Internal;
    using System;
    using System.Data;
    using System.Data.Common;

    /// <summary>
    /// An implementation of <see cref="IRepositoryContextFactory" />.
    /// </summary>
    /// <seealso cref="IRepositoryContextFactory" />
    public class AdoNetRepositoryContextFactory : IRepositoryContextFactory
    {
        #region Fields

        private readonly string _nameOrConnectionString;
        private readonly string _providerName;
        private readonly DbConnection _existingConnection;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContextFactory"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        public AdoNetRepositoryContextFactory(string nameOrConnectionString)
        {
            if (nameOrConnectionString == null)
                throw new ArgumentNullException(nameof(nameOrConnectionString));

            _nameOrConnectionString = nameOrConnectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContextFactory"/> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetRepositoryContextFactory(string providerName, string connectionString)
        {
            if (providerName == null)
                throw new ArgumentNullException(nameof(providerName));

            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            _providerName = providerName;
            _nameOrConnectionString = connectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContextFactory" /> class.
        /// </summary>
        /// <param name="existingConnection">The existing connection.</param>
        public AdoNetRepositoryContextFactory(DbConnection existingConnection)
        {
            if (existingConnection == null)
                throw new ArgumentNullException(nameof(existingConnection));

            if (existingConnection.State == ConnectionState.Closed)
                existingConnection.Open();

            _existingConnection = existingConnection;
        }

        #endregion

        #region Implementation of IRepositoryContextFactory

        /// <inheritdoc />
        public IRepositoryContext Create()
        {
            if (_existingConnection != null)
                return new AdoNetRepositoryContext(_existingConnection);

            if (!string.IsNullOrEmpty(_providerName))
                return new AdoNetRepositoryContext(_providerName, _nameOrConnectionString);

            return new AdoNetRepositoryContext(_nameOrConnectionString);
        }

        #endregion
    }
}
