namespace DotNetToolkit.Repository
{
    using System;
    using Logging;

    /// <summary>
    /// An implementation of <see cref="IRepositoryFactoryOptions" />.
    /// </summary>
    public class RepositoryFactoryOptions : IRepositoryFactoryOptions
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactoryOptions"/> class.
        /// </summary>
        /// <param name="dbContextType">The type of the database context.</param>
        /// <param name="dbContextArgs">The database context arguments.</param>
        public RepositoryFactoryOptions(Type dbContextType, params object[] dbContextArgs)
        {
            DbContextType = dbContextType;
            DbContextArgs = dbContextArgs;
        }

        #endregion

        #region Implementation of IRepositoryFactoryOptions

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
