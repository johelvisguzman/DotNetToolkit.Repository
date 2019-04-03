namespace DotNetToolkit.Repository.Xml
{
    using Configuration.Options;
    using Internal;
    using System;

    /// <summary>
    /// Contains various extension methods for <see cref="RepositoryOptionsBuilder" />
    /// </summary>
    public static class RepositoryOptionsBuilderExtensions
    {
        /// <summary>
        /// Configures the context to connect to a named XML formatted database.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="path">The database directory to create.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the context provider does not support transactions.</param>
        /// <param name="ignoreSqlQueryWarning">If a SQL query is executed, ignore any warnings since the in-memory provider does not support SQL query execution.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseXmlDatabase(this RepositoryOptionsBuilder source, string path, bool ignoreTransactionWarning = false, bool ignoreSqlQueryWarning = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            source.UseInternalContextFactory(new XmlRepositoryContextFactory(path, ignoreTransactionWarning, ignoreSqlQueryWarning));

            return source;
        }
    }
}
