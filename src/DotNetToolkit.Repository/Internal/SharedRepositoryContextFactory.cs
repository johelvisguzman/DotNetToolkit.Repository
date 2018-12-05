namespace DotNetToolkit.Repository.Internal
{
    using Configuration;
    using Factories;
    using System;

    /// <summary>
    /// An implementation of <see cref="IRepositoryContextFactory" />.
    /// </summary>
    internal class SharedRepositoryContextFactory : IRepositoryContextFactory
    {
        #region Fields

        private readonly IRepositoryContext _context;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IRepositoryContext"/> class.
        /// </summary>
        /// <param name="context">The shared repository context.</param>
        public SharedRepositoryContextFactory(IRepositoryContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        #endregion

        #region Implementation of IRepositoryContextFactory

        /// <summary>
        /// Create a new repository context.
        /// </summary>
        /// <returns>The new repository context.</returns>
        public IRepositoryContext Create()
        {
            return _context;
        }

        #endregion
    }
}
