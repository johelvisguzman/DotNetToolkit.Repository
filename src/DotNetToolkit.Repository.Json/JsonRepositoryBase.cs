namespace DotNetToolkit.Repository.Json
{
    using InMemory;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// Represents a repository for storing entities as an json formatted file.
    /// </summary>
    public abstract class JsonRepositoryBase<TEntity, TKey> : InMemoryRepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private const string FileExtension = ".json";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        protected JsonRepositoryBase(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException(nameof(filePath));

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

            DatabaseName = fileName;

            // Creates the file if does not exist
            if (!File.Exists(DatabaseName))
            {
                File.Create(DatabaseName).Dispose();
            }
            // Otherwise, try to get the data from the file
            else
            {
                // Adds the data from the file into memory
                using (var stream = new FileStream(DatabaseName, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(stream))
                {
                    var serializer = new JsonSerializer();
                    var entities = (List<TEntity>)serializer.Deserialize(reader, typeof(List<TEntity>));

                    EnsureDeleted();

                    entities.ForEach(AddItem);

                    base.SaveChanges();
                }
            }
        }

        #endregion

        #region Overrides of InMemoryRepositoryBase<TEntity,TKey>

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            using (var stream = new FileStream(DatabaseName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Delete))
            {
                // Saves the data into memory
                base.SaveChanges();

                // Puts from memory into the file
                using (var writer = new StreamWriter(stream))
                {
                    var entities = GetQuery();
                    var serializer = new JsonSerializer
                    {
                        Formatting = Formatting.Indented,
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        TypeNameHandling = TypeNameHandling.All
                    };

                    serializer.Serialize(writer, entities);
                }
            }
        }

        #endregion
    }
}
