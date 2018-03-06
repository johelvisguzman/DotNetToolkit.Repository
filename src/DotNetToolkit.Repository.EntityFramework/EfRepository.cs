﻿namespace DotNetToolkit.Repository.EntityFramework
{
    using System.Data.Entity;

    /// <summary>
    /// Represents a repository for entity framework.
    /// </summary>
    public class EfRepository<TEntity, TKey> : EfRepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public EfRepository(DbContext context) : base(context) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for entity framework with a default primary key value of type integer.
    /// </summary>
    public class EfRepository<TEntity> : EfRepositoryBase<TEntity, int> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public EfRepository(DbContext context) : base(context) { }

        #endregion
    }
}
