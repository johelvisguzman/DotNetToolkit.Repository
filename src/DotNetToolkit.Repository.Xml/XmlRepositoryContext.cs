namespace DotNetToolkit.Repository.Xml
{
    using InMemory;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents a xml repository context.
    /// </summary>
    /// <seealso cref="InMemoryRepositoryFileContextBase" />
    public class XmlRepositoryContext : InMemoryRepositoryFileContextBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepositoryContext" /> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public XmlRepositoryContext(string path) : base(path, ".xml") { }

        #endregion

        #region Implementation of InMemoryRepositoryFileContextBase

        /// <summary>
        /// A protected overridable method for loading the entities from the specified stream reader.
        /// </summary>
        protected override IEnumerable<TEntity> OnLoaded<TEntity>(StreamReader reader)
        {
            var serializer = new XmlSerializer(typeof(List<TEntity>));
            var entities = (List<TEntity>)serializer.Deserialize(reader);

            return entities;
        }

        /// <summary>
        /// A protected overridable method for saving the entities to the specified stream writer.
        /// </summary>
        protected override void OnSaved<TEntity>(StreamWriter writer, IEnumerable<TEntity> entities)
        {
            var serializer = new XmlSerializer(typeof(List<TEntity>));

            serializer.Serialize(writer, entities);
        }

        #endregion
    }
}
