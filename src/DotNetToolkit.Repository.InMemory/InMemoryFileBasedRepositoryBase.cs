namespace DotNetToolkit.Repository.InMemory
{
    using FetchStrategies;
    using Logging;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Represents a file based repository for in-memory operations (for testing purposes).
    /// </summary>
    public abstract class InMemoryFileBasedRepositoryBase<TEntity, TKey> : InMemoryRepositoryBase<TEntity, TKey> where TEntity : class
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
        /// Initializes a new instance of the <see cref="InMemoryFileBasedRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        protected InMemoryFileBasedRepositoryBase(string filePath) : this(filePath, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryFileBasedRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="logger">The logger.</param>
        protected InMemoryFileBasedRepositoryBase(string filePath, ILogger logger) : base(logger)
        {
            OnInitialize(filePath);
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

        private string ValidateFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmpty, nameof(filePath));

            // Ensures we have a valid file
            var fileName = filePath;

            if (Directory.Exists(filePath))
            {
                if (!fileName.EndsWith(@"\"))
                    fileName += @"\";

                fileName += $"{GetType().Name}{FileExtension}";
            }
            else
            {
                if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
                    fileName += FileExtension;

                if (!Path.GetExtension(fileName).Equals(FileExtension))
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidFileExtension, fileName, FileExtension));

                if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) < 0)
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidFilePath, fileName));
            }

            return fileName;
        }

        private void OnInitialize(string filePath)
        {
            DatabaseName = ValidateFile(filePath);

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
            var timetamp = GetTimeStamp();
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
                    AddItem(entity);
                }

                base.SaveChanges();
            }
        }

        #endregion

        #region Overrides of InMemoryRepositoryBase<TEntity,TKey>

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
        protected override TEntity GetEntity(TKey key, IFetchStrategy<TEntity> fetchStrategy)
        {
            LoadChangesOnFileChanged();

            return base.GetEntity(key, fetchStrategy);
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            _saveChangesInProcess = true;

            using (var stream = new FileStream(DatabaseName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete))
            {
                // Saves the data into memory
                base.SaveChanges();

                // Puts from memory into the file
                using (var writer = new StreamWriter(stream))
                {
                    var entities = base.GetQuery().ToList();

                    OnSaved(writer, entities);
                }

                SetTimeStamp(File.GetLastWriteTime(DatabaseName));
            }

            _saveChangesInProcess = false;
        }

        #endregion
    }
}
