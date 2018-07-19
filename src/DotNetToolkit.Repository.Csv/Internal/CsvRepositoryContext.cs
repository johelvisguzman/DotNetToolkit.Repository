namespace DotNetToolkit.Repository.Csv.Internal
{
    using CsvHelper;
    using InMemory.Internal;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Represents an internal csv repository context.
    /// </summary>
    /// <seealso cref="InMemoryRepositoryFileContextBase" />
    internal class CsvRepositoryContext : InMemoryRepositoryFileContextBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepositoryContext" /> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public CsvRepositoryContext(string path) : base(path, ".csv") { }

        #endregion

        #region Implementation of InMemoryRepositoryFileContextBase

        /// <summary>
        /// A protected overridable method for loading the entities from the specified stream reader.
        /// </summary>
        protected override IEnumerable<TEntity> OnLoaded<TEntity>(StreamReader reader)
        {
            var csv = new CsvReader(reader);
            var entities = csv.GetRecords<TEntity>();

            return entities;
        }

        /// <summary>
        /// A protected overridable method for saving the entities to the specified stream writer.
        /// </summary>
        protected override void OnSaved<TEntity>(StreamWriter writer, IEnumerable<TEntity> entities)
        {
            var csv = new CsvWriter(writer);

            csv.WriteRecords(entities);
        }

        #endregion
    }
}
