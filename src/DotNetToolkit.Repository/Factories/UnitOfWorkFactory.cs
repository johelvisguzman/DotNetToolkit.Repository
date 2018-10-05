namespace DotNetToolkit.Repository.Factories
{
    using Configuration;
    using System;
    using System.Reflection;
    using Transactions;

    /// <summary>
    /// An implementation of <see cref="IUnitOfWorkFactory" />.
    /// </summary>
    /// <seealso cref="DotNetToolkit.Repository.Factories.IUnitOfWorkFactory" />
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        #region Fields

        private readonly IRepositoryConfigurationOptions _configuration;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkFactory" /> class.
        /// </summary>
        /// <param name="factory">The repository context factory.</param>
        public UnitOfWorkFactory(IRepositoryContextFactory factory) : this(new RepositoryConfigurationOptions(factory)) { }

#if !NETSTANDARD
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkFactory" /> class and uses the data from the App.config file to configure the repositories.
        /// </summary>
        public UnitOfWorkFactory() : this(Internal.ConfigFile.ConfigurationHelper.GetRequiredConfigurationOptions()) { }
#endif
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory" /> class.
        /// </summary>
        /// <param name="configuration">The repository configuration.</param>
        internal UnitOfWorkFactory(IRepositoryConfigurationOptions configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _configuration = configuration;
        }

        #endregion

        #region Implementation of IUnitOfWorkFactory

        /// <summary>
        /// Creates a new unit of work.
        /// </summary>
        /// <returns>The new unit of work.</returns>
        public IUnitOfWork Create()
        {
            return CreateInstance<UnitOfWork>();
        }

        /// <summary>
        /// Creates a new repository for the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>The new repository.</returns>
        public T CreateInstance<T>() where T : class
        {
            try
            {
                return (T)Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { _configuration }, null);
            }
            catch (Exception ex)
            {
                throw ex.InnerException ?? ex;
            }
        }

        #endregion
    }
}
