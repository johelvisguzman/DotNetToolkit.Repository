namespace DotNetToolkit.Repository.EntityFrameworkCore
{
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents a repository for entity framework core.
    /// </summary>
    public class EfCoreRepository<TEntity, TKey> : EfCoreRepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public EfCoreRepository(DbContext context) : base(context) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for entity framework core with a default primary key value of type integer.
    /// </summary>
    public class EfCoreRepository<TEntity> : EfCoreRepositoryBase<TEntity, int>, IRepository<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfCoreRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public EfCoreRepository(DbContext context) : base(context) { }

        #endregion
    }
}
