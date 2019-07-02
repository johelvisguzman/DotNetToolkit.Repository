namespace DotNetToolkit.Repository.Xml.Internal
{
    using Extensions;
    using Extensions.Internal;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// An implementation of <see cref="IXmlRepositoryContext" />.
    /// </summary>
    /// <seealso cref="IXmlRepositoryContext" />
    internal class XmlRepositoryContext : FileStreamRepositoryContextBase, IXmlRepositoryContext
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepositoryContext" /> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the context provider does not support transactions.</param>
        /// <param name="ignoreSqlQueryWarning">If a SQL query is executed, ignore any warnings since the in-memory provider does not support SQL query execution.</param>
        public XmlRepositoryContext(string path, bool ignoreTransactionWarning = false, bool ignoreSqlQueryWarning = false) : base(path, ".xml", ignoreTransactionWarning, ignoreSqlQueryWarning) { }

        #endregion

        #region Private Methods

        private static XmlAttributeOverrides GetXmlAttributeOverrides<TEntity>()
        {
            var overrides = new XmlAttributeOverrides();
            var ignore = new XmlAttributes { XmlIgnore = true };

            var type = typeof(TEntity);

            var properties = type.GetRuntimeProperties().Where(x => x.IsComplex());

            foreach (var propInfo in properties)
            {
                overrides.Add(type, propInfo.Name, ignore);
            }

            return overrides;
        }

        private static XmlSerializer GetSerializer<TEntity>()
        {
            var overrides = GetXmlAttributeOverrides<TEntity>();
            var serializer = new XmlSerializer(typeof(List<TEntity>), overrides);

            return serializer;
        }

        #endregion

        #region Overrides of FileStreamRepositoryContextBase

        /// <summary>
        /// A protected overridable method for loading the entities from the specified stream reader.
        /// </summary>
        protected override IEnumerable<TEntity> OnLoaded<TEntity>(StreamReader reader)
        {
            using (var xmlReader = XmlReader.Create(reader))
            {
                var serializer = GetSerializer<TEntity>();

                if (serializer.CanDeserialize(xmlReader))
                    return (List<TEntity>)serializer.Deserialize(xmlReader);

                Logger.Debug("Unable deserialize file.");

                return Enumerable.Empty<TEntity>();
            }
        }

        /// <summary>
        /// A protected overridable method for saving the entities to the specified stream writer.
        /// </summary>
        protected override void OnSaved<TEntity>(StreamWriter writer, IEnumerable<TEntity> entities)
        {
            var serializer = GetSerializer<TEntity>();

            serializer.Serialize(writer, entities);
        }

        #endregion
    }
}
