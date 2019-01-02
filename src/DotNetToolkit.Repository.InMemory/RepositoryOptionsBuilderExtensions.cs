namespace DotNetToolkit.Repository.InMemory
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
        /// Configures the context to connect to a in-memory database.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the in-memory provider does not support transactions.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseInMemoryDatabase(this RepositoryOptionsBuilder source, bool ignoreTransactionWarning = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.UseInternalContextFactory(new InMemoryRepositoryContextFactory(ignoreTransactionWarning));

            return source;
        }

        /// <summary>
        /// Configures the context to connect to a named in-memory database.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the in-memory provider does not support transactions.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseInMemoryDatabase(this RepositoryOptionsBuilder source, string databaseName, bool ignoreTransactionWarning = false)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (databaseName == null)
                throw new ArgumentNullException(nameof(databaseName));

            source.UseInternalContextFactory(new InMemoryRepositoryContextFactory(databaseName, ignoreTransactionWarning));

            return source;
        }
    }
}
