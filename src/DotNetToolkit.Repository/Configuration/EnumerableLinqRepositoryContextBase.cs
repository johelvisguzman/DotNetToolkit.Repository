﻿namespace DotNetToolkit.Repository.Configuration
{
    using Conventions;
    using Extensions;
    using JetBrains.Annotations;
    using Logging;
    using Properties;
    using Query;
    using Query.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using Transactions;
    using Utility;

    /// <summary>
    /// Represents a repository context class which handles linq operations for querying data sources that implement <see cref="System.Collections.IEnumerable" />.
    /// </summary>
    public abstract class EnumerableLinqRepositoryContextBase : IRepositoryContext
    {
        #region Fields

        private ILoggerProvider _loggerProvider;
        private ILogger _logger;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the configurable conventions.
        /// </summary>
        public IRepositoryConventions Conventions { get; internal set; }

        /// <summary>
        /// Gets or sets the repository context logger.
        /// </summary>
        public ILogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = LoggerProvider?.Create(GetType().FullName);

                return _logger;
            }
        }

        /// <summary>
        /// Gets or sets the repository context logger provider.
        /// </summary>
        public ILoggerProvider LoggerProvider
        {
            get { return _loggerProvider; }
            set
            {
                _logger = null;
                _loggerProvider = value;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LinqRepositoryContextBase"/> class.
        /// </summary>
        protected EnumerableLinqRepositoryContextBase()
        {
            Conventions = RepositoryConventions.Default();

            _loggerProvider = NullLoggerProvider.Instance;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Returns the entity's query.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <returns>The entity's query.</returns>
        protected abstract IEnumerable<TEntity> AsEnumerable<TEntity>([CanBeNull] IFetchQueryStrategy<TEntity> fetchStrategy) where TEntity : class;

        #endregion

        #region Implementation of IRepositoryContext

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public virtual IEnumerable<TEntity> ExecuteSqlQuery<TEntity>([NotNull] string sql, CommandType cmdType, [CanBeNull] Dictionary<string, object> parameters, [NotNull] Func<IDataReader, TEntity> projector) where TEntity : class
        {
            throw new NotSupportedException(Resources.QueryExecutionNotSupported);
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>The number of rows affected.</returns>
        public virtual int ExecuteSqlCommand([NotNull] string sql, CommandType cmdType, [CanBeNull] Dictionary<string, object> parameters)
        {
            throw new NotSupportedException(Resources.QueryExecutionNotSupported);
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns>The transaction.</returns>
        public virtual ITransactionManager BeginTransaction()
        {
            throw new NotSupportedException(Resources.TransactionNotSupported);
        }

        /// <summary>
        /// Gets the current transaction.
        /// </summary>
        public ITransactionManager CurrentTransaction { get; internal set; }

        /// <summary>
        /// Tracks the specified entity in memory and will be inserted into the database when <see cref="IRepositoryContext.SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public abstract void Add<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Tracks the specified entity in memory and will be updated in the database when <see cref="IRepositoryContext.SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public abstract void Update<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Tracks the specified entity in memory and will be removed from the database when <see cref="IRepositoryContext.SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public abstract void Remove<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public abstract int SaveChanges();

        /// <summary>
        /// Finds an entity with the given primary key values in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity.</param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found in the repository.</returns>
        public virtual TEntity Find<TEntity>([CanBeNull] IFetchQueryStrategy<TEntity> fetchStrategy, [NotNull] params object[] keyValues) where TEntity : class
        {
            Guard.NotEmpty(keyValues, nameof(keyValues));

            var options = new QueryOptions<TEntity>()
                .Include(Conventions.GetByPrimaryKeySpecification<TEntity>(keyValues));

            var query = AsEnumerable(fetchStrategy);

            var result = query
                .ApplySpecificationOptions(options)
                .FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public virtual TResult Find<TEntity, TResult>([CanBeNull] IQueryOptions<TEntity> options, [NotNull] Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            Guard.NotNull(selector, nameof(selector));

            var result = AsEnumerable(options?.FetchStrategy)
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options)
                .ApplyPagingOptions(options)
                .Select(selector.Compile())
                .FirstOrDefault();

            return result;
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public virtual PagedQueryResult<IEnumerable<TResult>> FindAll<TEntity, TResult>([CanBeNull] IQueryOptions<TEntity> options, [NotNull] Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            Guard.NotNull(selector, nameof(selector));

            var query = AsEnumerable(options?.FetchStrategy)
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options);

            var total = query.Count();

            var result = query
                .ApplyPagingOptions(options)
                .Select(selector.Compile())
                .ToList();

            return new PagedQueryResult<IEnumerable<TResult>>(result, total);
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual int Count<TEntity>([CanBeNull] IQueryOptions<TEntity> options) where TEntity : class
        {
            var result = AsEnumerable(options?.FetchStrategy)
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options)
                .ApplyPagingOptions(options)
                .Count();

            return result;
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public virtual bool Exists<TEntity>([NotNull] IQueryOptions<TEntity> options) where TEntity : class
        {
            Guard.NotNull(options, nameof(options));

            var result = AsEnumerable(options?.FetchStrategy)
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options)
                .ApplyPagingOptions(options)
                .Any();

            return result;
        }

        /// <summary>
        /// Returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, and an element selector function with entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual PagedQueryResult<Dictionary<TDictionaryKey, TElement>> ToDictionary<TEntity, TDictionaryKey, TElement>([CanBeNull] IQueryOptions<TEntity> options, [NotNull] Expression<Func<TEntity, TDictionaryKey>> keySelector, [NotNull] Expression<Func<TEntity, TElement>> elementSelector) where TEntity : class
        {
            Guard.NotNull(keySelector, nameof(keySelector));
            Guard.NotNull(elementSelector, nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            var query = AsEnumerable(options?.FetchStrategy)
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options);

            Dictionary<TDictionaryKey, TElement> result;
            int total;

            if (options != null && options.PageSize != -1)
            {
                total = query.Count();

                result = query
                    .ApplyPagingOptions(options)
                    .ToDictionary(keySelectFunc, elementSelectorFunc);
            }
            else
            {
                // Gets the total count from memory
                result = query.ToDictionary(keySelectFunc, elementSelectorFunc);
                total = result.Count;
            }

            return new PagedQueryResult<Dictionary<TDictionaryKey, TElement>>(result, total);
        }

        /// <summary>
        /// Returns a new <see cref="IEnumerable{TResult}" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="IEnumerable{TResult}" /> that contains the grouped result that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public virtual PagedQueryResult<IEnumerable<TResult>> GroupBy<TEntity, TGroupKey, TResult>([CanBeNull] IQueryOptions<TEntity> options, [NotNull] Expression<Func<TEntity, TGroupKey>> keySelector, [NotNull] Expression<Func<IGrouping<TGroupKey, TEntity>, TResult>> resultSelector) where TEntity : class
        {
            Guard.NotNull(keySelector, nameof(keySelector));
            Guard.NotNull(resultSelector, nameof(resultSelector));

            var query = AsEnumerable(options?.FetchStrategy)
                .ApplySpecificationOptions(options);

            if (options?.SortingProperties.Count > 0)
            {
                throw new InvalidOperationException(Resources.GroupBySortingNotSupported);
            }

            var total = query.Count();

            var result = query
                .ApplyPagingOptions(options)
                .GroupBy(keySelector.Compile())
                .OrderBy(x => x.Key)
                .Select(resultSelector.Compile())
                .ToList();

            return new PagedQueryResult<IEnumerable<TResult>>(result, total);
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            CurrentTransaction = null;
        }

        #endregion
    }
}
