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
        private readonly IAzureStorageBlobContainerNameBuilder _containerNameBuilder;
        private readonly bool _createIfNotExists;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageBlobRepositoryContext" /> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="containerNameBuilder">The name of the container builder.</param>
        /// <param name="createIfNotExists">Creates the container if it does not exist.</param>
        public AzureStorageBlobRepositoryContextFactory(string nameOrConnectionString, IAzureStorageBlobContainerNameBuilder containerNameBuilder, bool createIfNotExists)
        {
            _nameOrConnectionString = Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));
            _containerNameBuilder = Guard.NotNull(containerNameBuilder, nameof(containerNameBuilder));
            _createIfNotExists = createIfNotExists;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureStorageBlobRepositoryContext" /> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <param name="containerNameBuilder">The name of the container builder.</param>
        public AzureStorageBlobRepositoryContextFactory(string nameOrConnectionString, IAzureStorageBlobContainerNameBuilder containerNameBuilder)
        {
            _nameOrConnectionString = Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));
            _containerNameBuilder = Guard.NotNull(containerNameBuilder, nameof(containerNameBuilder));
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
            return new AzureStorageBlobRepositoryContext(_nameOrConnectionString, _containerNameBuilder, _createIfNotExists);
        }

        #endregion
    }
}
