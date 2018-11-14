namespace DotNetToolkit.Repository.Configuration.Options
{
    using Factories;
    using Interceptors;
    using Logging;
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
        IEnumerable<Lazy<IRepositoryInterceptor>> Interceptors { get; }

        /// <summary>
        /// Gets the configured logger provider.
        /// </summary>
        ILoggerProvider LoggerProvider { get; }

        /// <summary>
        /// Gets the configured internal context factory.
        /// </summary>
        IRepositoryContextFactory ContextFactory { get; }
    }
}
