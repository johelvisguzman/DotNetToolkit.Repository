namespace DotNetToolkit.Repository.Extensions.Unity
{
    using Configuration.Interceptors;
    using Configuration.Options;
    using Factories;
    using global::Unity;
    using global::Unity.Injection;
    using global::Unity.Lifetime;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Transactions;

    /// <summary>
    /// Contains various extension methods for <see cref="IUnityContainer" />
    /// </summary>
    public static class UnityContainerExtensions
    {
        /// <summary>
        /// Register all the repositories services using the specified options builder.
        /// </summary>
        /// <param name="container">The unity container.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for the repositories.</param>
        /// <param name="assembliesToScan">The assemblies to scan.</param>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the specified assemblies collection, and will register them to the service collection.
        /// </remarks>
        public static void RegisterRepositories(this IUnityContainer container, Action<RepositoryOptionsBuilder> optionsAction, params Assembly[] assembliesToScan)
        {
            if (container == null)
                throw new ArgumentNullException(nameof(container));

            if (optionsAction == null)
                throw new ArgumentNullException(nameof(optionsAction));

            if (assembliesToScan == null)
                throw new ArgumentNullException(nameof(assembliesToScan));

            var baseAssembly = Assembly.Load("DotNetToolkit.Repository");

            var assToScan = !assembliesToScan.Any(x => x.FullName.Equals(baseAssembly.FullName))
                ? assembliesToScan.Concat(new[] { baseAssembly })
                : assembliesToScan;

            var types = assToScan.SelectMany(x => x.GetAccessibleTypes());

            var interfaceTypesToScan = new[]
            {
                typeof(IService<>),
                typeof(IService<,>),
                typeof(IService<,,>),
                typeof(IService<,,,>),
                typeof(IRepository<>),
                typeof(IRepository<,>),
                typeof(IRepository<,,>),
                typeof(IRepository<,,,>),
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
                var implementationTypes = t.Where(x => x.IsGenericType == serviceType.IsGenericType &&
                                                       x.GetGenericArguments().Length == serviceType.GetGenericArguments().Length);

                foreach (var implementationType in implementationTypes)
                {
                    if (serviceType == typeof(IRepositoryInterceptor))
                    {
                        if (container.IsRegistered(implementationType) || optionsBuilder.Options.Interceptors.ContainsKey(implementationType))
                            continue;

                        container.RegisterType(implementationType, implementationType);
                        container.RegisterType(serviceType, implementationType, implementationType.FullName);
                        registeredInterceptorTypes.Add(implementationType);
                    }
                    else
                    {
                        container.RegisterType(serviceType, implementationType, new ContainerControlledTransientManager());
                    }
                }
            }

            // Register other services
            container.RegisterType<IRepositoryFactory>(new InjectionFactory((c, t, n) => new RepositoryFactory(c.Resolve<IRepositoryOptions>())));
            container.RegisterType<IUnitOfWork>(new InjectionFactory((c, t, n) => new UnitOfWork(c.Resolve<IRepositoryOptions>())));
            container.RegisterType<IUnitOfWorkFactory>(new InjectionFactory((c, t, n) => new UnitOfWorkFactory(c.Resolve<IRepositoryOptions>())));
            container.RegisterType<IRepositoryOptions>(new InjectionFactory((c, t, n) =>
            {
                var options = new RepositoryOptions(optionsBuilder.Options);

                foreach (var interceptorType in registeredInterceptorTypes)
                {
                    options.With(interceptorType, () => (IRepositoryInterceptor)c.Resolve(interceptorType));
                }

                return options;
            }));
        }

        /// <summary>
        /// Register all the repositories services using the specified options builder.
        /// </summary>
        /// <param name="container">The unity container.</param>
        /// <param name="optionsAction">A builder action used to create or modify options for the repositories.</param>
        /// <remarks>
        /// This method will scan for repositories and interceptors from the assemblies that have been loaded into the
        /// execution context of this application domain, and will register them to the service collection.
        /// </remarks>
        public static void RegisterRepositories(this IUnityContainer container, Action<RepositoryOptionsBuilder> optionsAction)
        {
            RegisterRepositories(container, optionsAction, AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}
