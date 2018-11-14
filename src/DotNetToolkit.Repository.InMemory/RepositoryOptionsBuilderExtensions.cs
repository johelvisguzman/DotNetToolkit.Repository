namespace DotNetToolkit.Repository.InMemory
{
    using Configuration.Options;
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
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseInMemoryDatabase(this RepositoryOptionsBuilder source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Options.With(new InMemoryRepositoryContextFactory());

            return source;
        }

        /// <summary>
        /// Configures the context to connect to a named in-memory database.
        /// </summary>
        /// <param name="source">The repository options builder.</param>
        /// <param name="databaseName">The name of the in-memory database. This allows the scope of the in-memory database to be controlled independently of the context.</param>
        /// <returns>The same builder instance.</returns>
        public static RepositoryOptionsBuilder UseInMemoryDatabase(this RepositoryOptionsBuilder source, string databaseName)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Options.With(new InMemoryRepositoryContextFactory(databaseName));

            return source;
        }
    }
}
