namespace DotNetToolkit.Repository.Xml.Internal
{
    using Configuration;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IRepositoryContextFactory" />.
    /// </summary>
    /// <seealso cref="IRepositoryContextFactory" />
    internal class XmlRepositoryContextFactory : IRepositoryContextFactory
    {
        #region Fields

        private readonly string _path;
        private readonly bool _ignoreTransactionWarning;
        private readonly bool _ignoreSqlQueryWarning;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepositoryContextFactory" /> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public XmlRepositoryContextFactory(string path) : this(path, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepositoryContextFactory" /> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the context provider does not support transactions.</param>
        public XmlRepositoryContextFactory(string path, bool ignoreTransactionWarning) : this(path, ignoreTransactionWarning, false) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepositoryContextFactory" /> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the context provider does not support transactions.</param>
        /// <param name="ignoreSqlQueryWarning">If a SQL query is executed, ignore any warnings since the in-memory provider does not support SQL query execution.</param>
        public XmlRepositoryContextFactory(string path, bool ignoreTransactionWarning, bool ignoreSqlQueryWarning)
        {
            _path = Guard.NotEmpty(path, nameof(path));
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
            return new XmlRepositoryContext(_path, _ignoreTransactionWarning, _ignoreSqlQueryWarning);
        }

        #endregion
    }
}
