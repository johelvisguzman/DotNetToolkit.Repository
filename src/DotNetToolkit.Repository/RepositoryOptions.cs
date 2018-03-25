namespace DotNetToolkit.Repository
{
    using System;
    using Logging;

    /// <summary>
    /// An implementation of <see cref="IRepositoryOptions" />.
    /// </summary>
    public class RepositoryOptions : IRepositoryOptions
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryOptions"/> class.
        /// </summary>
        /// <param name="dbContextType">The type of the database context.</param>
        /// <param name="dbContextArgs">The database context arguments.</param>
        public RepositoryOptions(Type dbContextType, params object[] dbContextArgs)
        {
            DbContextType = dbContextType;
            DbContextArgs = dbContextArgs;
        }

        #endregion

        #region Implementation of IRepositoryOptions

        /// <summary>
        /// Gets or sets the database context arguments.
        /// </summary>
        public object[] DbContextArgs { get; set; }

        /// <summary>
        /// Gets or sets the type of the database context.
        /// </summary>
        public Type DbContextType { get; set; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        #endregion
    }
}
