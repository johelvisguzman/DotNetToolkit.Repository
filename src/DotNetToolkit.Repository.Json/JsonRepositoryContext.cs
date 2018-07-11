namespace DotNetToolkit.Repository.Json
{
    using InMemory;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Represents a json repository context.
    /// </summary>
    /// <seealso cref="InMemoryRepositoryFileContextBase" />
    public class JsonRepositoryContext : InMemoryRepositoryFileContextBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepositoryContext" /> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public JsonRepositoryContext(string path) : base(path, ".json") { }

        #endregion

        #region Implementation of InMemoryRepositoryFileContextBase

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
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            serializer.Serialize(writer, entities);
        }

        #endregion
    }
}
