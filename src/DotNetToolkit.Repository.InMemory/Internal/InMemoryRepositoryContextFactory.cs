namespace DotNetToolkit.Repository.InMemory.Internal
{
    using Configuration;
    using Factories;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IRepositoryContextFactory" />.
    /// </summary>
    /// <seealso cref="IRepositoryContextFactory" />
    internal class InMemoryRepositoryContextFactory : IRepositoryContextFactory
    {
        #region Fields

        private readonly string _databaseName;
        private readonly bool _ignoreTransactionWarning;
        private readonly bool _ignoreSqlQueryWarning;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryContextFactory" /> class.
        /// </summary>
        public InMemoryRepositoryContextFactory() : this(false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryContextFactory" /> class.
        /// </summary>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the in-memory provider does not support transactions.</param>
        public InMemoryRepositoryContextFactory(bool ignoreTransactionWarning) : this(ignoreTransactionWarning, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryContextFactory" /> class.
        /// </summary>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the in-memory provider does not support transactions.</param>
        /// <param name="ignoreSqlQueryWarning">If a SQL query is executed, ignore any warnings since the in-memory provider does not support SQL query execution.</param>
        public InMemoryRepositoryContextFactory(bool ignoreTransactionWarning, bool ignoreSqlQueryWarning)
        {
            _ignoreTransactionWarning = ignoreTransactionWarning;
            _ignoreSqlQueryWarning = ignoreSqlQueryWarning;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryContextFactory" /> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        public InMemoryRepositoryContextFactory(string databaseName) : this(databaseName, false, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryContextFactory" /> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the in-memory provider does not support transactions.</param>
        public InMemoryRepositoryContextFactory(string databaseName, bool ignoreTransactionWarning) : this(databaseName, ignoreTransactionWarning, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryContextFactory" /> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the in-memory provider does not support transactions.</param>
        /// <param name="ignoreSqlQueryWarning">If a SQL query is executed, ignore any warnings since the in-memory provider does not support SQL query execution.</param>
        public InMemoryRepositoryContextFactory(string databaseName, bool ignoreTransactionWarning, bool ignoreSqlQueryWarning)
        {
            _databaseName = Guard.NotEmpty(databaseName);
            _ignoreTransactionWarning = ignoreTransactionWarning;
            _ignoreSqlQueryWarning = ignoreSqlQueryWarning;
        }

        #endregion

        #region Implementation of IRepositoryContextFactory

        /// <summary>
        /// Create a new repository context.
        /// </summary>
        /// <returns>The new repository context.</returns>
        public IRepositoryContext Create()
        {
            return !string.IsNullOrEmpty(_databaseName)
                ? new InMemoryRepositoryContext(_databaseName, _ignoreTransactionWarning, _ignoreSqlQueryWarning)
                : new InMemoryRepositoryContext(_ignoreTransactionWarning, _ignoreSqlQueryWarning);
        }

        #endregion
    }
}
