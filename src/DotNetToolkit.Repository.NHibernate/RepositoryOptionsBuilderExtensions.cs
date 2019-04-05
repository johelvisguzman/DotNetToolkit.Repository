namespace DotNetToolkit.Repository.NHibernate
{
    using Configuration.Options;
    using global::NHibernate;
    using global::NHibernate.Cfg;
    using Internal;
    using JetBrains.Annotations;
    using System;
    using Utility;

    /// <summary>
    /// Contains various extension methods for <see cref="RepositoryOptionsBuilder" />
    /// </summary>
    public static class RepositoryOptionsBuilderExtensions
    {
        /// <summary>
        /// Configures the context to use NHibernate.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <returns>The same builder instance.</returns>
        /// <remarks>
        /// Configure NHibernate using the <c>&lt;hibernate-configuration&gt;</c> section
        /// from the application config file, if found, or the file <c>hibernate.cfg.xml</c> if the
        /// <c>&lt;hibernate-configuration&gt;</c> section not include the session-factory configuration.
        /// </remarks>
        public static RepositoryOptionsBuilder UseNHibernate([NotNull] this RepositoryOptionsBuilder source)
        {
            Guard.NotNull(source, nameof(source));

            source.UseInternalContextFactory(new NHibernateRepositoryContextFactory());

            return source;
        }

        /// <summary>
        /// Configures the context to use NHibernate.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="fileName">The location of the XML file to use to configure NHibernate.</param>
        /// <returns>The same builder instance.</returns>
        /// <remarks>
        /// Calling this constructor will override/merge the values set in app.config or web.config.
        /// </remarks>
        public static RepositoryOptionsBuilder UseNHibernate([NotNull] this RepositoryOptionsBuilder source, [NotNull] string fileName)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotEmpty(fileName, nameof(fileName));

            source.UseInternalContextFactory(new NHibernateRepositoryContextFactory(fileName));

            return source;
        }

        /// <summary>
        /// Configures the context to use NHibernate.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="cfgAction">A configuration action used to configure NHibernate.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseNHibernate([NotNull] this RepositoryOptionsBuilder source, [NotNull] Action<Configuration> cfgAction)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(cfgAction, nameof(cfgAction));

            var cfg = new Configuration();

            cfgAction(cfg);

            source.UseNHibernate(cfg.BuildSessionFactory());

            return source;
        }

        /// <summary>
        /// Configures the context to use NHibernate.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="sessionFactory">The session factory.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseNHibernate([NotNull] this RepositoryOptionsBuilder source, [NotNull] ISessionFactory sessionFactory)
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(sessionFactory, nameof(sessionFactory));

            source.UseInternalContextFactory(new NHibernateRepositoryContextFactory(sessionFactory));

            return source;
        }
    }
}
