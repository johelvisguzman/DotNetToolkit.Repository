namespace DotNetToolkit.Repository.Csv
{
    using CsvHelper;
    using InMemory;
    using System.Collections.Generic;
    using System.IO;

    internal class CsvContext<TEntity> : InMemoryFileContextBase<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvContext{TEntity}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public CsvContext(string path) : base(path, ".csv") { }

        #endregion

        #region Implementation of InMemoryFileContextBase<TEntity>

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
