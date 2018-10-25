namespace DotNetToolkit.Repository.Configuration.Options
{
    using Factories;
    using Interceptors;
    using System;

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
        public virtual bool IsConfigured { get { return Options.IsConfigured; } }

        /// <summary>
        /// Gets the options being configured.
        /// </summary>
        public virtual RepositoryOptions Options { get { return _options; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity}"/> class.
        /// </summary>
        public RepositoryOptionsBuilder()
        {
            _options = new RepositoryOptions();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity}"/> class.
        /// </summary>
        /// <param name="options">The repository options.</param>
        public RepositoryOptionsBuilder(RepositoryOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            _options = options.Clone();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Configures the repository with an internal context factory.
        /// </summary>
        /// <param name="contextFactory">The context factory.</param>
        /// <returns>The same builder instance.</returns>
        internal virtual RepositoryOptionsBuilder UseInternalContextFactory(IRepositoryContextFactory contextFactory)
        {
            if (contextFactory == null)
                throw new ArgumentNullException(nameof(contextFactory));

            _options.AddInternalContextFactory(contextFactory);

            return this;
        }

        #endregion

        #region Implementation of RepositoryOptionsBuilder

#if !NETSTANDARD1_3
        /// <summary>
        /// Configures the repository with the data from the App.config.
        /// </summary>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseConfiguration()
        {
            var config = (Internal.ConfigFile.ConfigurationSection)
                System.Configuration.ConfigurationManager.GetSection(Internal.ConfigFile.ConfigurationSection.SectionName);

            var defaultContextFactory = config.DefaultContextFactory.GetTypedValue();
            if (defaultContextFactory != null)
            {
                UseInternalContextFactory(defaultContextFactory);
            }

            foreach (var interceptor in config.Interceptors.GetTypedValues())
            {
                UseInterceptor(interceptor.GetType(), () => interceptor);
            }

            return this;
        }
#endif

#if NETSTANDARD2_0
        /// <summary>
        /// Configures the repository using the specified configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseConfiguration(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            var config = new Internal.ConfigFile.ConfigurationHandler(configuration);

            var defaultContextFactory = config.GetDefaultContextFactory();
            if (defaultContextFactory != null)
            {
                UseInternalContextFactory(defaultContextFactory);
            }

            foreach (var interceptor in config.GetInterceptors())
            {
                UseInterceptor(interceptor.GetType(), () => interceptor);
            }

            return this;
        }
#endif

        /// <summary>
        /// Configures the repository with an interceptor that intercepts any activity within the repository.
        /// </summary>
        /// <param name="underlyingType">The type of interceptor.</param>
        /// <param name="interceptorFactory">The interceptor factory.</param>
        /// <returns>The same builder instance.</returns>
        public virtual RepositoryOptionsBuilder UseInterceptor(Type underlyingType, Func<IRepositoryInterceptor> interceptorFactory)
        {
            if (underlyingType == null)
                throw new ArgumentNullException(nameof(underlyingType));

            if (interceptorFactory == null)
                throw new ArgumentNullException(nameof(interceptorFactory));

            _options.AddInterceptor(underlyingType, interceptorFactory);

            return this;
        }

        /// <summary>
        /// Configures the repository with an interceptor that intercepts any activity within the repository.
        /// </summary>
        /// <typeparam name="TInterceptor">The type of interceptor.</typeparam>
        /// <param name="interceptorFactory">The interceptor factory.</param>
        /// <returns>The same builder instance.</returns>
        public RepositoryOptionsBuilder UseInterceptor<TInterceptor>(Func<TInterceptor> interceptorFactory) where TInterceptor : class, IRepositoryInterceptor
        {
            return UseInterceptor(typeof(TInterceptor), interceptorFactory);
        }

        /// <summary>
        /// Configures the repository with an interceptor that intercepts any activity within the repository.
        /// </summary>
        /// <typeparam name="TInterceptor">The type of interceptor.</typeparam>
        /// <param name="interceptor">The interceptor.</param>
        /// <returns>The same builder instance.</returns>
        public RepositoryOptionsBuilder UseInterceptor<TInterceptor>(TInterceptor interceptor) where TInterceptor : class, IRepositoryInterceptor
        {
            return UseInterceptor<TInterceptor>(() => interceptor);
        }

        #endregion
    }
}
