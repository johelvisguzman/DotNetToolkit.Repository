namespace DotNetToolkit.Repository.Extensions.Microsoft.DependencyInjection.Internal
{
    using Configuration.Interceptors;
    using Factories;
    using global::Microsoft.Extensions.Configuration;
    using global::Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    internal class ConfigurationHandler
    {
        #region Fields

        private readonly IConfigurationSection _root;
        private readonly IServiceCollection _services;

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

        public ConfigurationHandler(IConfiguration config, IServiceCollection services)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (services == null)
                throw new ArgumentNullException(nameof(services));

            _root = config.GetSection(RepositorySectionKey);

            if (_root == null)
                throw new InvalidOperationException("Unable to find a configuration for the repositories.");

            _services = services;
        }

        #endregion

        #region Public Methods

        public ConfigurationHandler AddContextFactory()
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

                AddScopedService<IRepositoryContextFactory>(contextFactoryType, args.ToArray());
            }

            return this;
        }

        public ConfigurationHandler AddInterceptors()
        {
            var interceptorCollectionSection = _root.GetSection(InterceptorCollectionSectionKey);

            if (interceptorCollectionSection != null)
            {
                foreach (var interceptorCollectionSectionChildren in interceptorCollectionSection.GetChildren())
                {
                    var interceptorSection = interceptorCollectionSectionChildren.GetSection(InterceptorSectionKey);

                    if (interceptorSection != null)
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

                        var interceptorType = Type.GetType(interceptorSection[TypeKey], throwOnError: true);

                        AddScopedService<IRepositoryInterceptor>(interceptorType, args.ToArray());
                    }
                }
            }

            return this;
        }

        #endregion

        #region Private Methods

        private void AddScopedService<TService>(Type implementationType, params object[] args)
        {
            if (implementationType == null)
                throw new ArgumentNullException(nameof(implementationType));

            if (args.Any())
            {
                _services.AddScoped(typeof(TService), sp => Activator.CreateInstance(implementationType, args));
                _services.AddScoped(implementationType, sp => Activator.CreateInstance(implementationType, args));
            }
            else
            {
                _services.AddScoped(typeof(TService), implementationType);
                _services.AddScoped(implementationType);
            }
        }

        #endregion
    }
}
