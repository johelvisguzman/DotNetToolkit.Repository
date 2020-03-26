namespace DotNetToolkit.Repository.Utility
{
    using Configuration.Interceptors;
    using Configuration.Logging;
    using Extensions.Internal;
    using JetBrains.Annotations;
    using Services;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Transactions;

    /// <summary>
    /// A helper class for scanning all repositories and interceptors from a specified assembly collection.
    /// </summary>
    public class AssemblyScanner : IEnumerable<AssemblyScanner.AssemblyScanResult>
    {
        #region Fields

        private readonly IEnumerable<Type> _types;
        private readonly Type[] _interfaceTypesToScan;

        #endregion

        #region Constructors

        /// <summary>
		/// Creates a scanner that works on a sequence of types.
		/// </summary>
        private AssemblyScanner([NotNull] IEnumerable<Type> types)
        {
            _types = Guard.NotNull(types, nameof(types));
            _interfaceTypesToScan = new[]
            {
                typeof(IService<>),
                typeof(IService<,>),
                typeof(IService<,,>),
                typeof(IService<,,,>),
                typeof(IRepository<>),
                typeof(IRepository<,>),
                typeof(IRepository<,,>),
                typeof(IRepository<,,,>),
                typeof(IReadOnlyService<>),
                typeof(IReadOnlyService<,>),
                typeof(IReadOnlyService<,,>),
                typeof(IReadOnlyService<,,,>),
                typeof(IReadOnlyRepository<>),
                typeof(IReadOnlyRepository<,>),
                typeof(IReadOnlyRepository<,,>),
                typeof(IReadOnlyRepository<,,,>),
                typeof(IRepositoryInterceptor),
                typeof(ILoggerProvider),
                typeof(IRepositoryFactory),
                typeof(IServiceFactory),
                typeof(IUnitOfWork),
                typeof(IUnitOfWorkFactory),
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Scans for repositories and interceptors from the specified assembly.
        /// </summary>
        public static AssemblyScanner FindRepositoriesFromAssembly([NotNull] Assembly assemblyToScan)
        {
            return FindRepositoriesFromAssemblies(Guard.NotNull(assemblyToScan, nameof(assemblyToScan)));
        }

        /// <summary>
        /// Scans for repositories and interceptors from the specified assembly collection.
        /// </summary>
        public static AssemblyScanner FindRepositoriesFromAssemblies([NotNull] params Assembly[] assembliesToScan)
        {
            Guard.NotNull(assembliesToScan, nameof(assembliesToScan));

            var baseAssembly = typeof(AssemblyScanner).GetTypeInfo().Assembly;
            var types = assembliesToScan.Concat(new[] { baseAssembly })
                .SelectMany(x => x.GetAccessibleTypes().Distinct());

            return new AssemblyScanner(types);
        }

        /// <summary>
		/// Performs the specified action to all of the assembly scan results.
		/// </summary>
		public void ForEach([NotNull] Action<AssemblyScanResult> action)
        {
            Guard.NotNull(action, nameof(action));

            foreach (var result in this)
            {
                action(result);
            }
        }

        /// <summary>
        /// Gets a collection of types that implement the specified interface type.
        /// </summary>
        /// <typeparam name="T">The interface type to get the implementations from.</typeparam>
        /// <returns>A collection of types that implement the specified interface type.</returns>
        public IEnumerable<Type> OfType<T>()
        {
            return this
                .Where(result => result.InterfaceType == typeof(T))
                .SelectMany(result => result.ImplementationTypes);
        }

        #endregion

        #region Private Methods

        private IEnumerable<AssemblyScanResult> ScanResults()
        {
            var interfaceTypes = _interfaceTypesToScan
                .SelectMany(interfaceType => _types.Where(x => !x.GetTypeInfo().IsClass && x.ImplementsInterface(interfaceType)))
                .Distinct();

            var typesMapping = interfaceTypes
                .SelectMany(interfaceType => _types.Where(x => x.GetTypeInfo().IsClass && !x.GetTypeInfo().IsAbstract && x.ImplementsInterface(interfaceType))
                .GroupBy(x => interfaceType));

            var query = from mapping in typesMapping
                        let interfaceType = mapping.Key
                        let implementationTypes = mapping.Where(x => x.GetTypeInfo().IsGenericType == interfaceType.GetTypeInfo().IsGenericType &&
                                                                x.GetGenericArguments().Length == interfaceType.GetGenericArguments().Length &&
                                                                x.GetTypeInfo().IsVisible && !x.GetTypeInfo().IsAbstract)
                        select new AssemblyScanResult(interfaceType, implementationTypes);

            return query;
        }

        #endregion

        #region Implementations of IEnumerable<AssemblyScanner.AssemblyScanResult>

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.</returns>
        public IEnumerator<AssemblyScanResult> GetEnumerator()
        {
            return ScanResults().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.IEnumerator"/> that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Nested type: AssemblyScanResult

        /// <summary>
        /// Result of performing a scan.
        /// </summary>
        public class AssemblyScanResult
        {
            internal AssemblyScanResult(Type interfaceType, IEnumerable<Type> implmentationTypes)
            {
                InterfaceType = interfaceType;
                ImplementationTypes = implmentationTypes;
            }

            /// <summary>
            /// Gets the interface type.
            /// </summary>
            public Type InterfaceType { get; private set; }

            /// <summary>
            /// Gets the concrete types that implements the <see cref="InterfaceType"/>.
            /// </summary>
            public IEnumerable<Type> ImplementationTypes { get; private set; }
        }

        #endregion
    }
}
