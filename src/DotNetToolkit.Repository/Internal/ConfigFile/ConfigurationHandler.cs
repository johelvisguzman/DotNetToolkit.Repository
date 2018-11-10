#if NETSTANDARD2_0

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
        private const string InterceptorSectionKey = "interceptor";
        private const string ParameterCollectionSectionKey = "parameters";
        private const string ParameterSectionKey = "parameter";
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
                var parameterCollectionSection = defaultContextFactorySection.GetSection(ParameterCollectionSectionKey);
                var args = new List<object>();

                if (parameterCollectionSection != null)
                {
                    foreach (var parameterCollectionSectionChildren in parameterCollectionSection.GetChildren())
                    {
                        var parameterSection = parameterCollectionSectionChildren.GetSection(ParameterSectionKey);

                        if (parameterSection != null)
                        {
                            var parameterType = Type.GetType(parameterSection[TypeKey], throwOnError: true);
                            var parameterValue = Convert.ChangeType(parameterSection[ValueKey], parameterType, CultureInfo.InvariantCulture);

                            args.Add(parameterValue);
                        }
                    }
                }

                var contextFactoryType = Type.GetType(defaultContextFactorySection[TypeKey], throwOnError: true);

                return CreateInstance<IRepositoryContextFactory>(contextFactoryType, args.ToArray());
            }

            return null;
        }

        public IEnumerable<IRepositoryInterceptor> GetInterceptors()
        {
            var interceptors = new List<IRepositoryInterceptor>();
            var interceptorCollectionSection = _root.GetSection(InterceptorCollectionSectionKey);

            if (interceptorCollectionSection != null)
            {
                var defaultFactory = RepositoryInterceptorProvider.GetDefaultFactory();

                foreach (var interceptorCollectionSectionChildren in interceptorCollectionSection.GetChildren())
                {
                    var interceptorSection = interceptorCollectionSectionChildren.GetSection(InterceptorSectionKey);

                    if (interceptorSection != null)
                    {
                        var interceptorType = Type.GetType(interceptorSection[TypeKey], throwOnError: true);

                        if (defaultFactory != null)
                        {
                            interceptors.Add((IRepositoryInterceptor)defaultFactory(interceptorType));
                        }
                        else
                        {
                            var parameterCollectionSection = interceptorSection.GetSection(ParameterCollectionSectionKey);
                            var args = new List<object>();

                            if (parameterCollectionSection != null)
                            {
                                foreach (var parameterCollectionSectionChildren in parameterCollectionSection.GetChildren())
                                {
                                    var parameterSection = parameterCollectionSectionChildren.GetSection(ParameterSectionKey);

                                    if (parameterSection != null)
                                    {
                                        var parameterType = Type.GetType(parameterSection[TypeKey], throwOnError: true);
                                        var parameterValue = Convert.ChangeType(parameterSection[ValueKey], parameterType, CultureInfo.InvariantCulture);

                                        args.Add(parameterValue);
                                    }
                                }
                            }

                            interceptors.Add(CreateInstance<IRepositoryInterceptor>(interceptorType, args.ToArray()));
                        }
                    }
                }
            }

            return interceptors;
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

#endif