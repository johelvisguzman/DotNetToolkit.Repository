namespace DotNetToolkit.Repository.AdoNet
{
    using Interceptors;
    using System.Collections.Generic;
    using System.Data.Common;

    /// <summary>
    /// Represents a repository for entity framework.
    /// </summary>
    public class AdoNetRepository<TEntity, TKey> : AdoNetRepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetRepository(string connectionString) : base(connectionString) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetRepository(string providerName, string connectionString) : base(providerName, connectionString) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetRepository(string connectionString, IEnumerable<IRepositoryInterceptor> interceptors) : base(connectionString, interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetRepository(string providerName, string connectionString, IEnumerable<IRepositoryInterceptor> interceptors) : base(providerName, connectionString, interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        public AdoNetRepository(DbTransaction transaction) : base(transaction) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetRepository(DbTransaction transaction, IEnumerable<IRepositoryInterceptor> interceptors) : base(transaction, interceptors) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for entity framework with a default primary key value of type integer.
    /// </summary>
    public class AdoNetRepository<TEntity> : AdoNetRepositoryBase<TEntity, int>, IRepositoryAsync<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetRepository(string connectionString) : base(connectionString) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetRepository(string providerName, string connectionString) : base(providerName, connectionString) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetRepository(string connectionString, IEnumerable<IRepositoryInterceptor> interceptors) : base(connectionString, interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetRepository(string providerName, string connectionString, IEnumerable<IRepositoryInterceptor> interceptors) : base(providerName, connectionString, interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        public AdoNetRepository(DbTransaction transaction) : base(transaction) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="interceptors">The interceptors.</param>
        public AdoNetRepository(DbTransaction transaction, IEnumerable<IRepositoryInterceptor> interceptors) : base(transaction, interceptors) { }

        #endregion
    }
}
