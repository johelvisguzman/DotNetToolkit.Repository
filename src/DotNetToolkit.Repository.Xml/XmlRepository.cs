namespace DotNetToolkit.Repository.Xml
{
    /// <summary>
    /// Represents a repository for storing entities as an xml formatted file.
    /// </summary>
    public class XmlRepository<TEntity, TKey> : XmlRepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository{TEntity,TKey}"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public XmlRepository(string filePath = null) : base(filePath)
        {
        }

        #endregion
    }

    /// <summary>
    /// Represents a repository for storing entities as an xml formatted file with a default primary key value of type integer (for testing purposes).
    /// </summary>
    public class XmlRepository<TEntity> : XmlRepositoryBase<TEntity, int>, IRepository<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        public XmlRepository(string filePath = null) : base(filePath)
        {
        }

        #endregion
    }
}
