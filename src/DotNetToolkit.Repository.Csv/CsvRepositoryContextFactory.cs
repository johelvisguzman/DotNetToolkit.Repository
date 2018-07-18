namespace DotNetToolkit.Repository.Csv
{
    using Configuration;
    using Factories;
    using Internal;
    using System;

    /// <summary>
    /// An implementation of <see cref="IRepositoryContextFactory" />.
    /// </summary>
    /// <seealso cref="IRepositoryContextFactory" />
    public class CsvRepositoryContextFactory : IRepositoryContextFactory
    {
        #region Fields

        private readonly string _path;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepositoryContextFactory"/> class.
        /// </summary>
        public CsvRepositoryContextFactory() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRepositoryContextFactory"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public CsvRepositoryContextFactory(string path)
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
            return new CsvRepositoryContext(_path);
        }

        #endregion
    }
}
