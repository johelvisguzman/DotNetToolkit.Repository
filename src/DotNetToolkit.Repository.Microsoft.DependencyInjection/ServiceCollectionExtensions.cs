namespace DotNetToolkit.Repository.Extensions.Microsoft.DependencyInjection
{
    using Configuration.Interceptors;
    using Configuration.Options;
    using Factories;
    using global::Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Transactions;

    /// <summary>
    /// Contains various extension methods for <see cref="IServiceCollection" />
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds all the repository services using the specified options builder.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for this unit of work factory.</param>
        /// <returns>The same instance of the service collection which has been configured with the repositories.</returns>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the assemblies that have been loaded into the
        /// execution context of this application domain, and will register them to the service collection.
        /// </remarks>
        public static IServiceCollection AddRepositories(this IServiceCollection services, Action<RepositoryOptionsBuilder> optionsAction)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (optionsAction == null)
                throw new ArgumentNullException(nameof(optionsAction));

            // Gets all the accessible types for all the assemblies
            var assembliesToScan = AppDomain.CurrentDomain.GetAssemblies();

            var types = assembliesToScan.SelectMany(x => x.GetAccessibleTypes());

            var interfaceTypesToScan = new[]
            {
                typeof(IService<>),
                typeof(IRepository<>),
                typeof(IRepositoryInterceptor)
            };

            // Gets all the interfaces that inherent from IRepository<> or IRepositoryInterceptor
            var interfaceTypes = interfaceTypesToScan
                .SelectMany(interfaceType => types.Where(t => !t.IsClass && t.ImplementsInterface(interfaceType)))
                .Distinct();

            var serviceTypesMapping = interfaceTypes
                .SelectMany(interfaceType => types.Where(t => t.IsClass && !t.IsAbstract && t.ImplementsInterface(interfaceType))
                .GroupBy(t => interfaceType, t => t));

            // Register the repositories and interceptors that have been scanned
            var optionsBuilder = new RepositoryOptionsBuilder();

            optionsAction(optionsBuilder);

            var registeredInterceptorTypes = new List<Type>();

            foreach (var t in serviceTypesMapping)
            {
                var serviceType = t.Key;
                var implementationTypes = t.Where(x => x.IsGenericType == t.Key.IsGenericType);

                foreach (var implementationType in implementationTypes)
                {
                    if (serviceType == typeof(IRepositoryInterceptor))
                    {
                        if (services.Any(x => x.ServiceType == implementationType) || optionsBuilder.Options.ContainsInterceptorOfType(implementationType))
                            continue;

                        services.AddScoped(implementationType, implementationType);
                        services.AddScoped(serviceType, implementationType);
                        registeredInterceptorTypes.Add(implementationType);
                    }
                    else
                    {
                        services.AddTransient(serviceType, implementationType);
                    }
                }
            }

            // Register other services
            services.AddScoped<IRepositoryFactory, RepositoryFactory>(sp => new RepositoryFactory(sp.GetRequiredService<RepositoryOptions>()));
            services.AddScoped<IUnitOfWork, UnitOfWork>(sp => new UnitOfWork(sp.GetRequiredService<RepositoryOptions>()));
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>(sp => new UnitOfWorkFactory(sp.GetRequiredService<RepositoryOptions>()));
            services.AddScoped<RepositoryOptions>(sp =>
            {
                var serviceOptionsBuilder = new RepositoryOptionsBuilder(optionsBuilder.Options);

                var contextFactory = serviceOptionsBuilder.Options.ContextFactory;
                if (contextFactory == null)
                    throw new InvalidOperationException("No context provider has been configured.");

                foreach (var interceptorType in registeredInterceptorTypes)
                {
                    serviceOptionsBuilder.UseInterceptor(interceptorType, () => (IRepositoryInterceptor)sp.GetService(interceptorType));
                }

                return serviceOptionsBuilder.Options;
            });

            return services;
        }
    }
}
