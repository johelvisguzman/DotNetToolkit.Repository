namespace DotNetToolkit.Repository.Internal.ConfigFile
{
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
        private const string InterceptorCollectionSectionKey = "interceptors";
        private const string ParameterCollectionSectionKey = "parameters";
        private const string ValueKey = "value";
        private const string TypeKey = "type";

        #endregion

        #region Constructors

        public ConfigurationHandler(IConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            _root = config.GetSection(RepositorySectionKey);

            if (_root == null)
                throw new InvalidOperationException("Unable to find a configuration for the repositories.");
        }

        #endregion

        #region Public Methods

        public IRepositoryContextFactory GetDefaultContextFactory()
        {
            var defaultContextFactorySection = _root.GetSection(DefaultContextFactorySectionKey);

            if (defaultContextFactorySection != null)
            {
                var type = ExtractType(defaultContextFactorySection);
                var args = ExtractParameters(defaultContextFactorySection);

                return CreateInstance<IRepositoryContextFactory>(type, args.ToArray());
            }

            return null;
        }

        public ILoggerProvider GetLoggerProvider()
        {
            var loggingProviderSection = _root.GetSection(LoggingProviderSectionKey);

            if (loggingProviderSection != null)
            {
                var type = ExtractType(loggingProviderSection);
                var args = ExtractParameters(loggingProviderSection);

                return CreateInstance<ILoggerProvider>(type, args.ToArray());
            }

            return null;
        }

        public Dictionary<Type, Func<IRepositoryInterceptor>> GetInterceptors()
        {
            var interceptorsDict = new Dictionary<Type, Func<IRepositoryInterceptor>>();
            var interceptorCollectionSection = _root.GetSection(InterceptorCollectionSectionKey);

            if (interceptorCollectionSection != null)
            {
                var defaultFactory = RepositoryInterceptorProvider.GetDefaultFactory();

                foreach (var interceptorSection in interceptorCollectionSection.GetChildren())
                {
                    if (interceptorSection != null)
                    {
                        var type = ExtractType(interceptorSection);

                        IRepositoryInterceptor Factory()
                        {
                            if (defaultFactory != null)
                                return (IRepositoryInterceptor)defaultFactory(type);

                            var args = ExtractParameters(interceptorSection);

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
            return Type.GetType(Extract(section, TypeKey), throwOnError: true);
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

            return (TService)Activator.CreateInstance(implementationType);
        }

        #endregion
    }
}
