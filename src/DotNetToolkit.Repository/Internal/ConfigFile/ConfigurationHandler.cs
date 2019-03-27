namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Configuration.Caching;
    using Configuration.Interceptors;
    using Configuration.Logging;
    using Extensions;
    using Factories;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class ConfigurationHandler
    {
        #region Fields

        private readonly IConfigurationSection _root;

        private const string RepositorySectionKey = "repository";
        private const string DefaultContextFactorySectionKey = "defaultContextFactory";
        private const string LoggingProviderSectionKey = "loggingProvider";
        private const string CachingProviderSectionKey = "cachingProvider";
        private const string ExpiryKey = "expiry";
        private const string InterceptorCollectionSectionKey = "interceptors";
        private const string ParameterCollectionSectionKey = "parameters";
        private const string ValueKey = "value";
        private const string TypeKey = "type";

        private readonly Func<Type, object> _defaultFactory;

        #endregion

        #region Constructors

        public ConfigurationHandler(IConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            _root = config.GetSection(RepositorySectionKey);

            if (_root == null)
                throw new InvalidOperationException("Unable to find a configuration for the repositories.");

            _defaultFactory = ConfigurationProvider.GetDefaultFactory();
        }

        #endregion

        #region Public Methods

        public IRepositoryContextFactory GetDefaultContextFactory()
        {
            var section = _root.GetSection(DefaultContextFactorySectionKey);

            if (section != null)
            {
                var type = ExtractType(section);
                var args = ExtractParameters(section);

                return CreateInstance<IRepositoryContextFactory>(type, args.ToArray());
            }

            return null;
        }

        public ILoggerProvider GetLoggerProvider()
        {
            var section = _root.GetSection(LoggingProviderSectionKey);

            if (section != null)
            {
                var type = ExtractType(section);
                var args = ExtractParameters(section);

                return CreateInstance<ILoggerProvider>(type, args.ToArray());
            }

            return null;
        }

        public ICacheProvider GetCachingProvider()
        {
            var section = _root.GetSection(CachingProviderSectionKey);

            if (section != null)
            {
                var type = ExtractType(section);
                var args = ExtractParameters(section);
                var expiry = ExtractExpiry(section);

                var provider = CreateInstance<ICacheProvider>(type, args.ToArray());

                if (expiry != null)
                    provider.CacheExpiration = expiry;

                return provider;
            }

            return null;
        }

        public Dictionary<Type, Func<IRepositoryInterceptor>> GetInterceptors()
        {
            var interceptorsDict = new Dictionary<Type, Func<IRepositoryInterceptor>>();
            var section = _root.GetSection(InterceptorCollectionSectionKey);

            if (section != null)
            {
                foreach (var subSection in section.GetChildren())
                {
                    if (subSection != null)
                    {
                        var type = ExtractType(subSection);

                        IRepositoryInterceptor Factory()
                        {
                            var args = ExtractParameters(subSection);

                            return CreateInstance<IRepositoryInterceptor>(type, args.ToArray());
                        }

                        interceptorsDict.Add(type, (Func<IRepositoryInterceptor>)Factory);
                    }
                }
            }

            return interceptorsDict;
        }

        #endregion

        #region Private Methods

        private static TimeSpan? ExtractExpiry(IConfigurationSection section)
        {
            var value = Extract(section, ExpiryKey, isRequired: false);

            if (string.IsNullOrEmpty(value))
                return null;

            return TimeSpan.Parse(value);
        }

        private static List<object> ExtractParameters(IConfigurationSection section)
        {
            var parameterCollectionSection = section.GetSection(ParameterCollectionSectionKey);

            var args = new List<object>();

            if (parameterCollectionSection != null)
            {
                args.AddRange(parameterCollectionSection.GetChildren().Select(ExtractParameter));
            }

            return args;
        }

        private static Type ExtractType(IConfigurationSection section)
        {
            var value = Extract(section, TypeKey, isRequired: false);

            if (string.IsNullOrEmpty(value))
                value = "System.String";

            return Type.GetType(value, throwOnError: true);
        }

        private static object ExtractParameter(IConfigurationSection section)
        {
            var type = ExtractType(section);
            var value = Extract(section, ValueKey);

            return type.ConvertTo(value);
        }

        private static string Extract(IConfigurationSection section, string key, bool isRequired = true)
        {
            if (section[key] == null && isRequired)
                throw new InvalidOperationException($"The '{key}' key is missing for the '{section.Path}' section.");

            return section[key];
        }

        private TService CreateInstance<TService>(Type implementationType, object[] args)
        {
            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            if (args.Any())
                return (TService)Activator.CreateInstance(implementationType, args);

            if (_defaultFactory != null)
                return (TService)_defaultFactory(implementationType);

            return (TService)Activator.CreateInstance(implementationType);
        }

        #endregion
    }
}
