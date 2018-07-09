namespace DotNetToolkit.Repository.Json
{
    using InMemory;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Collections.Generic;
    using System.IO;

    internal class JsonContext<TEntity> : InMemoryFileContextBase<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonContext{TEntity}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public JsonContext(string path) : base(path, ".json") { }

        #endregion

        #region Implementation of InMemoryFileContextBase<TEntity>

        /// <summary>
        /// A protected overridable method for loading the entities from the specified stream reader.
        /// </summary>
        protected override IEnumerable<TEntity> OnLoaded(StreamReader reader)
        {
            var serializer = new JsonSerializer();
            var entities = (List<TEntity>)serializer.Deserialize(reader, typeof(List<TEntity>));

            return entities;
        }

        /// <summary>
        /// A protected overridable method for saving the entities to the specified stream writer.
        /// </summary>
        protected override void OnSaved(StreamWriter writer, IEnumerable<TEntity> entities)
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
