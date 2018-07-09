namespace DotNetToolkit.Repository.Xml
{
    using InMemory;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    internal class XmlContext<TEntity> : InMemoryFileContextBase<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlContext{TEntity}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public XmlContext(string path) : base(path, ".xml") { }

        #endregion

        #region Implementation of InMemoryFileContextBase<TEntity>

        /// <summary>
        /// A protected overridable method for loading the entities from the specified stream reader.
        /// </summary>
        protected override IEnumerable<TEntity> OnLoaded(StreamReader reader)
        {
            var serializer = new XmlSerializer(typeof(List<TEntity>));
            var entities = (List<TEntity>)serializer.Deserialize(reader);

            return entities;
        }

        /// <summary>
        /// A protected overridable method for saving the entities to the specified stream writer.
        /// </summary>
        protected override void OnSaved(StreamWriter writer, IEnumerable<TEntity> entities)
        {
            var serializer = new XmlSerializer(typeof(List<TEntity>));

            serializer.Serialize(writer, entities);
        }

        #endregion
    }
}
