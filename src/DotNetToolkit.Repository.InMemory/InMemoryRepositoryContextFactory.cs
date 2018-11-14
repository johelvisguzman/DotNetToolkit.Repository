namespace DotNetToolkit.Repository.InMemory
{
    using Configuration;
    using Factories;
    using Internal;
    using System;

    /// <summary>
    /// An implementation of <see cref="IRepositoryContextFactory" />.
    /// </summary>
    /// <seealso cref="IRepositoryContextFactory" />
    public class InMemoryRepositoryContextFactory : IRepositoryContextFactory
    {
        #region Fields

        private readonly string _databaseName;
        private readonly bool _ignoreTransactionWarning;

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
        public InMemoryRepositoryContextFactory(bool ignoreTransactionWarning)
        {
            _ignoreTransactionWarning = ignoreTransactionWarning;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryContextFactory" /> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        public InMemoryRepositoryContextFactory(string databaseName) : this(databaseName, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryContextFactory" /> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the in-memory provider does not support transactions.</param>
        public InMemoryRepositoryContextFactory(string databaseName, bool ignoreTransactionWarning)
        {
            if (databaseName == null)
                throw new ArgumentNullException(nameof(databaseName));

            _databaseName = databaseName;
            _ignoreTransactionWarning = ignoreTransactionWarning;
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
                ? new InMemoryRepositoryContext(_databaseName, _ignoreTransactionWarning)
                : new InMemoryRepositoryContext(_ignoreTransactionWarning);
        }

        #endregion
    }
}
