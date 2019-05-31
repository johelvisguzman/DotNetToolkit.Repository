namespace DotNetToolkit.Repository.NHibernate.Internal
{
    using Configuration;
    using global::NHibernate;
    using global::NHibernate.Cfg;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IRepositoryContextFactory" />.
    /// </summary>
    /// <seealso cref="IRepositoryContextFactory" />
    internal class NHibernateRepositoryContextFactory : IRepositoryContextFactory
    {
        #region Fields

        private readonly ISessionFactory _sessionFactory;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateRepositoryContextFactory" /> class.
        /// </summary>
        /// <remarks>
        /// Configure NHibernate using the <c>&lt;hibernate-configuration&gt;</c> section
        /// from the application config file, if found, or the file <c>hibernate.cfg.xml</c> if the
        /// <c>&lt;hibernate-configuration&gt;</c> section not include the session-factory configuration.
        /// </remarks>
        public NHibernateRepositoryContextFactory()
        {
            _sessionFactory = new Configuration()
                .Configure()
                .BuildSessionFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateRepositoryContextFactory" /> class.
        /// </summary>
        /// <param name="fileName">The location of the XML file to use to configure NHibernate.</param>
        /// <remarks>
        /// Calling this constructor will override/merge the values set in app.config or web.config.
        /// </remarks>
        public NHibernateRepositoryContextFactory(string fileName)
        {
            _sessionFactory = new Configuration()
                .Configure(Guard.NotEmpty(fileName, nameof(fileName)))
                .BuildSessionFactory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateRepositoryContextFactory"/> class.
        /// </summary>
        /// <param name="sessionFactory">The session factory.</param>
        public NHibernateRepositoryContextFactory(ISessionFactory sessionFactory)
        {
            _sessionFactory = Guard.NotNull(sessionFactory, nameof(sessionFactory));
        }

        #endregion

        #region Implementation of IRepositoryContextFactory

        /// <summary>
        /// Create a new repository context.
        /// </summary>
        /// <returns>The new repository context.</returns>
        public IRepositoryContext Create()
        {
            return new NHibernateRepositoryContext(_sessionFactory);
        }

        #endregion
    }
}
