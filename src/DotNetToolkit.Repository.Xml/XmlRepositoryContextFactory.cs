namespace DotNetToolkit.Repository.Xml
{
    using Configuration;
    using Factories;
    using Internal;
    using System;

    /// <summary>
    /// An implementation of <see cref="IRepositoryContextFactory" />.
    /// </summary>
    /// <seealso cref="IRepositoryContextFactory" />
    public class XmlRepositoryContextFactory : IRepositoryContextFactory
    {
        #region Fields

        private readonly string _path;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepositoryContextFactory"/> class.
        /// </summary>
        public XmlRepositoryContextFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepositoryContextFactory"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public XmlRepositoryContextFactory(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            _path = path;
        }

        #endregion

        #region Implementation of IRepositoryContextFactory

        /// <summary>
        /// Create a new repository context.
        /// </summary>
        /// <returns>The new repository context.</returns>
        public IRepositoryContext Create()
        {
            return new XmlRepositoryContext(_path);
        }

        #endregion
    }
}
