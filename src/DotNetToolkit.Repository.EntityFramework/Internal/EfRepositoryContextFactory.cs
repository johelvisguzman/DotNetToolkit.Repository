namespace DotNetToolkit.Repository.EntityFramework.Internal
{
    using Configuration;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IRepositoryContextFactory" />.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the database context.</typeparam>
    /// <seealso cref="IRepositoryContextFactory" />
    internal class EfRepositoryContextFactory<TDbContext> : IRepositoryContextFactory where TDbContext : DbContext
    {
        #region Fields

        private readonly DbConnection _existingConnection;
        private readonly string _nameOrConnectionString;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryContextFactory{TDbContext}"/> using an IOC container to resolve the <typeparamref name="TDbContext"/>.
        /// </summary>
        public EfRepositoryContextFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryContextFactory{TDbContext}"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        public EfRepositoryContextFactory(string nameOrConnectionString)
        {
            _nameOrConnectionString = Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryContextFactory{TDbContext}"/> class.
        /// </summary>
        /// <param name="existingConnection">The existing connection.</param>
        public EfRepositoryContextFactory(DbConnection existingConnection)
        {
            _existingConnection = Guard.NotNull(existingConnection, nameof(existingConnection));
        }

        #endregion

        #region Implementation of IRepositoryContextFactory

        /// <summary>
        /// Create a new repository context.
        /// </summary>
        /// <returns>The new repository context.</returns>
        public IRepositoryContext Create()
        {
            var args = new List<object>();

            if (_existingConnection != null)
            {
                args.Add(_existingConnection);
                args.Add(false); // owns connection
            }
            else if (_nameOrConnectionString != null)
            {
                args.Add(_nameOrConnectionString);
            }

            var underlyingContext = args.Count > 0
                ? (TDbContext)FastActivator.CreateInstance(typeof(TDbContext), args.ToArray())
                : RepositoryDependencyResolver.Current.Resolve<TDbContext>();

            return new EfRepositoryContext(underlyingContext);
        }

        #endregion
    }
}
