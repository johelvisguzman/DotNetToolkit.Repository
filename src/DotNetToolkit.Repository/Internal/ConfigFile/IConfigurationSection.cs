namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Configuration.Caching;
    using Configuration.Interceptors;
    using Configuration.Logging;
    using Configuration.Mapper;
    using Factories;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an internal configuration section for configuring the repositories.
    /// </summary>
    internal interface IConfigurationSection
    {
        IRepositoryContextFactory GetDefaultContextFactory();

        ILoggerProvider GetLoggerProvider();

        ICacheProvider GetCachingProvider();

        IMapperProvider GetMappingProvider();

        Dictionary<Type, Func<IRepositoryInterceptor>> GetInterceptors();
    }
}
