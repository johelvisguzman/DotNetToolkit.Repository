namespace DotNetToolkit.Repository.Json
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
        /// Configures the context to connect to a named JSON formatted database.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="path">The database directory to create.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the context provider does not support transactions.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseJsonDatabase(this RepositoryOptionsBuilder source, string path, bool ignoreTransactionWarning = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (path == null)
                throw new ArgumentNullException(nameof(path));

            source.UseInternalContextFactory(new JsonRepositoryContextFactory(path, ignoreTransactionWarning));

            return source;
        }
    }
}
