namespace DotNetToolkit.Repository.AdoNet
{
    using Logging;

    /// <summary>
    /// Represents a repository for entity framework.
    /// </summary>
    public class AdoNetRepository<TEntity, TKey> : AdoNetRepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetRepository(string providerName, string connectionString) : base(providerName, connectionString) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="logger">The logger.</param>
        public AdoNetRepository(string providerName, string connectionString, ILogger logger) : base(providerName, connectionString, logger) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for entity framework with a default primary key value of type integer.
    /// </summary>
    public class AdoNetRepository<TEntity> : AdoNetRepositoryBase<TEntity, int>, IRepository<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetRepository(string providerName, string connectionString) : base(providerName, connectionString) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="logger">The logger.</param>
        public AdoNetRepository(string providerName, string connectionString, ILogger logger) : base(providerName, connectionString, logger) { }

        #endregion
    }
}
