namespace DotNetToolkit.Repository
{
    using Configuration;
    using Configuration.Caching;
    using Configuration.Conventions;
    using Configuration.Interceptors;
    using Configuration.Logging;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a repository options.
    /// </summary>
    public interface IRepositoryOptions
    {
        /// <summary>
        /// Gets the configured interceptors.
        /// </summary>
        IReadOnlyDictionary<Type, Lazy<IRepositoryInterceptor>> Interceptors { get; }

        /// <summary>
        /// Gets the configured logger provider.
        /// </summary>
        ILoggerProvider LoggerProvider { get; }

        /// <summary>
        /// Gets the configured caching provider.
        /// </summary>
        ICacheProvider CachingProvider { get; }

        /// <summary>
        /// Gets the configured internal context factory.
        /// </summary>
        IRepositoryContextFactory ContextFactory { get; }

        /// <summary>
        /// Gets the configured conventions.
        /// </summary>
        IRepositoryConventions Conventions { get; }
    }
}
