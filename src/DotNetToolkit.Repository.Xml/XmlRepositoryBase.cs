namespace DotNetToolkit.Repository.Xml
{
    using InMemory;
    using Interceptors;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents a repository for storing entities as an xml formatted file.
    /// </summary>
    public abstract class XmlRepositoryBase<TEntity, TKey> : InMemoryRepositoryFileBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        protected XmlRepositoryBase(string path) : base(path) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected XmlRepositoryBase(string path, IEnumerable<IRepositoryInterceptor> interceptors) : base(path, interceptors) { }

        #endregion

        #region Overrides of InMemoryRepositoryFileBase<TEntity, TKey>

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        protected override string FileExtension { get; } = ".xml";

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
