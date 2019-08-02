namespace DotNetToolkit.Repository.AzureStorageBlob.Internal
{
    using Configuration;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IRepositoryContextFactory" />.
    /// </summary>
    /// <seealso cref="IRepositoryContextFactory" />
    internal class AzureStorageBlobRepositoryContextFactory : IRepositoryContextFactory
    {
        #region Fields

        private readonly string _nameOrConnectionString;
        private readonly string _container;
        private readonly bool _createIfNotExists;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageBlobRepositoryContext" /> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="container">The name of the container.</param>
        /// <param name="createIfNotExists">Creates the container if it does not exist.</param>
        public AzureStorageBlobRepositoryContextFactory(string nameOrConnectionString, string container, bool createIfNotExists)
        {
            _nameOrConnectionString = Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));
            _container = Guard.NotEmpty(container, nameof(container));
            _createIfNotExists = createIfNotExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageBlobRepositoryContext" /> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="container">The name of the container.</param>
        public AzureStorageBlobRepositoryContextFactory(string nameOrConnectionString, string container)
        {
            _nameOrConnectionString = Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));
            _container = Guard.NotEmpty(container, nameof(container));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageBlobRepositoryContext" /> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="createIfNotExists">Creates the container if it does not exist.</param>
        public AzureStorageBlobRepositoryContextFactory(string nameOrConnectionString, bool createIfNotExists)
        {
            _nameOrConnectionString = Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));
            _createIfNotExists = createIfNotExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageBlobRepositoryContext" /> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        public AzureStorageBlobRepositoryContextFactory(string nameOrConnectionString)
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
            return new AzureStorageBlobRepositoryContext(_nameOrConnectionString, _container, _createIfNotExists);
        }

        #endregion
    }
}
