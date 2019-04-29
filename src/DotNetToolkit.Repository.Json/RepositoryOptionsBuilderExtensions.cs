namespace DotNetToolkit.Repository.Json
{
    using Configuration.Options;
    using Internal;
    using JetBrains.Annotations;
    using Utility;

    /// <summary>
    /// Contains various extension methods for <see cref="RepositoryOptionsBuilder" />
    /// </summary>
    public static class RepositoryOptionsBuilderExtensions
    {
        /// <summary>
        /// Configures the context to connect to a named JSON formatted database.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="path">The database directory to create.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the context provider does not support transactions.</param>
        /// <param name="ignoreSqlQueryWarning">If a SQL query is executed, ignore any warnings since the in-memory provider does not support SQL query execution.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseJsonDatabase([NotNull] this RepositoryOptionsBuilder source, [NotNull] string path, bool ignoreTransactionWarning = false, bool ignoreSqlQueryWarning = false)
        {
            Guard.NotNull(source);
            Guard.NotEmpty(path);

            source.UseInternalContextFactory(new JsonRepositoryContextFactory(path, ignoreTransactionWarning, ignoreSqlQueryWarning));

            return source;
        }
    }
}
