namespace DotNetToolkit.Repository.InMemory.Internal
{
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    /// Represents an internal thread safe database storage which will store any information for the in-memory
    /// store that is needed through the life time of the application.
    /// </summary>
    internal class InMemoryDbStorage
    {
        #region Fields

        private static volatile InMemoryDbStorage _instance;
        private static readonly object _syncRoot = new object();
        private readonly ConcurrentDictionary<string, InMemoryDbContext> _storage;

        #endregion

        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="InMemoryDbStorage"/> class from being created.
        /// </summary>
        private InMemoryDbStorage()
        {
            _storage = new ConcurrentDictionary<string, InMemoryDbContext>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static InMemoryDbStorage Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new InMemoryDbStorage();
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
        public InMemoryDbContext GetScopedContext(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(Properties.Resources.ArgumentCannotBeNullOrEmpty, nameof(name));

            return _storage.GetOrAdd(name, new InMemoryDbContext());
        }

        #endregion
    }
}
