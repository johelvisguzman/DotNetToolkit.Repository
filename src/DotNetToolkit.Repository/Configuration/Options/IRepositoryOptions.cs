namespace DotNetToolkit.Repository.Configuration.Options
{
    using Caching;
    using Factories;
    using Interceptors;
    using Logging;
    using Mapper;
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
        /// Gets the configured mapper provider.
        /// </summary>
        IMapperProvider MapperProvider { get; }

        /// <summary>
        /// Gets the configured internal context factory.
        /// </summary>
        IRepositoryContextFactory ContextFactory { get; }
    }
}
