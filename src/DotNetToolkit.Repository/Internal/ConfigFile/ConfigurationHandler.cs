namespace DotNetToolkit.Repository.Internal.ConfigFile
{
    using Configuration.Interceptors;
    using Factories;
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    internal class ConfigurationHandler
    {
        #region Fields

        private readonly IConfigurationSection _root;

        private const string RepositorySectionKey = "repository";
        private const string DefaultContextFactorySectionKey = "defaultContextFactory";
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
                if (defaultContextFactorySection[TypeKey] == null)
                    throw new InvalidOperationException($"The '{TypeKey}' is missing for this interceptor section.");

                var contextFactoryType = Type.GetType(defaultContextFactorySection[TypeKey], throwOnError: true);

                var parameterCollectionSection = defaultContextFactorySection.GetSection(ParameterCollectionSectionKey);
                var args = new List<object>();

                if (parameterCollectionSection != null)
                {
                    args.AddRange(parameterCollectionSection.GetChildren().Select(ExtractParameterValue));
                }

                return CreateInstance<IRepositoryContextFactory>(contextFactoryType, args.ToArray());
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
                        var interceptorType = Type.GetType(interceptorSection[TypeKey], throwOnError: true);

                        IRepositoryInterceptor Factory()
                        {
                            if (defaultFactory != null)
                                return (IRepositoryInterceptor)defaultFactory(interceptorType);

                            var parameterCollectionSection = interceptorSection.GetSection(ParameterCollectionSectionKey);
                            var args = new List<object>();

                            if (parameterCollectionSection != null)
                            {
                                args.AddRange(parameterCollectionSection.GetChildren().Select(ExtractParameterValue));
                            }

                            return CreateInstance<IRepositoryInterceptor>(interceptorType, args.ToArray());
                        }

                        interceptorsDict.Add(interceptorType, (Func<IRepositoryInterceptor>)Factory);
                    }
                }
            }

            return interceptorsDict;
        }

        private static object ExtractParameterValue(IConfigurationSection parameterSection)
        {
            if (parameterSection[TypeKey] == null)
                throw new InvalidOperationException($"The '{TypeKey}' is missing for this parameter section.");

            if (parameterSection[ValueKey] == null)
                throw new InvalidOperationException($"The '{ValueKey}' is missing for this parameter section.");

            var parameterType = Type.GetType(parameterSection[TypeKey], throwOnError: true);
            var parameterValue = Convert.ChangeType(parameterSection[ValueKey], parameterType, CultureInfo.InvariantCulture);

            return parameterValue;
        }

        #endregion

        #region Private Methods

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
