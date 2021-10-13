namespace DotNetToolkit.Repository.InMemory.Internal
{
    using System.Collections.Concurrent;
    using Utility;

    internal class InMemoryDatabasesCache
    {
        #region Fields

        private static volatile InMemoryDatabasesCache _instance;
        private static readonly object _syncRoot = new object();
        private readonly ConcurrentDictionary<string, InMemoryDatabase> _dbs;

        #endregion

        #region Constructors

        private InMemoryDatabasesCache()
        {
            _dbs = new ConcurrentDictionary<string, InMemoryDatabase>();
        }

        #endregion

        #region Properties

        public static InMemoryDatabasesCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new InMemoryDatabasesCache();
                    }
                }

                return _instance;
            }
        }

        #endregion

        #region Public Methods

        public InMemoryDatabase GetDatabase(string name)
        {
            return _dbs.GetOrAdd(Guard.NotEmpty(name, nameof(name)), InMemoryDatabase.Empty());
        }

        #endregion
    }
}