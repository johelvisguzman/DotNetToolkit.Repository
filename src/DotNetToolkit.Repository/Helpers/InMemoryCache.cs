namespace DotNetToolkit.Repository.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;

    internal class InMemoryCache
    {
        private static volatile InMemoryCache _instance;
        private static readonly object _syncRoot = new object();

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

        public ConcurrentDictionary<Type, PropertyInfo> PrimaryKeyMapping { get; }
        public ConcurrentDictionary<Tuple<Type, Type>, PropertyInfo> ForeignKeyMapping { get; }
        public ConcurrentDictionary<Type, string> TableNameMapping { get; }

        private InMemoryCache()
        {
            PrimaryKeyMapping = new ConcurrentDictionary<Type, PropertyInfo>();
            ForeignKeyMapping = new ConcurrentDictionary<Tuple<Type, Type>, PropertyInfo>();
            TableNameMapping = new ConcurrentDictionary<Type, string>();
        }
    }
}
