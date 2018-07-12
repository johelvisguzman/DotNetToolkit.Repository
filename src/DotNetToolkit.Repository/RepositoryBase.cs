﻿namespace DotNetToolkit.Repository
{
    using FetchStrategies;
    using Helpers;
    using Interceptors;
    using Properties;
    using Queries;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using Wrappers;

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey1, TKey2, TKey3}" />.
    /// </summary>
    public abstract class RepositoryBase<TEntity, TKey1, TKey2, TKey3> : RepositoryBase<TEntity>, IRepository<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Fields

        private IReadOnlyRepository<TEntity, TKey1, TKey2, TKey3> _wrapper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey1, TKey2, TKey3}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        protected RepositoryBase(IRepositoryContext context) : this(context, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey1, TKey2, TKey3}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected RepositoryBase(IRepositoryContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion

        #region Implementation of IRepository<TEntity, TKey1, TKey2, TKey3>

        /// <summary>
        /// Returns a read-only <see cref="IReadOnlyRepository{TEntity, TKey1, TKey2, TKey3}" /> wrapper for the current repository.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current repository.</returns>
        public IReadOnlyRepository<TEntity, TKey1, TKey2, TKey3> AsReadOnly()
        {
            return _wrapper ?? (_wrapper = new ReadOnlyRepository<TEntity, TKey1, TKey2, TKey3>(this));
        }

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        public void Delete(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            var entity = Get(key1, key2, key3);

            if (entity == null)
            {
                var ex = new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key1 + ", " + key2));

                Intercept(x => x.Error(ex));

                throw ex;
            }

            Delete(entity);
        }

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <return>The entity found.</return>
        public TEntity Get(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            return Get(key1, key2, key3, (IFetchStrategy<TEntity>)null);
        }

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public TEntity Get(TKey1 key1, TKey2 key2, TKey3 key3, IFetchStrategy<TEntity> fetchStrategy)
        {
            try
            {
                if (key1 == null)
                    throw new ArgumentNullException(nameof(key1));

                if (key2 == null)
                    throw new ArgumentNullException(nameof(key2));

                if (key3 == null)
                    throw new ArgumentNullException(nameof(key3));

                return Context.Find<TEntity>(fetchStrategy, key1, key2, key3);

            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="key3">The value of the third part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            return Get(key1, key2, key3) != null;
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey1, TKey2}" />.
    /// </summary>
    public abstract class RepositoryBase<TEntity, TKey1, TKey2> : RepositoryBase<TEntity>, IRepository<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Fields

        private IReadOnlyRepository<TEntity, TKey1, TKey2> _wrapper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey1, TKey2}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        protected RepositoryBase(IRepositoryContext context) : this(context, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey1, TKey2}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected RepositoryBase(IRepositoryContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion

        #region Implementation of IRepository<TEntity, TKey1, TKey2>

        /// <summary>
        /// Returns a read-only <see cref="IReadOnlyRepository{TEntity, TKey1, TKey2}" /> wrapper for the current repository.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current repository.</returns>
        public IReadOnlyRepository<TEntity, TKey1, TKey2> AsReadOnly()
        {
            return _wrapper ?? (_wrapper = new ReadOnlyRepository<TEntity, TKey1, TKey2>(this));
        }

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        public void Delete(TKey1 key1, TKey2 key2)
        {
            var entity = Get(key1, key2);

            if (entity == null)
            {
                var ex = new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key1 + ", " + key2));

                Intercept(x => x.Error(ex));

                throw ex;
            }

            Delete(entity);
        }

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <return>The entity found.</return>
        public TEntity Get(TKey1 key1, TKey2 key2)
        {
            return Get(key1, key2, (IFetchStrategy<TEntity>)null);
        }

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public TEntity Get(TKey1 key1, TKey2 key2, IFetchStrategy<TEntity> fetchStrategy)
        {
            try
            {
                if (key1 == null)
                    throw new ArgumentNullException(nameof(key1));

                if (key2 == null)
                    throw new ArgumentNullException(nameof(key2));

                return Context.Find<TEntity>(fetchStrategy, key1, key2);

            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <param name="key1">The value of the first part of the composite primary key used to match entities against.</param>
        /// <param name="key2">The value of the second part of the composite primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists(TKey1 key1, TKey2 key2)
        {
            return Get(key1, key2) != null;
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IRepository{TEntity, TKey}" />.
    /// </summary>
    public abstract class RepositoryBase<TEntity, TKey> : RepositoryBase<TEntity>, IRepository<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private IReadOnlyRepository<TEntity, TKey> _wrapper;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        protected RepositoryBase(IRepositoryContext context) : this(context, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity, TKey}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected RepositoryBase(IRepositoryContext context, IEnumerable<IRepositoryInterceptor> interceptors) : base(context, interceptors) { }

        #endregion

        #region Implementation of IRepository<TEntity, TKey>

        /// <summary>
        /// Returns a read-only <see cref="IReadOnlyRepository{TEntity, TKey}" /> wrapper for the current repository.
        /// </summary>
        /// <returns>An object that acts as a read-only wrapper around the current repository.</returns>
        public IReadOnlyRepository<TEntity, TKey> AsReadOnly()
        {
            return _wrapper ?? (_wrapper = new ReadOnlyRepository<TEntity, TKey>(this));
        }

        /// <summary>
        /// Deletes an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        public void Delete(TKey key)
        {
            var entity = Get(key);

            if (entity == null)
            {
                var ex = new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyNotFound, key));

                Intercept(x => x.Error(ex));

                throw ex;
            }

            Delete(entity);
        }

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <return>The entity found.</return>
        public TEntity Get(TKey key)
        {
            return Get(key, (IFetchStrategy<TEntity>)null);
        }

        /// <summary>
        /// Gets an entity with the given primary key value in the repository.
        /// </summary>
        /// <param name="key">The value of the primary key for the entity to be found.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <return>The entity found.</return>
        public TEntity Get(TKey key, IFetchStrategy<TEntity> fetchStrategy)
        {
            try
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                return Context.Find<TEntity>(fetchStrategy, key);
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Determines whether the repository contains an entity with the given primary key value
        /// </summary>
        /// <param name="key">The value of the primary key used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the given primary key value; otherwise, <c>false</c>.</returns>
        public bool Exists(TKey key)
        {
            return Get(key) != null;
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IRepositoryBase{TEntity}" />.
    /// </summary>
    public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : class
    {
        #region Fields

        private bool _disposed;
        private readonly IEnumerable<IRepositoryInterceptor> _interceptors;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the context.
        /// </summary>
        internal IRepositoryContext Context { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        protected RepositoryBase(IRepositoryContext context) : this(context, (IEnumerable<IRepositoryInterceptor>)null) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase{TEntity}" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected RepositoryBase(IRepositoryContext context, IEnumerable<IRepositoryInterceptor> interceptors)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            Context = context;
            _interceptors = interceptors ?? Enumerable.Empty<IRepositoryInterceptor>();
        }

        #endregion

        #region Internal Methods

        internal void InterceptAddItem(TEntity entity)
        {
            Intercept(x => x.AddExecuting(entity));

            Context.Add(entity);

            Intercept(x => x.AddExecuted(entity));
        }

        internal void InterceptAddItem(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                InterceptAddItem(entity);
            }
        }

        internal void InterceptUpdateItem(TEntity entity)
        {
            Intercept(x => x.UpdateExecuting(entity));

            Context.Update(entity);

            Intercept(x => x.UpdateExecuted(entity));
        }

        internal void InterceptUpdateItem(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                InterceptUpdateItem(entity);
            }
        }

        internal void InterceptDeleteItem(TEntity entity)
        {
            Intercept(x => x.DeleteExecuting(entity));

            Context.Remove(entity);

            Intercept(x => x.DeleteExecuted(entity));
        }

        internal void InterceptDeleteItem(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                InterceptDeleteItem(entity);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (Context != null)
                {
                    Context.Dispose();
                    Context = null;
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// Executes the specified interceptor activity.
        /// </summary>
        /// <param name="action">The action to be executed.</param>
        protected void Intercept(Action<IRepositoryInterceptor> action)
        {
            foreach (var interceptor in _interceptors)
            {
                action(interceptor);
            }
        }

        /// <summary>
        /// Intercepts any errors that occurred while performing the specified action.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by the specified action.</typeparam>
        /// <param name="action">The action.</param>
        /// <returns>The result of the performed action.</returns>
        protected T InterceptError<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        #endregion

        #region Implementation of IRepositoryBase<TEntity>

        /// <summary>
        /// Returns the number of entities contained in the repository.
        /// </summary>
        /// <returns>The number of entities contained in the repository.</returns>
        public int Count()
        {
            return Count((IQueryOptions<TEntity>)null);
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Count(InterceptError<IQueryOptions<TEntity>>(() => new QueryOptions<TEntity>().SatisfyBy(predicate)));
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public int Count(IQueryOptions<TEntity> options)
        {
            return InterceptError<int>(() => Context.Count<TEntity>(options));
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Dictionary<TDictionaryKey, TEntity> ToDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return ToDictionary<TDictionaryKey>((IQueryOptions<TEntity>)null, keySelector);
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> according to the specified <paramref name="keySelector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Dictionary<TDictionaryKey, TEntity> ToDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return ToDictionary<TDictionaryKey, TEntity>(options, keySelector, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Dictionary<TDictionaryKey, TElement> ToDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return ToDictionary<TDictionaryKey, TElement>((IQueryOptions<TEntity>)null, keySelector, elementSelector);
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function with entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Dictionary<TDictionaryKey, TElement> ToDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return InterceptError<Dictionary<TDictionaryKey, TElement>>(() => Context.ToDictionary(options, keySelector, elementSelector));
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A transform function to produce a result value from each element.</param>
        /// <returns>A new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        public IEnumerable<TResult> GroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            return GroupBy<TGroupKey, TResult>((IQueryOptions<TEntity>)null, keySelector, resultSelector);
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A transform function to produce a result value from each element.</param>
        /// <returns>A new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public IEnumerable<TResult> GroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            return InterceptError<IEnumerable<TResult>>(() => Context.GroupBy(options, keySelector, resultSelector));
        }

        /// <summary>
        /// Adds the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        public void Add(TEntity entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                InterceptAddItem(entity);

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Adds the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        public void Add(IEnumerable<TEntity> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                if (!entities.Any())
                    return;

                InterceptAddItem(entities);

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Updates the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        public void Update(TEntity entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                InterceptUpdateItem(entity);

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Updates the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        public void Update(IEnumerable<TEntity> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                if (!entities.Any())
                    return;

                InterceptUpdateItem(entities);

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Deletes the specified <paramref name="entity" /> into the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public void Delete(TEntity entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                InterceptDeleteItem(entity);

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Deletes all the entities in the repository that satisfies the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            Delete(FindAll(predicate));
        }

        /// <summary>
        /// Deletes all entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        public void Delete(IQueryOptions<TEntity> options)
        {
            Delete(FindAll(options));
        }

        /// <summary>
        /// Deletes the specified <paramref name="entities" /> collection into the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to delete.</param>
        public void Delete(IEnumerable<TEntity> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                if (!entities.Any())
                    return;

                InterceptDeleteItem(entities);

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                Intercept(x => x.Error(ex));

                throw;
            }
        }

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="predicate" /> in the repository.</returns>
        public TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Find<TEntity>(predicate, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the first entity in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The entity that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public TEntity Find(IQueryOptions<TEntity> options)
        {
            return Find<TEntity>(options, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="predicate" /> in the repository.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Find<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return Find<TResult>(InterceptError<IQueryOptions<TEntity>>(() => new QueryOptions<TEntity>().SatisfyBy(predicate)), selector);
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public TResult Find<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            return InterceptError<TResult>(() => Context.Find<TEntity, TResult>(options, selector));
        }

        /// <summary>
        /// Finds the collection of entities in the repository.
        /// </summary>
        /// <returns>The collection of entities in the repository.</returns>
        public IEnumerable<TEntity> FindAll()
        {
            return FindAll<TEntity>(IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return FindAll<TEntity>(predicate, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The collection of entities in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public IEnumerable<TEntity> FindAll(IQueryOptions<TEntity> options)
        {
            return FindAll<TEntity>(options, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository.
        /// </summary>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository.</returns>
        public IEnumerable<TResult> FindAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return FindAll<TResult>((IQueryOptions<TEntity>)null, selector);
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">A function to filter each entity.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="predicate" />.</returns>
        public IEnumerable<TResult> FindAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return FindAll<TResult>(InterceptError<IQueryOptions<TEntity>>(() => new QueryOptions<TEntity>().SatisfyBy(predicate)), selector);
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public IEnumerable<TResult> FindAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            return InterceptError<IEnumerable<TResult>>(() => Context.FindAll<TEntity, TResult>(options, selector));
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="predicate" />.
        /// </summary>
        /// <param name="predicate">The predicate used to match entities against.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified predicate; otherwise, <c>false</c>.</returns>
        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return Exists(InterceptError<IQueryOptions<TEntity>>(() => new QueryOptions<TEntity>().SatisfyBy(predicate)));
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public bool Exists(IQueryOptions<TEntity> options)
        {
            return InterceptError<bool>(() => Context.Exists<TEntity>(options));
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}