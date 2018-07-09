namespace DotNetToolkit.Repository.InMemory
{
    using Helpers;
    using Properties;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Represents an internal repository context for file based repositories.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.InMemory.InMemoryContext" />
    internal abstract class InMemoryFileContextBase<TEntity> : InMemoryContext where TEntity : class
    {
        #region Fields

        private bool _saveChangesInProcess;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryFileContextBase{TEntity}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="ext">The file extension.</param>
        protected InMemoryFileContextBase(string path, string ext)
        {
            OnInitialize(path, ext);
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

        private void OnInitialize(string path, string ext)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (ext == null)
                throw new ArgumentNullException(nameof(ext));

            if (!string.IsNullOrEmpty(Path.GetExtension(path)))
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.CannotBeFileName, path));

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileName = path;

            if (!fileName.EndsWith(@"\"))
                fileName += @"\";

            fileName += $"{ConventionHelper.GetTableName<TEntity>()}{ext}";

            DatabaseName = fileName;

            // Creates the file if does not exist
            if (!File.Exists(DatabaseName))
            {
                File.Create(DatabaseName).Dispose();
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
            if (new FileInfo(DatabaseName).Length == 0)
                return;

            // Checks to see when was the last time that the file was updated. If the TimeStamp and the lastWriteTime
            // are the same, then it means that the file has only been updated by this repository; otherwise,
            // something else updated it (maybe a manual edit). In which case, we need to re-upload the entities into memory
            var lastWriteTime = File.GetLastWriteTime(DatabaseName);
            var timetamp = InMemoryCache.Instance.GetTimeStamp(DatabaseName);
            if (!lastWriteTime.Equals(timetamp))
            {
                LoadChanges();
            }
        }

        private void LoadChanges()
        {
            // Adds the data from the file into memory
            using (var stream = new FileStream(DatabaseName, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                EnsureDeleted();

                var entities = OnLoaded(reader);
                if (entities == null)
                    return;

                foreach (var entity in entities)
                {
                    Add(entity);
                }

                base.SaveChanges();
            }
        }

        #endregion

        #region Implementation of InMemoryContext

        /// <summary>
        /// Returns the entity <see cref="T:System.Linq.IQueryable`1" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <returns>The entity <see cref="T:System.Linq.IQueryable`1" />.</returns>
        public override IQueryable<TEntity> AsQueryable<TEntity>()
        {
            if (!_saveChangesInProcess)
            {
                LoadChangesOnFileChanged();
            }

            return base.AsQueryable<TEntity>();
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public override int SaveChanges()
        {
            using (var stream = new FileStream(DatabaseName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete))
            {
                // Saves the data into memory
                _saveChangesInProcess = true;

                var count = base.SaveChanges();

                _saveChangesInProcess = false;

                // Puts from memory into the file
                using (var writer = new StreamWriter(stream))
                {
                    var entities = AsQueryable<TEntity>().ToList();

                    OnSaved(writer, entities);
                }

                InMemoryCache.Instance.SetTimeStamp(DatabaseName, File.GetLastWriteTime(DatabaseName));

                return count;
            }
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
