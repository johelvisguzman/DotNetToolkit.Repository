namespace DotNetToolkit.Repository.Utility
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Reflection;

    internal class InMemoryCache
    {
        #region Fields

        private static volatile InMemoryCache _instance;
        private static readonly object _syncRoot = new object();

        #endregion

        #region Properties

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

        public ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> PrimaryKeyMapping { get; }
        public ConcurrentDictionary<Tuple<Type, Type>, IEnumerable<PropertyInfo>> ForeignKeyMapping { get; }
        public ConcurrentDictionary<Type, string> TableNameMapping { get; }

        #endregion

        #region Constructors

        private InMemoryCache()
        {
            PrimaryKeyMapping = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();
            ForeignKeyMapping = new ConcurrentDictionary<Tuple<Type, Type>, IEnumerable<PropertyInfo>>();
            TableNameMapping = new ConcurrentDictionary<Type, string>();
        }

        #endregion
    }
}
