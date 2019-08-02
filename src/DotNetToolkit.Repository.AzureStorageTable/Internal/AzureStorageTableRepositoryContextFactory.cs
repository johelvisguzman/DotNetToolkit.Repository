namespace DotNetToolkit.Repository.AzureStorageTable.Internal
{
    using Configuration;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IRepositoryContextFactory" />.
    /// </summary>
    /// <seealso cref="IRepositoryContextFactory" />
    internal class AzureStorageTableRepositoryContextFactory : IRepositoryContextFactory
    {
        #region Fields

        private readonly string _nameOrConnectionString;
        private readonly string _tableName;
        private readonly bool _createIfNotExists;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageTableRepositoryContext" /> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="tableName">The name of the table.</param>
        /// <param name="createIfNotExists">Creates the table if it does not exist.</param>
        public AzureStorageTableRepositoryContextFactory(string nameOrConnectionString, string tableName, bool createIfNotExists)
        {
            _nameOrConnectionString = Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));
            _tableName = Guard.NotEmpty(tableName, nameof(tableName));
            _createIfNotExists = createIfNotExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageTableRepositoryContext" /> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="tableName">The name of the table.</param>
        public AzureStorageTableRepositoryContextFactory(string nameOrConnectionString, string tableName)
        {
            _nameOrConnectionString = Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));
            _tableName = Guard.NotEmpty(tableName, nameof(tableName));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageTableRepositoryContext" /> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="createIfNotExists">Creates the table if it does not exist.</param>
        public AzureStorageTableRepositoryContextFactory(string nameOrConnectionString, bool createIfNotExists)
        {
            _nameOrConnectionString = Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));
            _createIfNotExists = createIfNotExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageTableRepositoryContext" /> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        public AzureStorageTableRepositoryContextFactory(string nameOrConnectionString)
        {
            _nameOrConnectionString = Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));
        }

        #endregion

        #region Implementation of IRepositoryContextFactory

        /// <summary>
        /// Create a new repository context.
        /// </summary>
        /// <returns>The new repository context.</returns>
        public IRepositoryContext Create()
        {
            return new AzureStorageTableRepositoryContext(_nameOrConnectionString, _tableName, _createIfNotExists);
        }

        #endregion
    }
}
