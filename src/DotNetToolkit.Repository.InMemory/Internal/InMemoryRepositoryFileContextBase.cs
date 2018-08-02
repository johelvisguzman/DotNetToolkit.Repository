namespace DotNetToolkit.Repository.InMemory.Internal
{
    using Configuration.Conventions;
    using Properties;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents an internal file based repository context for in-memory operations (for testing purposes).
    /// </summary>
    /// <seealso cref="InMemoryRepositoryContext" />
    internal abstract class InMemoryRepositoryFileContextBase : InMemoryRepositoryContext
    {
        #region Fields

        private readonly string _path;
        private bool _saveChangesInProcess;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        protected string FileExtension { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryRepositoryFileContextBase" /> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="ext">The file extension.</param>
        protected InMemoryRepositoryFileContextBase(string path, string ext)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (ext == null)
                throw new ArgumentNullException(nameof(ext));

            if (!IsFileExtensionValid(ext))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidFileExtension, ext));

            if (!string.IsNullOrEmpty(Path.GetExtension(path)))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.CannotBeFileName, path));

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (!path.EndsWith(@"\"))
                path += @"\";

            _path = path;
            FileExtension = ext;

            DatabaseName = _path;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// A protected overridable method for loading the entities from the specified stream reader.
        /// </summary>
        protected abstract IEnumerable<TEntity> OnLoaded<TEntity>(StreamReader reader) where TEntity : class;

        /// <summary>
        /// A protected overridable method for saving the entities to the specified stream writer.
        /// </summary>
        protected abstract void OnSaved<TEntity>(StreamWriter writer, IEnumerable<TEntity> entities) where TEntity : class;

        #endregion

        #region Private Methods

        private static bool IsFileExtensionValid(string ext)
        {
            var answer = true;

            if (!string.IsNullOrWhiteSpace(ext) && ext.Length > 1 && ext[0] == '.')
            {
                if (Path.GetInvalidFileNameChars().Any(ext.Contains))
                {
                    answer = false;
                }
            }

            return answer;
        }

        private void LoadChangesOnFileChanged<TEntity>() where TEntity : class
        {
            // Gets the name of the current file
            var fileName = GetFileName(typeof(TEntity));

            // Creates the file if does not exist
            if (!File.Exists(fileName))
                File.Create(fileName).Dispose();

            // Don't do anything if we are currently saving changes, or if the file is empty
            if (new FileInfo(fileName).Length == 0)
                return;

            // Checks to see when was the last time that the file was updated. If the TimeStamp and the lastWriteTime
            // are the same, then it means that the file has only been updated by this repository; otherwise,
            // something else updated it (maybe a manual edit). In which case, we need to re-upload the entities into memory
            var lastWriteTime = File.GetLastWriteTime(fileName);
            var timetamp = InMemoryCache.Instance.GetTimeStamp(fileName);
            if (!lastWriteTime.Equals(timetamp))
            {
                LoadChanges<TEntity>(fileName);
            }
        }

        private void LoadChanges<TEntity>(string fileName) where TEntity : class
        {
            // Adds the data from the file into memory
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                EnsureDeleted();

                var entities = OnLoaded<TEntity>(reader);
                if (entities == null)
                    return;

                foreach (var entity in entities)
                {
                    Add(entity);
                }

                base.SaveChanges();
            }
        }

        private object InvokeFindAll(Type type)
        {
            return GetType()
                .GetRuntimeMethods()
                .Single(x => x.Name == "FindAll" &&
                             x.IsGenericMethodDefinition &&
                             x.GetGenericArguments().Length == 1 &&
                             x.GetParameters().Length == 0)
                .MakeGenericMethod(type)
                .Invoke(this, null);
        }

        private void InvokeOnSaved(Type type, StreamWriter writer, object entities)
        {
            GetType()
                .GetRuntimeMethods()
                .Single(x => x.Name == "OnSaved" &&
                             x.IsGenericMethodDefinition &&
                             x.GetGenericArguments().Length == 1 &&
                             x.GetParameters().Length == 2)
                .MakeGenericMethod(type)
                .Invoke(this, new[] { writer, entities });
        }

        private string GetFileName(Type type)
        {
            return $"{_path}{type.GetTableName()}{FileExtension}";
        }

        #endregion

        #region Implementation of InMemoryRepositoryContext

        /// <summary>
        /// Returns the entity <see cref="T:System.Linq.IQueryable`1" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <returns>The entity <see cref="T:System.Linq.IQueryable`1" />.</returns>
        public override IQueryable<TEntity> AsQueryable<TEntity>()
        {
            if (!_saveChangesInProcess)
            {
                LoadChangesOnFileChanged<TEntity>();
            }

            return base.AsQueryable<TEntity>();
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public override int SaveChanges()
        {
            var types = ItemTypes.ToList();

            // Saves the data into memory
            _saveChangesInProcess = true;

            var count = base.SaveChanges();

            foreach (var type in types)
            {
                // Gets the name of the current file
                var fileName = GetFileName(type);

                using (var stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete))
                {
                    // Puts from memory into the file
                    using (var writer = new StreamWriter(stream))
                    {
                        // Gets the entities from memory
                        var entities = InvokeFindAll(type);

                        // Saves the entities
                        InvokeOnSaved(type, writer, entities);
                    }

                    // Keeps track of the last time the file was updated
                    InMemoryCache.Instance.SetTimeStamp(fileName, File.GetLastWriteTime(fileName));
                }
            }

            _saveChangesInProcess = false;

            return count;
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
            /// Prevents a default instance of the <see cref="InMemoryCache" /> class from being created.
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