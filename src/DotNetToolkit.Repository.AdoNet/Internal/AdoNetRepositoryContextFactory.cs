namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Configuration;
    using System.Data.Common;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IRepositoryContextFactory" />.
    /// </summary>
    /// <seealso cref="IRepositoryContextFactory" />
    internal class AdoNetRepositoryContextFactory : IRepositoryContextFactory
    {
        #region Fields

        private readonly string _nameOrConnectionString;
        private readonly string _providerName;
        private readonly DbConnection _existingConnection;
        private readonly bool _ensureDatabaseCreated;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContextFactory"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        public AdoNetRepositoryContextFactory(string nameOrConnectionString) : this(nameOrConnectionString, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContextFactory"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="ensureDatabaseCreated">
        /// Ensures that the database for the context exists. If it exists, no action is taken.
        /// If it does not exist then the database and all its schema are created.
        /// If the database exists, then no effort is made to ensure it is compatible with the model for this context.
        /// </param>
        public AdoNetRepositoryContextFactory(string nameOrConnectionString, bool ensureDatabaseCreated)
        {
            _nameOrConnectionString = Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));
            _ensureDatabaseCreated = ensureDatabaseCreated;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContextFactory"/> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetRepositoryContextFactory(string providerName, string connectionString) : this(providerName, connectionString, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContextFactory"/> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="ensureDatabaseCreated">
        /// Ensures that the database for the context exists. If it exists, no action is taken.
        /// If it does not exist then the database and all its schema are created.
        /// If the database exists, then no effort is made to ensure it is compatible with the model for this context.
        /// </param>
        public AdoNetRepositoryContextFactory(string providerName, string connectionString, bool ensureDatabaseCreated)
        {
            _providerName = Guard.NotEmpty(providerName, nameof(providerName));
            _nameOrConnectionString = Guard.NotEmpty(connectionString, nameof(connectionString));
            _ensureDatabaseCreated = ensureDatabaseCreated;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContextFactory" /> class.
        /// </summary>
        /// <param name="existingConnection">The existing connection.</param>
        public AdoNetRepositoryContextFactory(DbConnection existingConnection) : this(existingConnection, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContextFactory" /> class.
        /// </summary>
        /// <param name="existingConnection">The existing connection.</param>
        /// <param name="ensureDatabaseCreated">
        /// Ensures that the database for the context exists. If it exists, no action is taken.
        /// If it does not exist then the database and all its schema are created.
        /// If the database exists, then no effort is made to ensure it is compatible with the model for this context.
        /// </param>
        public AdoNetRepositoryContextFactory(DbConnection existingConnection, bool ensureDatabaseCreated)
        {
            _existingConnection = Guard.NotNull(existingConnection, nameof(existingConnection));
            _ensureDatabaseCreated = ensureDatabaseCreated;
        }

        #endregion

        #region Implementation of IRepositoryContextFactory

        /// <summary>
        /// Create a new repository context.
        /// </summary>
        /// <returns>The new repository context.</returns>
        public IRepositoryContext Create()
        {
            if (_existingConnection != null)
                return new AdoNetRepositoryContext(_existingConnection, _ensureDatabaseCreated);

            if (!string.IsNullOrEmpty(_providerName))
                return new AdoNetRepositoryContext(_providerName, _nameOrConnectionString, _ensureDatabaseCreated);

            return new AdoNetRepositoryContext(_nameOrConnectionString, _ensureDatabaseCreated);
        }

        #endregion
    }
}
