namespace DotNetToolkit.Repository.Extensions.Microsoft.DependencyInjection
{
    using Configuration;
    using Configuration.Interceptors;
    using Factories;
    using global::Microsoft.Extensions.Configuration;
    using global::Microsoft.Extensions.DependencyInjection;
    using Helpers;
    using Internal;
    using System;
    using System.Linq;
    using Transactions;

    /// <summary>
    /// Contains various extension methods for <see cref="IServiceCollection" />
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all the repository services using the specified configuration.
        /// </summary>
        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration config)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            // If a context factory has not been registered already, try to add one from the config
            if (services.All(sd => sd.ServiceType != typeof(IRepositoryContextFactory)))
            {
                new ConfigurationHandler(config, services)
                    .AddContextFactory();
            }

            return AddRepositories(services, null, config);
        }

        /// <summary>
        /// Adds all the repository services using the specified repository context and configuration.
        /// </summary>
        public static IServiceCollection AddRepositories(this IServiceCollection services, IRepositoryContextFactory contextFactory, IConfiguration config)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // Configure context factory
            if (contextFactory != null)
            {
                services.AddScoped(typeof(IRepositoryContextFactory), sp => contextFactory);
            }

            // Configures services from the config file
            if (config != null)
            {
                new ConfigurationHandler(config, services)
                    .AddInterceptors();
            }

            services.AddScoped<IRepositoryConfigurationOptions, RepositoryConfigurationOptions>(sp =>
            {
                var ctxFactory = sp.GetRequiredService<IRepositoryContextFactory>();
                var interceptors = sp.GetServices<IRepositoryInterceptor>();

                return new RepositoryConfigurationOptions(ctxFactory, interceptors);
            });

            // Configure other services
            services.AddScoped<IRepositoryFactory, RepositoryFactory>(
                sp => new RepositoryFactory(sp.GetRequiredService<IRepositoryConfigurationOptions>()));

            services.AddScoped<IUnitOfWork, UnitOfWork>(sp => new UnitOfWork(
                sp.GetRequiredService<IRepositoryConfigurationOptions>()));

            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>(
                sp => new UnitOfWorkFactory(sp.GetRequiredService<IRepositoryConfigurationOptions>()));

            // Gets all the accessible types for all the assemblies
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetAccessibleTypes());

            var interfaceTypesToScan = new[]
            {
                typeof(IRepository<>),
                typeof(IRepository<,>),
                typeof(IRepository<,,>),
                typeof(IRepository<,,,>),
                typeof(IRepositoryInterceptor)
            };

            // Gets all the interfaces that inherent from IRepository<>
            var interfaceTypes = interfaceTypesToScan
                .SelectMany(interfaceType => types
                    .Where(t => !t.IsClass && t.ImplementsInterface(interfaceType)))
                .Distinct();

            var serviceTypesMapping = interfaceTypes
                .SelectMany(interfaceType => types
                    .Where(t => t.IsClass && !t.IsAbstract && t.ImplementsInterface(interfaceType))
                .GroupBy(t => interfaceType, t => t));

            // Register the repositories
            foreach (var t in serviceTypesMapping)
            {
                var serviceType = t.Key;
                var implementationTypes = t.Where(x => x.IsGenericType == t.Key.IsGenericType);

                foreach (var implementationType in implementationTypes)
                {
                    if (serviceType == typeof(IRepositoryInterceptor))
                    {
                        if (services.Any(x => x.ServiceType == implementationType))
                            continue;

                        services.AddScoped(serviceType, implementationType);
                    }
                    else
                    {
                        services.AddTransient(serviceType, implementationType);
                    }
                }
            }

            return services;
        }
    }
}
