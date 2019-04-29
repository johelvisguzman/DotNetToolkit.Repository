namespace DotNetToolkit.Repository.InMemory.Internal
{
    using System;
    using System.Collections.Concurrent;
    using Utility;

    /// <summary>
    /// Represents an internal thread safe database storage which will store any information for the in-memory
    /// store that is needed through the life time of the application.
    /// </summary>
    internal class InMemoryCache
    {
        #region Fields

        private static volatile InMemoryCache _instance;
        private static readonly object _syncRoot = new object();
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>> _storage;

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="InMemoryCache" /> class from being created.
        /// </summary>
        private InMemoryCache()
        {
            _storage = new ConcurrentDictionary<string, ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static InMemoryCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new InMemoryCache();
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the scoped database context by the specified name.
        /// </summary>
        /// <param name="name">The database name.</param>
        /// <returns>The scoped database context by the specified database name.</returns>
        public ConcurrentDictionary<Type, ConcurrentDictionary<object, object>> GetDatabaseStore(string name)
        {
            Guard.NotEmpty(name);

            if (!_storage.ContainsKey(name))
            {
                _storage[name] = new ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>();
            }

            return _storage[name];
        }

        #endregion
    }
}
