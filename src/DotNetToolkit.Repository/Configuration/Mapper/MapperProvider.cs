namespace DotNetToolkit.Repository.Configuration.Mapper
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="IMapperProvider" />.
    /// </summary>
    public class MapperProvider : IMapperProvider
    {
        private static volatile MapperProvider _instance;
        private static readonly object _syncRoot = new object();
        private readonly Dictionary<Type, IMapper<object>> _mappings = new Dictionary<Type, IMapper<object>>();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        internal static MapperProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new MapperProvider();
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Creates a mapper for the specified entity type.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <returns>The <see cref="IMapper{T}"/>.</returns>
        public IMapper<T> Create<T>() where T : class
        {
            if (_mappings.ContainsKey(typeof(T)))
                return (IMapper<T>)_mappings[typeof(T)];

            return new DefaultMapper<T>();
        }

        /// <summary>
        /// Registers a mapper for the specified entity type.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="mapper">The mapper to register.</param>
        public MapperProvider Register<T>(IMapper<T> mapper) where T : class
        {
            if (mapper == null)
                throw new ArgumentNullException(nameof(mapper));

            if (_mappings.ContainsKey(typeof(T)))
                _mappings[typeof(T)] = mapper;
            else
                _mappings.Add(typeof(T), mapper);

            return this;
        }
    }
}
