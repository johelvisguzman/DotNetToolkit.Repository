namespace DotNetToolkit.Repository.EntityFramework
{
    using Configuration.Options;
    using Factories;
    using System;
    using System.Data.Common;
    using System.Data.Entity;

    /// <summary>
    /// An implementation of <see cref="IRepositoryOptionsContextFactoryExtensions" />.
    /// </summary>
    public class EfRepositoryOptionsExtension<TDbContext> : IRepositoryOptionsContextFactoryExtensions where TDbContext : DbContext
    {
        private string _nameOrConnectionString;
        private DbConnection _existingConnection;
        private EfRepositoryContextFactory<TDbContext> _contextFactory;

        /// <summary>
        /// Returns the extension instance with a connection string to configure the context.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        /// <returns>The same extension instance.</returns>
        public EfRepositoryOptionsExtension<TDbContext> WithConnectionString(string nameOrConnectionString)
        {
            if (nameOrConnectionString == null)
                throw new ArgumentNullException(nameof(nameOrConnectionString));

            _nameOrConnectionString = nameOrConnectionString;

            return this;
        }

        /// <summary>
        /// Returns the extension instance with an existing connection to configure the context.
        /// </summary>
        /// <param name="existingConnection">The existing connection.</param>
        /// <returns>The same extension instance.</returns>
        public EfRepositoryOptionsExtension<TDbContext> WithConnection(DbConnection existingConnection)
        {
            if (existingConnection == null)
                throw new ArgumentNullException(nameof(existingConnection));

            _existingConnection = existingConnection;

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
                    if (!string.IsNullOrEmpty(_nameOrConnectionString))
                        _contextFactory = new EfRepositoryContextFactory<TDbContext>(_nameOrConnectionString);
                    else if (_existingConnection != null)
                        _contextFactory = new EfRepositoryContextFactory<TDbContext>(_existingConnection);
                    else
                        _contextFactory = new EfRepositoryContextFactory<TDbContext>();
                }

                return _contextFactory;
            }
        }

        /// <summary>
        /// Gets the type of the repository context factory.
        /// </summary>
        public Type ContextFactoryType { get { return typeof(EfRepositoryContextFactory<TDbContext>); } }
    }
}
