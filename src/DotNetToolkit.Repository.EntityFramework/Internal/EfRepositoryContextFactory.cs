namespace DotNetToolkit.Repository.EntityFramework.Internal
{
    using Configuration;
    using Factories;
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Entity;

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
        /// Initializes a new instance of the <see cref="EfRepositoryContextFactory{TDbContext}"/> class.
        /// </summary>
        public EfRepositoryContextFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryContextFactory{TDbContext}"/> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        public EfRepositoryContextFactory(string nameOrConnectionString)
        {
            if (nameOrConnectionString == null)
                throw new ArgumentNullException(nameof(nameOrConnectionString));

            _nameOrConnectionString = nameOrConnectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryContextFactory{TDbContext}"/> class.
        /// </summary>
        /// <param name="existingConnection">The existing connection.</param>
        public EfRepositoryContextFactory(DbConnection existingConnection)
        {
            if (existingConnection == null)
                throw new ArgumentNullException(nameof(existingConnection));

            _existingConnection = existingConnection;
        }

        #endregion

        #region Implementation of IRepositoryContextFactory

        /// <summary>
        /// Create a new repository context.
        /// </summary>
        /// <returns>The new repository context.</returns>
        public IRepositoryContext Create()
        {
            TDbContext underlyingContext;

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

            try
            {
                underlyingContext = (TDbContext)Activator.CreateInstance(typeof(TDbContext), args.ToArray());
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return new EfRepositoryContext(underlyingContext);

        }

        #endregion
    }
}
