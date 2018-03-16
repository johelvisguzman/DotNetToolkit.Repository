﻿namespace DotNetToolkit.Repository.Json
{
    using InMemory;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Represents a repository for storing entities as an json formatted file.
    /// </summary>
    public abstract class JsonRepositoryBase<TEntity, TKey> : InMemoryFileBasedRepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        protected JsonRepositoryBase(string filePath) : base(filePath)
        {
        }

        #endregion

        #region Overrides of InMemoryFileBasedRepositoryBase<TEntity,TKey>

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        protected override string FileExtension { get; } = ".json";

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
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                TypeNameHandling = TypeNameHandling.All
            };

            serializer.Serialize(writer, entities);
        }

        #endregion
    }
}