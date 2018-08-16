﻿namespace DotNetToolkit.Repository
{
    using Configuration;
    using Configuration.Interceptors;
    using Factories;
    using System.Collections.Generic;

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey1, TKey2, TKey3}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepository{TEntity, TKey1, TKey2, TKey3}" />
    public class Repository<TEntity, TKey1, TKey2, TKey3> : RepositoryBase<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2, TKey3}"/> class, which uses a default in-memory repository context factory to create it's context (for testing purposes only).
        /// </summary>
        public Repository() : this(new InMemoryRepositoryContextFactory()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2, TKey3}"/> class, which uses a default in-memory repository context factory to create it's context (for testing purposes only).
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        public Repository(IRepositoryInterceptor interceptor) : this(new InMemoryRepositoryContextFactory(), new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2, TKey3}"/> class, which uses a default in-memory repository context factory to create it's context (for testing purposes only).
        /// </summary>
        /// <param name="interceptors">The interceptors.</param>
        public Repository(IEnumerable<IRepositoryInterceptor> interceptors) : this(new InMemoryRepositoryContextFactory(), interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2, TKey3}" /> class.
        /// </summary>
        /// <param name="factory">The repository context factory.</param>
        public Repository(IRepositoryContextFactory factory) : this(factory, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2, TKey3}" /> class.
        /// </summary>
        /// <param name="factory">The repository context factory.</param>
        /// <param name="interceptor">The interceptor.</param>
        public Repository(IRepositoryContextFactory factory, IRepositoryInterceptor interceptor) : this(factory, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2, TKey3}" /> class.
        /// </summary>
        /// <param name="factory">The repository context factory.</param>
        /// <param name="interceptors">The interceptors.</param>
        public Repository(IRepositoryContextFactory factory, IEnumerable<IRepositoryInterceptor> interceptors) : base(factory, interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2, TKey3}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        internal Repository(IRepositoryContext context) : this(context, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2, TKey3}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptor">The interceptor.</param>
        internal Repository(IRepositoryContext context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2, TKey3}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptors">The interceptors.</param>
        internal Repository(IRepositoryContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey1, TKey2}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepository{TEntity, TKey1, TKey2}" />
    public class Repository<TEntity, TKey1, TKey2> : RepositoryBase<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2}"/> class, which uses a default in-memory repository context factory to create it's context (for testing purposes only).
        /// </summary>
        public Repository() : this(new InMemoryRepositoryContextFactory()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2}"/> class, which uses a default in-memory repository context factory to create it's context (for testing purposes only).
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        public Repository(IRepositoryInterceptor interceptor) : this(new InMemoryRepositoryContextFactory(), new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2}"/> class, which uses a default in-memory repository context factory to create it's context (for testing purposes only).
        /// </summary>
        /// <param name="interceptors">The interceptors.</param>
        public Repository(IEnumerable<IRepositoryInterceptor> interceptors) : this(new InMemoryRepositoryContextFactory(), interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2}" /> class.
        /// </summary>
        /// <param name="factory">The repository context factory.</param>
        public Repository(IRepositoryContextFactory factory) : this(factory, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2}" /> class.
        /// </summary>
        /// <param name="factory">The repository context factory.</param>
        /// <param name="interceptor">The interceptor.</param>
        public Repository(IRepositoryContextFactory factory, IRepositoryInterceptor interceptor) : this(factory, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2}" /> class.
        /// </summary>
        /// <param name="factory">The repository context factory.</param>
        /// <param name="interceptors">The interceptors.</param>
        public Repository(IRepositoryContextFactory factory, IEnumerable<IRepositoryInterceptor> interceptors) : base(factory, interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        internal Repository(IRepositoryContext context) : this(context, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptor">The interceptor.</param>
        internal Repository(IRepositoryContext context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey1, TKey2}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptors">The interceptors.</param>
        internal Repository(IRepositoryContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepository{TEntity, TKey}" />
    public class Repository<TEntity, TKey> : RepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey}"/> class, which uses a default in-memory repository context factory to create it's context (for testing purposes only).
        /// </summary>
        public Repository() : this(new InMemoryRepositoryContextFactory()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey}"/> class, which uses a default in-memory repository context factory to create it's context (for testing purposes only).
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        public Repository(IRepositoryInterceptor interceptor) : this(new InMemoryRepositoryContextFactory(), new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey}"/> class, which uses a default in-memory repository context factory to create it's context (for testing purposes only).
        /// </summary>
        /// <param name="interceptors">The interceptors.</param>
        public Repository(IEnumerable<IRepositoryInterceptor> interceptors) : this(new InMemoryRepositoryContextFactory(), interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="factory">The repository context factory.</param>
        public Repository(IRepositoryContextFactory factory) : this(factory, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="factory">The repository context factory.</param>
        /// <param name="interceptor">The interceptor.</param>
        public Repository(IRepositoryContextFactory factory, IRepositoryInterceptor interceptor) : this(factory, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="factory">The repository context factory.</param>
        /// <param name="interceptors">The interceptors.</param>
        public Repository(IRepositoryContextFactory factory, IEnumerable<IRepositoryInterceptor> interceptors) : base(factory, interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        internal Repository(IRepositoryContext context) : this(context, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptor">The interceptor.</param>
        internal Repository(IRepositoryContext context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptors">The interceptors.</param>
        internal Repository(IRepositoryContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <seealso cref="DotNetToolkit.Repository.IRepository{TEntity}" />
    public class Repository<TEntity> : RepositoryBase<TEntity, int>, IRepository<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class, which uses a default in-memory repository context factory to create it's context (for testing purposes only).
        /// </summary>
        public Repository() : this(new InMemoryRepositoryContextFactory()) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class, which uses a default in-memory repository context factory to create it's context (for testing purposes only).
        /// </summary>
        /// <param name="interceptor">The interceptor.</param>
        public Repository(IRepositoryInterceptor interceptor) : this(new InMemoryRepositoryContextFactory(), new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class, which uses a default in-memory repository context factory to create it's context (for testing purposes only).
        /// </summary>
        /// <param name="interceptors">The interceptors.</param>
        public Repository(IEnumerable<IRepositoryInterceptor> interceptors) : this(new InMemoryRepositoryContextFactory(), interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}" /> class.
        /// </summary>
        /// <param name="factory">The repository context factory.</param>
        public Repository(IRepositoryContextFactory factory) : this(factory, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}" /> class.
        /// </summary>
        /// <param name="factory">The repository context factory.</param>
        /// <param name="interceptor">The interceptor.</param>
        public Repository(IRepositoryContextFactory factory, IRepositoryInterceptor interceptor) : this(factory, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}" /> class.
        /// </summary>
        /// <param name="factory">The repository context factory.</param>
        /// <param name="interceptors">The interceptors.</param>
        public Repository(IRepositoryContextFactory factory, IEnumerable<IRepositoryInterceptor> interceptors) : base(factory, interceptors) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        internal Repository(IRepositoryContext context) : this(context, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptor">The interceptor.</param>
        internal Repository(IRepositoryContext context, IRepositoryInterceptor interceptor) : this(context, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The repository context.</param>
        /// <param name="interceptors">The interceptors.</param>
        internal Repository(IRepositoryContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion
    }
}