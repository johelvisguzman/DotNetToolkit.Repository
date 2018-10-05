namespace DotNetToolkit.Repository.Factories
{
    using Configuration;
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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryContextFactory"/> class.
        /// </summary>
        public InMemoryRepositoryContextFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryContextFactory"/> class.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        public InMemoryRepositoryContextFactory(string databaseName)
        {
            if (databaseName == null)
                throw new ArgumentNullException(nameof(databaseName));

            _databaseName = databaseName;
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
                ? new InMemoryRepositoryContext(_databaseName)
                : new InMemoryRepositoryContext();
        }

        #endregion
    }
}
