﻿namespace DotNetToolkit.Repository.Xml
{
    using Interceptors;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a repository for storing entities as an xml formatted file with a composite primary key.
    /// </summary>
    public class XmlRepository<TEntity, TKey1, TKey2, TKey3> : RepositoryBase<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public XmlRepository(string path) : this(path, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptor">The interceptor.</param>
        public XmlRepository(string path, IRepositoryInterceptor interceptor) : this(path, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptors">The interceptors.</param>
        public XmlRepository(string path, IEnumerable<IRepositoryInterceptor> interceptors) : base(new XmlContext<TEntity>(path), interceptors) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for storing entities as an xml formatted file with a composite primary key.
    /// </summary>
    public class XmlRepository<TEntity, TKey1, TKey2> : RepositoryBase<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public XmlRepository(string path) : this(path, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptor">The interceptor.</param>
        public XmlRepository(string path, IRepositoryInterceptor interceptor) : this(path, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptors">The interceptors.</param>
        public XmlRepository(string path, IEnumerable<IRepositoryInterceptor> interceptors) : base(new XmlContext<TEntity>(path), interceptors) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for storing entities as an xml formatted file.
    /// </summary>
    public class XmlRepository<TEntity, TKey> : RepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public XmlRepository(string path) : this(path, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptor">The interceptor.</param>
        public XmlRepository(string path, IRepositoryInterceptor interceptor) : this(path, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptors">The interceptors.</param>
        public XmlRepository(string path, IEnumerable<IRepositoryInterceptor> interceptors) : base(new XmlContext<TEntity>(path), interceptors) { }

        #endregion
    }

    /// <summary>
    /// Represents a repository for storing entities as an xml formatted file with a default primary key value of type integer (for testing purposes).
    /// </summary>
    public class XmlRepository<TEntity> : RepositoryBase<TEntity, int>, IRepository<TEntity> where TEntity : class
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public XmlRepository(string path) : this(path, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptor">The interceptor.</param>
        public XmlRepository(string path, IRepositoryInterceptor interceptor) : this(path, new List<IRepositoryInterceptor> { interceptor }) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlRepository{TEntity}"/> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="interceptors">The interceptors.</param>
        public XmlRepository(string path, IEnumerable<IRepositoryInterceptor> interceptors) : base(new XmlContext<TEntity>(path), interceptors) { }

        #endregion
    }
}
