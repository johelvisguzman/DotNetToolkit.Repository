namespace DotNetToolkit.Repository.InMemory.Internal
{
    using System.Collections.Concurrent;
    using Utility;

    internal class InMemoryDatabaseStoreCache
    {
        #region Fields

        private static volatile InMemoryDatabaseStoreCache _instance;
        private static readonly object _syncRoot = new object();
        private readonly ConcurrentDictionary<string, InMemoryDatabase> _dbs;

        #endregion

        #region Constructors

        private InMemoryDatabaseStoreCache()
        {
            _dbs = new ConcurrentDictionary<string, InMemoryDatabase>();
        }

        #endregion

        #region Properties

        public static InMemoryDatabaseStoreCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                            _instance = new InMemoryDatabaseStoreCache();
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