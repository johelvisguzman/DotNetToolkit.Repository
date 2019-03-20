namespace DotNetToolkit.Repository.Json.Internal
{
    using Configuration;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Represents an internal JSON repository context.
    /// </summary>
    /// <seealso cref="IRepositoryContext" />
    internal class JsonRepositoryContext : FileStreamRepositoryContextBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStreamRepositoryContextBase" /> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the context provider does not support transactions.</param>
        public JsonRepositoryContext(string path, bool ignoreTransactionWarning = false) : base(path, ".json", ignoreTransactionWarning) { }

        #endregion

        #region Overrides of FileStreamRepositoryContextBase

        /// <summary>
        /// A protected overridable method for loading the entities from the specified stream reader.
        /// </summary>
        protected override IEnumerable<TEntity> OnLoaded<TEntity>(StreamReader reader)
        {
            var serializer = new JsonSerializer();
            var entities = (List<TEntity>)serializer.Deserialize(reader, typeof(List<TEntity>));

            return entities;
        }

        /// <summary>
        /// A protected overridable method for saving the entities to the specified stream writer.
        /// </summary>
        protected override void OnSaved<TEntity>(StreamWriter writer, IEnumerable<TEntity> entities)
        {
            var serializer = new JsonSerializer
            {
                Formatting = Formatting.Indented,
                ContractResolver = new JsonDefaultSerializeContractResolver(),
                PreserveReferencesHandling = PreserveReferencesHandling.None
            };

            serializer.Serialize(writer, entities);
        }

        #endregion
    }
}
