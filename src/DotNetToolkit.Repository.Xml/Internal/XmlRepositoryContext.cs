namespace DotNetToolkit.Repository.Xml.Internal
{
    using Configuration;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents an internal XML repository context.
    /// </summary>
    /// <seealso cref="IRepositoryContext" />
    internal class XmlRepositoryContext : FileStreamRepositoryContextBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStreamRepositoryContextBase" /> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the context provider does not support transactions.</param>
        public XmlRepositoryContext(string path, bool ignoreTransactionWarning = false) : base(path, ".xml", ignoreTransactionWarning) { }

        #endregion

        #region Overrides of FileStreamRepositoryContextBase

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
