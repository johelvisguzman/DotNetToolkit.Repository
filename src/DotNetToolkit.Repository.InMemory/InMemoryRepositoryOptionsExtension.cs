namespace DotNetToolkit.Repository.InMemory
{
    using Configuration.Options;
    using Factories;
    using System;

    /// <summary>
    /// An implementation of <see cref="IRepositoryOptionsContextFactoryExtensions" />.
    /// </summary>
    public class InMemoryRepositoryOptionsExtension : IRepositoryOptionsContextFactoryExtensions
    {
        private string _databaseName;
        private InMemoryRepositoryContextFactory _contextFactory;

        /// <summary>
        /// Returns the extension instance with a named in-memory database store.
        /// </summary>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <returns>The same extension instance.</returns>
        public InMemoryRepositoryOptionsExtension WithDatabaseName(string databaseName)
        {
            if (databaseName == null)
                throw new ArgumentNullException(nameof(databaseName));

            _databaseName = databaseName;

            return this;
        }

        /// <summary>
        /// Gets the configured repository context factory.
        /// </summary>
        public IRepositoryContextFactory ContextFactory
        {
            get
            {
                if (_contextFactory == null)
                {
                    _contextFactory = string.IsNullOrEmpty(_databaseName)
                        ? new InMemoryRepositoryContextFactory()
                        : new InMemoryRepositoryContextFactory(_databaseName);
                }

                return _contextFactory;
            }
        }

        /// <summary>
        /// Gets the type of the repository context factory.
        /// </summary>
        public Type ContextFactoryType { get { return typeof(InMemoryRepositoryContextFactory); } }
    }
}
