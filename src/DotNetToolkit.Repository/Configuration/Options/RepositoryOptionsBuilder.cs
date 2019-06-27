namespace DotNetToolkit.Repository.Configuration.Options
{
    using Caching;
    using Conventions;
    using Interceptors;
    using Internal;
    using JetBrains.Annotations;
    using Logging;
    using Mapper;
    using Properties;
    using System;
    using System.Linq;
    using Utility;

    /// <summary>
    /// Represents a builder used to create or modify options for a repository.
    /// </summary>
    public class RepositoryOptionsBuilder
    {
        #region Fields

        private RepositoryOptions _options;

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
                       Options.Conventions != null ||
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
            _options = new RepositoryOptions(Guard.NotNull(options, nameof(options)));
        }

        #endregion

        #region Public Methods

#if !NETSTANDARD1_3
        /// <summary>
        /// Configures the repository options with the data from the <paramref name="fileName"/>; otherwise, it will configure using the default App.config.
        /// </summary>
        /// <param name="fileName">The name of the file to configure from.</param>
        /// <returns>The same builder instance.</returns>
        /// <remarks>Any element that is defined in the config file can be resolved using the <see cref="RepositoryDependencyResolver"/>.</remarks>
        public virtual RepositoryOptionsBuilder UseConfiguration([CanBeNull] string fileName = null)
        {
            const string SectionName = DotNetToolkit.Repository.Internal.ConfigFile.ConfigurationSection.SectionName;

            DotNetToolkit.Repository.Internal.ConfigFile.ConfigurationSection config;

            if (!string.IsNullOrEmpty(fileName))
            {
                var fileMap = new System.Configuration.ExeConfigurationFileMap { ExeConfigFilename = fileName };
                var exeConfiguration = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(fileMap, System.Configuration.ConfigurationUserLevel.None);

                if (!exeConfiguration.HasFile)
                    throw new System.IO.FileNotFoundException("The file is not found.", fileName);

                config = (DotNetToolkit.Repository.Internal.ConfigFile.ConfigurationSection)exeConfiguration.GetSection(SectionName);
            }
            else
            {
                config = (DotNetToolkit.Repository.Internal.ConfigFile.ConfigurationSection)System.Configuration.ConfigurationManager.GetSection(SectionName);
            }

            if (config == null)
                throw new InvalidOperationException(string.Format(Resources.UnableToFindConfigurationSection, SectionName));

            UseConfiguration(config);

            return this;
        }
#endif

#if NETSTANDARD
        /// <summary>
        /// Configures the repository options using the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The same builder instance.</returns>
        /// <remarks>Any element that is defined in the config file can be resolved using the <see cref="RepositoryDependencyResolver"/>.</remarks>
        public virtual RepositoryOptionsBuilder UseConfiguration([NotNull]  Microsoft.Extensions.Configuration.IConfigurationRoot configuration)
        {
            Guard.NotNull(configuration, nameof(configuration));

            const string SectionName = DotNetToolkit.Repository.Internal.ConfigFile.Json.ConfigurationSection.SectionName;

            var root = configuration.GetSection(SectionName);

            if (root == null || !root.GetChildren().Any())
                throw new InvalidOperationException(string.Format(Resources.UnableToFindConfigurationSection, SectionName));

            var config = new DotNetToolkit.Repository.Internal.ConfigFile.Json.ConfigurationSection(root);

            UseConfiguration(config);

            return this;
        } 
#endif

        /// <summary>
        /// Configures the repository options with an interceptor that intercepts any activity within the repository.
        /// </summary>
        /// <param name="underlyingType">The type of interceptor.</param>
        /// <param name="interceptorFactory">The interceptor factory.</param>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseInterceptor([NotNull] Type underlyingType, [NotNull] Func<IRepositoryInterceptor> interceptorFactory)
        {
            _options = _options.With(Guard.NotNull(underlyingType, nameof(underlyingType)), Guard.NotNull(interceptorFactory, nameof(interceptorFactory)));

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
            _options = _options.With(Guard.NotNull(loggerProvider, nameof(loggerProvider)));

            return this;
        }

        /// <summary>
        /// Configures the repository options with a caching provider for caching queries within the repository.
        /// </summary>
        /// <param name="cacheProvider">The caching provider.</param>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseCachingProvider([NotNull] ICacheProvider cacheProvider)
        {
            _options = _options.With(Guard.NotNull(cacheProvider, nameof(cacheProvider)));

            return this;
        }

        /// <summary>
        /// Configures the repository options with a mapper provider for mapping an query result to a valid entity object within the repository.
        /// </summary>
        /// <param name="mapperProvider">The entity mapper provider.</param>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseMapperProvider([NotNull] IMapperProvider mapperProvider)
        {
            _options = _options.With(Guard.NotNull(mapperProvider, nameof(mapperProvider)));

            return this;
        }

        /// <summary>
        /// Configures the repository options with an internal context factory.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseInternalContextFactory([NotNull] IRepositoryContextFactory contextFactory)
        {
            _options = _options.With(Guard.NotNull(contextFactory, nameof(contextFactory)));

            return this;
        }

        /// <summary>
        /// Configures the repository options with the specified conventions.
        /// </summary>
        /// <param name="conventionsAction">The configurable conventions action.</param>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseConventions([NotNull] Action<IRepositoryConventions> conventionsAction)
        {
            Guard.NotNull(conventionsAction, nameof(conventionsAction));

            var conventions = new RepositoryConventions();

            conventionsAction(conventions);

            _options = _options.With(conventions);

            return this;
        }

        #endregion

        #region Private Methods

        private void UseConfiguration([NotNull] Repository.Internal.ConfigFile.IConfigurationSection config)
        {
            Guard.NotNull(config, nameof(config));

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
        }

        #endregion
    }
}
