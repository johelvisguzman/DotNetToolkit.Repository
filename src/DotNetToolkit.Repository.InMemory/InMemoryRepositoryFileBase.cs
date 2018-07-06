namespace DotNetToolkit.Repository.InMemory
{
    using FetchStrategies;
    using Helpers;
    using Interceptors;
    using Properties;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Represents a file based repository for in-memory operations with a composite primary key (for testing purposes).
    /// </summary>
    public abstract class InMemoryRepositoryFileBase<TEntity, TKey1, TKey2, TKey3> : InMemoryRepositoryBase<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Fields

        private bool _saveChangesInProcess;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        protected abstract string FileExtension { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryFileBase{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        protected InMemoryRepositoryFileBase(string path) : this(path, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryFileBase{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected InMemoryRepositoryFileBase(string path, IEnumerable<IRepositoryInterceptor> interceptors) : base(interceptors)
        {
            OnInitialize(path);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// A protected overridable method for loading the entities from the specified stream reader.
        /// </summary>
        protected abstract IEnumerable<TEntity> OnLoaded(StreamReader reader);

        /// <summary>
        /// A protected overridable method for saving the entities to the specified stream writer.
        /// </summary>
        protected abstract void OnSaved(StreamWriter writer, IEnumerable<TEntity> entities);

        #endregion

        #region Private Methods

        private void OnInitialize(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!string.IsNullOrEmpty(Path.GetExtension(path)))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CannotBeFileName, path));

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileName = path;

            if (!fileName.EndsWith(@"\"))
                fileName += @"\";

            fileName += $"{ConventionHelper.GetTableName<TEntity>()}{FileExtension}";

            Context.DatabaseName = fileName;

            // Creates the file if does not exist
            if (!File.Exists(Context.DatabaseName))
            {
                File.Create(Context.DatabaseName).Dispose();
            }
            // Otherwise, try to get the data from the file
            else
            {
                LoadChangesOnFileChanged();
            }
        }

        private void LoadChangesOnFileChanged()
        {
            // Don't do anything if we are currently saving changes, or if the file is empty
            if (new FileInfo(Context.DatabaseName).Length == 0)
                return;

            // Checks to see when was the last time that the file was updated. If the TimeStamp and the lastWriteTime
            // are the same, then it means that the file has only been updated by this repository; otherwise,
            // something else updated it (maybe a manual edit). In which case, we need to re-upload the entities into memory
            var lastWriteTime = File.GetLastWriteTime(Context.DatabaseName);
            var timetamp = InMemoryCache.Instance.GetTimeStamp(Context.DatabaseName);
            if (!lastWriteTime.Equals(timetamp))
            {
                LoadChanges();
            }
        }

        private void LoadChanges()
        {
            // Adds the data from the file into memory
            using (var stream = new FileStream(Context.DatabaseName, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                Context.EnsureDeleted();

                var entities = OnLoaded(reader);
                if (entities == null)
                    return;

                foreach (var entity in entities)
                {
                    AddItem(entity);
                }

                base.SaveChanges();
            }
        }

        #endregion

        #region Overrides of InMemoryRepositoryBase<TEntity, TKey1, TKey2>

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected override IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null)
        {
            if (!_saveChangesInProcess)
            {
                LoadChangesOnFileChanged();
            }

            return base.GetQuery(fetchStrategy);
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override TEntity GetEntity(object[] keyValues, IFetchStrategy<TEntity> fetchStrategy)
        {
            LoadChangesOnFileChanged();

            return base.GetEntity(keyValues, fetchStrategy);
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            _saveChangesInProcess = true;

            using (var stream = new FileStream(Context.DatabaseName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete))
            {
                // Saves the data into memory
                base.SaveChanges();

                // Puts from memory into the file
                using (var writer = new StreamWriter(stream))
                {
                    var entities = base.GetQuery().ToList();

                    OnSaved(writer, entities);
                }

                InMemoryCache.Instance.SetTimeStamp(Context.DatabaseName, File.GetLastWriteTime(Context.DatabaseName));
            }

            _saveChangesInProcess = false;
        }

        #endregion

        #region Nested type: InMemoryCache

        /// <summary>
        /// Represents an internal thread safe database storage which will store any information for the in-memory
        /// store that is needed through the life time of the application.
        /// </summary>
        private class InMemoryCache
        {
            #region Fields

            private static volatile InMemoryCache _instance;
            private static readonly object _syncRoot = new object();
            private readonly ConcurrentDictionary<string, DateTime> _timestamp;

            #endregion

            #region Constructors

            /// <summary>
            /// Prevents a default instance of the <see cref="InMemoryCache"/> class from being created.
            /// </summary>
            private InMemoryCache()
            {
                _timestamp = new ConcurrentDictionary<string, DateTime>();
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
            /// Sets the time stamp.
            /// </summary>
            /// <param name="name">The database name.</param>
            /// <param name="time">The time.</param>
            public void SetTimeStamp(string name, DateTime time)
            {
                _timestamp[name] = time;
            }

            /// <summary>
            /// Gets the time stamp.
            /// </summary>
            /// <param name="name">The database name.</param>
            /// <returns>The time stamp.</returns>
            public DateTime GetTimeStamp(string name)
            {
                _timestamp.TryGetValue(name, out DateTime time);

                return time;
            }

            #endregion
        }

        #endregion
    }

    /// <summary>
    /// Represents a file based repository for in-memory operations with a composite primary key (for testing purposes).
    /// </summary>
    public abstract class InMemoryRepositoryFileBase<TEntity, TKey1, TKey2> : InMemoryRepositoryBase<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Fields

        private bool _saveChangesInProcess;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        protected abstract string FileExtension { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryFileBase{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        protected InMemoryRepositoryFileBase(string path) : this(path, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryFileBase{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected InMemoryRepositoryFileBase(string path, IEnumerable<IRepositoryInterceptor> interceptors) : base(interceptors)
        {
            OnInitialize(path);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// A protected overridable method for loading the entities from the specified stream reader.
        /// </summary>
        protected abstract IEnumerable<TEntity> OnLoaded(StreamReader reader);

        /// <summary>
        /// A protected overridable method for saving the entities to the specified stream writer.
        /// </summary>
        protected abstract void OnSaved(StreamWriter writer, IEnumerable<TEntity> entities);

        #endregion

        #region Private Methods

        private void OnInitialize(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!string.IsNullOrEmpty(Path.GetExtension(path)))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CannotBeFileName, path));

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileName = path;

            if (!fileName.EndsWith(@"\"))
                fileName += @"\";

            fileName += $"{ConventionHelper.GetTableName<TEntity>()}{FileExtension}";

            Context.DatabaseName = fileName;

            // Creates the file if does not exist
            if (!File.Exists(Context.DatabaseName))
            {
                File.Create(Context.DatabaseName).Dispose();
            }
            // Otherwise, try to get the data from the file
            else
            {
                LoadChangesOnFileChanged();
            }
        }

        private void LoadChangesOnFileChanged()
        {
            // Don't do anything if we are currently saving changes, or if the file is empty
            if (new FileInfo(Context.DatabaseName).Length == 0)
                return;

            // Checks to see when was the last time that the file was updated. If the TimeStamp and the lastWriteTime
            // are the same, then it means that the file has only been updated by this repository; otherwise,
            // something else updated it (maybe a manual edit). In which case, we need to re-upload the entities into memory
            var lastWriteTime = File.GetLastWriteTime(Context.DatabaseName);
            var timetamp = InMemoryCache.Instance.GetTimeStamp(Context.DatabaseName);
            if (!lastWriteTime.Equals(timetamp))
            {
                LoadChanges();
            }
        }

        private void LoadChanges()
        {
            // Adds the data from the file into memory
            using (var stream = new FileStream(Context.DatabaseName, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                Context.EnsureDeleted();

                var entities = OnLoaded(reader);
                if (entities == null)
                    return;

                foreach (var entity in entities)
                {
                    AddItem(entity);
                }

                base.SaveChanges();
            }
        }

        #endregion

        #region Overrides of InMemoryRepositoryBase<TEntity, TKey1, TKey2>

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected override IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null)
        {
            if (!_saveChangesInProcess)
            {
                LoadChangesOnFileChanged();
            }

            return base.GetQuery(fetchStrategy);
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override TEntity GetEntity(object[] keyValues, IFetchStrategy<TEntity> fetchStrategy)
        {
            LoadChangesOnFileChanged();

            return base.GetEntity(keyValues, fetchStrategy);
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            _saveChangesInProcess = true;

            using (var stream = new FileStream(Context.DatabaseName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete))
            {
                // Saves the data into memory
                base.SaveChanges();

                // Puts from memory into the file
                using (var writer = new StreamWriter(stream))
                {
                    var entities = base.GetQuery().ToList();

                    OnSaved(writer, entities);
                }

                InMemoryCache.Instance.SetTimeStamp(Context.DatabaseName, File.GetLastWriteTime(Context.DatabaseName));
            }

            _saveChangesInProcess = false;
        }

        #endregion

        #region Nested type: InMemoryCache

        /// <summary>
        /// Represents an internal thread safe database storage which will store any information for the in-memory
        /// store that is needed through the life time of the application.
        /// </summary>
        private class InMemoryCache
        {
            #region Fields

            private static volatile InMemoryCache _instance;
            private static readonly object _syncRoot = new object();
            private readonly ConcurrentDictionary<string, DateTime> _timestamp;

            #endregion

            #region Constructors

            /// <summary>
            /// Prevents a default instance of the <see cref="InMemoryCache"/> class from being created.
            /// </summary>
            private InMemoryCache()
            {
                _timestamp = new ConcurrentDictionary<string, DateTime>();
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
            /// Sets the time stamp.
            /// </summary>
            /// <param name="name">The database name.</param>
            /// <param name="time">The time.</param>
            public void SetTimeStamp(string name, DateTime time)
            {
                _timestamp[name] = time;
            }

            /// <summary>
            /// Gets the time stamp.
            /// </summary>
            /// <param name="name">The database name.</param>
            /// <returns>The time stamp.</returns>
            public DateTime GetTimeStamp(string name)
            {
                _timestamp.TryGetValue(name, out DateTime time);

                return time;
            }

            #endregion
        }

        #endregion
    }

    /// <summary>
    /// Represents a file based repository for in-memory operations (for testing purposes).
    /// </summary>
    public abstract class InMemoryRepositoryFileBase<TEntity, TKey> : InMemoryRepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private bool _saveChangesInProcess;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        protected abstract string FileExtension { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryFileBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        protected InMemoryRepositoryFileBase(string path) : this(path, null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryFileBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected InMemoryRepositoryFileBase(string path, IEnumerable<IRepositoryInterceptor> interceptors) : base(interceptors)
        {
            OnInitialize(path);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// A protected overridable method for loading the entities from the specified stream reader.
        /// </summary>
        protected abstract IEnumerable<TEntity> OnLoaded(StreamReader reader);

        /// <summary>
        /// A protected overridable method for saving the entities to the specified stream writer.
        /// </summary>
        protected abstract void OnSaved(StreamWriter writer, IEnumerable<TEntity> entities);

        #endregion

        #region Private Methods

        private void OnInitialize(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!string.IsNullOrEmpty(Path.GetExtension(path)))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CannotBeFileName, path));

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileName = path;

            if (!fileName.EndsWith(@"\"))
                fileName += @"\";

            fileName += $"{ConventionHelper.GetTableName<TEntity>()}{FileExtension}";

            Context.DatabaseName = fileName;

            // Creates the file if does not exist
            if (!File.Exists(Context.DatabaseName))
            {
                File.Create(Context.DatabaseName).Dispose();
            }
            // Otherwise, try to get the data from the file
            else
            {
                LoadChangesOnFileChanged();
            }
        }

        private void LoadChangesOnFileChanged()
        {
            // Don't do anything if we are currently saving changes, or if the file is empty
            if (new FileInfo(Context.DatabaseName).Length == 0)
                return;

            // Checks to see when was the last time that the file was updated. If the TimeStamp and the lastWriteTime
            // are the same, then it means that the file has only been updated by this repository; otherwise,
            // something else updated it (maybe a manual edit). In which case, we need to re-upload the entities into memory
            var lastWriteTime = File.GetLastWriteTime(Context.DatabaseName);
            var timetamp = InMemoryCache.Instance.GetTimeStamp(Context.DatabaseName);
            if (!lastWriteTime.Equals(timetamp))
            {
                LoadChanges();
            }
        }

        private void LoadChanges()
        {
            // Adds the data from the file into memory
            using (var stream = new FileStream(Context.DatabaseName, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                Context.EnsureDeleted();

                var entities = OnLoaded(reader);
                if (entities == null)
                    return;

                foreach (var entity in entities)
                {
                    AddItem(entity);
                }

                base.SaveChanges();
            }
        }

        #endregion

        #region Overrides of InMemoryRepositoryBase<TEntity, TKey>

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected override IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null)
        {
            if (!_saveChangesInProcess)
            {
                LoadChangesOnFileChanged();
            }

            return base.GetQuery(fetchStrategy);
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override TEntity GetEntity(object[] keyValues, IFetchStrategy<TEntity> fetchStrategy)
        {
            LoadChangesOnFileChanged();

            return base.GetEntity(keyValues, fetchStrategy);
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            _saveChangesInProcess = true;

            using (var stream = new FileStream(Context.DatabaseName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete))
            {
                // Saves the data into memory
                base.SaveChanges();

                // Puts from memory into the file
                using (var writer = new StreamWriter(stream))
                {
                    var entities = base.GetQuery().ToList();

                    OnSaved(writer, entities);
                }

                InMemoryCache.Instance.SetTimeStamp(Context.DatabaseName, File.GetLastWriteTime(Context.DatabaseName));
            }

            _saveChangesInProcess = false;
        }

        #endregion

        #region Nested type: InMemoryCache

        /// <summary>
        /// Represents an internal thread safe database storage which will store any information for the in-memory
        /// store that is needed through the life time of the application.
        /// </summary>
        private class InMemoryCache
        {
            #region Fields

            private static volatile InMemoryCache _instance;
            private static readonly object _syncRoot = new object();
            private readonly ConcurrentDictionary<string, DateTime> _timestamp;

            #endregion

            #region Constructors

            /// <summary>
            /// Prevents a default instance of the <see cref="InMemoryCache"/> class from being created.
            /// </summary>
            private InMemoryCache()
            {
                _timestamp = new ConcurrentDictionary<string, DateTime>();
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
            /// Sets the time stamp.
            /// </summary>
            /// <param name="name">The database name.</param>
            /// <param name="time">The time.</param>
            public void SetTimeStamp(string name, DateTime time)
            {
                _timestamp[name] = time;
            }

            /// <summary>
            /// Gets the time stamp.
            /// </summary>
            /// <param name="name">The database name.</param>
            /// <returns>The time stamp.</returns>
            public DateTime GetTimeStamp(string name)
            {
                _timestamp.TryGetValue(name, out DateTime time);

                return time;
            }

            #endregion
        }

        #endregion
    }
}
