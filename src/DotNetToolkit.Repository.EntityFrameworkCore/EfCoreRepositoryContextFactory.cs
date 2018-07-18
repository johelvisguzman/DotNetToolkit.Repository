namespace DotNetToolkit.Repository.EntityFrameworkCore
{
    using Configuration;
    using Factories;
    using Internal;
    using Microsoft.EntityFrameworkCore;
    using System;

    /// <summary>
    /// An implementation of <see cref="IRepositoryContextFactory" />.
    /// </summary>
    /// <typeparam name="TDbContext">The type of the database context.</typeparam>
    /// <seealso cref="IRepositoryContextFactory" />
    public class EfCoreRepositoryContextFactory<TDbContext> : IRepositoryContextFactory where TDbContext : DbContext
    {
        #region Implementation of IRepositoryContextFactory

        /// <summary>
        /// Create a new repository context.
        /// </summary>
        /// <returns>The new repository context.</returns>
        public IRepositoryContext Create()
        {
            TDbContext underlyingContext;

            try
            {
                underlyingContext = (TDbContext)Activator.CreateInstance(typeof(TDbContext));
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }

            return new EfCoreRepositoryContext(underlyingContext);

        }

        #endregion
    }
}
