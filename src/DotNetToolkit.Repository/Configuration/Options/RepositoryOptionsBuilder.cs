namespace DotNetToolkit.Repository.Configuration.Options
{
    using Caching;
    using Conventions;
    using Factories;
    using Interceptors;
    using Internal;
    using JetBrains.Annotations;
    using Logging;
    using Mapper;
    using System;
    using System.Linq;
    using Utility;

    /// <summary>
    /// Represents a builder used to create or modify options for a repository.
    /// </summary>
    public class RepositoryOptionsBuilder
    {
        #region Fields

        private readonly RepositoryOptions _options;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether any options have been configured.
        /// </summary>
        public virtual bool IsConfigured
        {
            get
            {
                return Options.ContextFactory != null ||
                       Options.LoggerProvider != null ||
                       Options.CachingProvider != null ||
                       Options.MapperProvider != null ||
                       Options.Interceptors.Any();
            }
        }

        /// <summary>
        /// Gets the options being configured.
        /// </summary>
        public virtual IRepositoryOptions Options { get { return _options; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryOptionsBuilder"/> class.
        /// </summary>
        public RepositoryOptionsBuilder()
        {
            _options = new RepositoryOptions();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryOptionsBuilder"/> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        public RepositoryOptionsBuilder([NotNull] IRepositoryOptions options)
        {
            _options = new RepositoryOptions(Guard.NotNull(options));
        }

        #endregion

        #region Public Methods

#if !NETSTANDARD1_3
        /// <summary>
        /// Configures the repository options with the data from the App.config.
        /// </summary>
        /// <returns>The same builder instance.</returns>
        /// <remarks>Any element that is defined in the config file can be resolved using the <see cref="DotNetToolkit.Repository.Internal.ConfigFile.ConfigurationProvider.SetDefaultFactory"/></remarks>
        public virtual RepositoryOptionsBuilder UseConfiguration()
        {
            var config = (DotNetToolkit.Repository.Internal.ConfigFile.ConfigurationSection)
                System.Configuration.ConfigurationManager.GetSection(DotNetToolkit.Repository.Internal.ConfigFile.ConfigurationSection.SectionName);

            var defaultContextFactory = config.DefaultContextFactory.GetTypedValue();
            if (defaultContextFactory != null)
            {
                UseInternalContextFactory(defaultContextFactory);
            }

            var loggingProvider = config.LoggingProvider.GetTypedValue();
            if (loggingProvider != null)
            {
                UseLoggerProvider(loggingProvider);
            }

            var cachingProvider = config.CachingProvider.GetTypedValue();
            if (cachingProvider != null)
            {
                UseCachingProvider(cachingProvider);
            }

            var mappingProvider = config.MappingProvider.GetTypedValue();
            if (mappingProvider != null)
            {
                UseMapperProvider(mappingProvider);
            }

            foreach (var item in config.Interceptors.GetTypedValues())
            {
                UseInterceptor(item.Key, item.Value);
            }

            return this;
        }
#endif

        /// <summary>
        /// Configures the repository options using the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The same builder instance.</returns>
        /// <remarks>Any element that is defined in the config file can be resolved using the <see cref="DotNetToolkit.Repository.Internal.ConfigFile.ConfigurationProvider.SetDefaultFactory"/></remarks>
        public virtual RepositoryOptionsBuilder UseConfiguration([NotNull]  Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            Guard.NotNull(configuration);

            var config = new DotNetToolkit.Repository.Internal.ConfigFile.Json.ConfigurationSection(configuration);

            var defaultContextFactory = config.GetDefaultContextFactory();
            if (defaultContextFactory != null)
            {
                UseInternalContextFactory(defaultContextFactory);
            }

            var loggingProvider = config.GetLoggerProvider();
            if (loggingProvider != null)
            {
                UseLoggerProvider(loggingProvider);
            }

            var cachingProvider = config.GetCachingProvider();
            if (cachingProvider != null)
            {
                UseCachingProvider(cachingProvider);
            }

            var mappingProvider = config.GetMappingProvider();
            if (mappingProvider != null)
            {
                UseMapperProvider(mappingProvider);
            }

            foreach (var item in config.GetInterceptors())
            {
                UseInterceptor(item.Key, item.Value);
            }

            return this;
        }

        /// <summary>
        /// Configures the repository options with an interceptor that intercepts any activity within the repository.
        /// </summary>
        /// <param name="underlyingType">The type of interceptor.</param>
        /// <param name="interceptorFactory">The interceptor factory.</param>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseInterceptor([NotNull] Type underlyingType, [NotNull] Func<IRepositoryInterceptor> interceptorFactory)
        {
            _options.With(Guard.NotNull(underlyingType), Guard.NotNull(interceptorFactory));

            return this;
        }

        /// <summary>
        /// Configures the repository options with an interceptor that intercepts any activity within the repository.
        /// </summary>
        /// <typeparam name="TInterceptor">The type of interceptor.</typeparam>
        /// <param name="interceptorFactory">The interceptor factory.</param>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseInterceptor<TInterceptor>([NotNull] Func<TInterceptor> interceptorFactory) where TInterceptor : class, IRepositoryInterceptor
        {
            return UseInterceptor(typeof(TInterceptor), interceptorFactory);
        }

        /// <summary>
        /// Configures the repository options with an interceptor that intercepts any activity within the repository.
        /// </summary>
        /// <typeparam name="TInterceptor">The type of interceptor.</typeparam>
        /// <param name="interceptor">The interceptor.</param>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseInterceptor<TInterceptor>([NotNull] TInterceptor interceptor) where TInterceptor : class, IRepositoryInterceptor
        {
            return UseInterceptor<TInterceptor>(() => interceptor);
        }

        /// <summary>
        /// Configures the repository options with a logger provider for logging messages within the repository.
        /// </summary>
        /// <param name="loggerProvider">The logger provider.</param>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseLoggerProvider([NotNull] ILoggerProvider loggerProvider)
        {
            _options.With(Guard.NotNull(loggerProvider));

            return this;
        }

        /// <summary>
        /// Configures the repository options with a caching provider for caching queries within the repository.
        /// </summary>
        /// <param name="cacheProvider">The caching provider.</param>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseCachingProvider([NotNull] ICacheProvider cacheProvider)
        {
            _options.With(Guard.NotNull(cacheProvider));

            return this;
        }

        /// <summary>
        /// Configures the repository options with a mapper provider for mapping an query result to a valid entity object within the repository.
        /// </summary>
        /// <param name="mapperProvider">The entity mapper provider.</param>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseMapperProvider([NotNull] IMapperProvider mapperProvider)
        {
            _options.With(Guard.NotNull(mapperProvider));

            return this;
        }

        /// <summary>
        /// Configures the repository options with an internal context factory.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseInternalContextFactory([NotNull] IRepositoryContextFactory contextFactory)
        {
            _options.With(Guard.NotNull(contextFactory));

            return this;
        }

        /// <summary>
        /// Configures the repository options with the specified conventions.
        /// </summary>
        /// <param name="conventions">The configurable conventions.</param>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseConventions([NotNull] IRepositoryConventions conventions)
        {
            _options.With(Guard.NotNull(conventions));

            return this;
        }

        #endregion
    }
}
