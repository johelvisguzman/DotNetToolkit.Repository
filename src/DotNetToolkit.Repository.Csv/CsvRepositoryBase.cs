namespace DotNetToolkit.Repository.Csv
{
    using CsvHelper;
    using InMemory;
    using Interceptors;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Represents a repository for storing entities as an csv formatted file.
    /// </summary>
    public abstract class CsvRepositoryBase<TEntity, TKey> : InMemoryRepositoryFileBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        protected CsvRepositoryBase(string path) : base(path) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected CsvRepositoryBase(string path, IEnumerable<IRepositoryInterceptor> interceptors) : base(path, interceptors) { }

        #endregion

        #region Overrides of InMemoryRepositoryFileBase<TEntity, TKey>

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        protected override string FileExtension { get; } = ".csv";

        /// <summary>
        /// A protected overridable method for loading the entities from the specified stream reader.
        /// </summary>
        protected override IEnumerable<TEntity> OnLoaded(StreamReader reader)
        {
            var csv = new CsvReader(reader);
            var entities = csv.GetRecords<TEntity>();

            return entities;
        }

        /// <summary>
        /// A protected overridable method for saving the entities to the specified stream writer.
        /// </summary>
        protected override void OnSaved(StreamWriter writer, IEnumerable<TEntity> entities)
        {
            var csv = new CsvWriter(writer);

            csv.WriteRecords(entities);
        }

        #endregion
    }
}
