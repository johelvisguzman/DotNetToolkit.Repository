namespace DotNetToolkit.Repository.Internal
{
    using Configuration.Conventions;
    using JetBrains.Annotations;
    using Queries;
    using Queries.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="IReadOnlyRepository{TEntity, TKey1, TKey2, TKey3}" />.
    /// </summary>
    [ComVisible(false)]
    internal class ReadOnlyRepositoryWrapper<TEntity, TKey1, TKey2, TKey3> : IReadOnlyRepository<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Fields

        private readonly IRepository<TEntity, TKey1, TKey2, TKey3> _underlyingRepo;

        #endregion

        #region Constructors

        public ReadOnlyRepositoryWrapper([NotNull] IRepository<TEntity, TKey1, TKey2, TKey3> repo)
        {
            _underlyingRepo = Guard.NotNull(repo, nameof(repo));
        }

        #endregion

        #region Implementation of IReadOnlyRepository<TEntity,in TKey1, TKey2, TKey3>

        public bool CacheEnabled
        {
            get { return _underlyingRepo.CacheEnabled; }
            set { _underlyingRepo.CacheEnabled = value; }
        }

        public void ClearCache()
        {
            _underlyingRepo.ClearCache();
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, CommandType cmdType, object[] parameters)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, cmdType, parameters);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, object[] parameters)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, parameters);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, cmdType, parameters, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, object[] parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, parameters, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, Func<IDataReader, IRepositoryConventions, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, cmdType, parameters, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, object[] parameters, Func<IDataReader, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, parameters, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, Func<IDataReader, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, projector);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, CommandType cmdType, object[] parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, cmdType, parameters, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, object[] parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, parameters, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, cmdType, parameters, projector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, object[] parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, parameters, projector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, Func<IDataReader, IRepositoryConventions, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, projector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, cmdType, parameters, projector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, object[] parameters, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, parameters, projector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, projector, cancellationToken);
        }

        public TEntity Find(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            return _underlyingRepo.Find(key1, key2, key3);
        }

        public TEntity Find(TKey1 key1, TKey2 key2, TKey3 key3, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            return _underlyingRepo.Find(key1, key2, key3, fetchStrategy);
        }

        public TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingRepo.Find(predicate);
        }

        public TEntity Find(IQueryOptions<TEntity> options)
        {
            return _underlyingRepo.Find(options);
        }

        public TResult Find<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingRepo.Find<TResult>(predicate, selector);
        }

        public TResult Find<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingRepo.Find<TResult>(options, selector);
        }

        public IEnumerable<TEntity> FindAll()
        {
            return _underlyingRepo.FindAll();
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingRepo.FindAll(predicate);
        }

        public IPagedQueryResult<IEnumerable<TEntity>> FindAll(IQueryOptions<TEntity> options)
        {
            return _underlyingRepo.FindAll(options);
        }

        public IEnumerable<TResult> FindAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingRepo.FindAll(selector);
        }

        public IEnumerable<TResult> FindAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingRepo.FindAll<TResult>(predicate, selector);
        }

        public IPagedQueryResult<IEnumerable<TResult>> FindAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingRepo.FindAll<TResult>(options, selector);
        }

        public bool Exists(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            return _underlyingRepo.Exists(key1, key2, key3);
        }

        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingRepo.Exists(predicate);
        }

        public bool Exists(IQueryOptions<TEntity> options)
        {
            return _underlyingRepo.Exists(options);
        }

        public int Count()
        {
            return _underlyingRepo.Count();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingRepo.Count(predicate);
        }

        public int Count(IQueryOptions<TEntity> options)
        {
            return _underlyingRepo.Count(options);
        }

        public Dictionary<TDictionaryKey, TEntity> ToDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return _underlyingRepo.ToDictionary<TDictionaryKey>(keySelector);
        }

        public IPagedQueryResult<Dictionary<TDictionaryKey, TEntity>> ToDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return _underlyingRepo.ToDictionary<TDictionaryKey>(options, keySelector);
        }

        public Dictionary<TDictionaryKey, TElement> ToDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return _underlyingRepo.ToDictionary<TDictionaryKey, TElement>(keySelector, elementSelector);
        }

        public IPagedQueryResult<Dictionary<TDictionaryKey, TElement>> ToDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return _underlyingRepo.ToDictionary<TDictionaryKey, TElement>(options, keySelector, elementSelector);
        }

        public IEnumerable<TResult> GroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            return _underlyingRepo.GroupBy<TGroupKey, TResult>(keySelector, resultSelector);
        }

        public IPagedQueryResult<IEnumerable<TResult>> GroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            return _underlyingRepo.GroupBy<TGroupKey, TResult>(options, keySelector, resultSelector);
        }

        public Task<TEntity> FindAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync(key1, key2, key3, cancellationToken);
        }

        public Task<TEntity> FindAsync(TKey1 key1, TKey2 key2, TKey3 key3, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync(key1, key2, key3, fetchStrategy, cancellationToken);
        }

        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync(predicate, cancellationToken);
        }

        public Task<TEntity> FindAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync(options, cancellationToken);
        }

        public Task<TResult> FindAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync<TResult>(predicate, selector, cancellationToken);
        }

        public Task<TResult> FindAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync<TResult>(options, selector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> FindAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync(cancellationToken);
        }

        public Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync(predicate, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TEntity>>> FindAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync(options, cancellationToken);
        }

        public Task<IEnumerable<TResult>> FindAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync<TResult>(selector, cancellationToken);
        }

        public Task<IEnumerable<TResult>> FindAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync<TResult>(predicate, selector, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TResult>>> FindAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync<TResult>(options, selector, cancellationToken);
        }

        public Task<bool> ExistsAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExistsAsync(key1, key2, key3, cancellationToken);
        }

        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExistsAsync(predicate, cancellationToken);
        }

        public Task<bool> ExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExistsAsync(options, cancellationToken);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.CountAsync(cancellationToken);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.CountAsync(predicate, cancellationToken);
        }

        public Task<int> CountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.CountAsync(options, cancellationToken);
        }

        public Task<Dictionary<TDictionaryKey, TEntity>> ToDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ToDictionaryAsync<TDictionaryKey>(keySelector, cancellationToken);
        }

        public Task<IPagedQueryResult<Dictionary<TDictionaryKey, TEntity>>> ToDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ToDictionaryAsync<TDictionaryKey>(options, keySelector, cancellationToken);
        }

        public Task<Dictionary<TDictionaryKey, TElement>> ToDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ToDictionaryAsync<TDictionaryKey, TElement>(keySelector, elementSelector, cancellationToken);
        }

        public Task<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>> ToDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ToDictionaryAsync<TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);
        }

        public Task<IEnumerable<TResult>> GroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.GroupByAsync<TGroupKey, TResult>(keySelector, resultSelector, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TResult>>> GroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.GroupByAsync<TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IReadOnlyRepository{TEntity, TKey1, TKey2}" />.
    /// </summary>
    [ComVisible(false)]
    internal class ReadOnlyRepositoryWrapper<TEntity, TKey1, TKey2> : IReadOnlyRepository<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Fields

        private readonly IRepository<TEntity, TKey1, TKey2> _underlyingRepo;

        #endregion

        #region Constructors

        public ReadOnlyRepositoryWrapper([NotNull] IRepository<TEntity, TKey1, TKey2> repo)
        {
            _underlyingRepo = Guard.NotNull(repo, nameof(repo));
        }

        #endregion

        #region Implementation of IReadOnlyRepository<TEntity,in TKey1, TKey2>

        public bool CacheEnabled
        {
            get { return _underlyingRepo.CacheEnabled; }
            set { _underlyingRepo.CacheEnabled = value; }
        }

        public void ClearCache()
        {
            _underlyingRepo.ClearCache();
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, CommandType cmdType, object[] parameters)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, cmdType, parameters);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, object[] parameters)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, parameters);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, cmdType, parameters, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, object[] parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, parameters, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, Func<IDataReader, IRepositoryConventions, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, cmdType, parameters, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, object[] parameters, Func<IDataReader, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, parameters, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, Func<IDataReader, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, projector);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, CommandType cmdType, object[] parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, cmdType, parameters, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, object[] parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, parameters, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, cmdType, parameters, projector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, object[] parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, parameters, projector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, Func<IDataReader, IRepositoryConventions, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, projector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, cmdType, parameters, projector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, object[] parameters, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, parameters, projector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, projector, cancellationToken);
        }

        public TEntity Find(TKey1 key1, TKey2 key2)
        {
            return _underlyingRepo.Find(key1, key2);
        }

        public TEntity Find(TKey1 key1, TKey2 key2, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            return _underlyingRepo.Find(key1, key2, fetchStrategy);
        }

        public TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingRepo.Find(predicate);
        }

        public TEntity Find(IQueryOptions<TEntity> options)
        {
            return _underlyingRepo.Find(options);
        }

        public TResult Find<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingRepo.Find<TResult>(predicate, selector);
        }

        public TResult Find<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingRepo.Find<TResult>(options, selector);
        }

        public IEnumerable<TEntity> FindAll()
        {
            return _underlyingRepo.FindAll();
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingRepo.FindAll(predicate);
        }

        public IPagedQueryResult<IEnumerable<TEntity>> FindAll(IQueryOptions<TEntity> options)
        {
            return _underlyingRepo.FindAll(options);
        }

        public IEnumerable<TResult> FindAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingRepo.FindAll(selector);
        }

        public IEnumerable<TResult> FindAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingRepo.FindAll<TResult>(predicate, selector);
        }

        public IPagedQueryResult<IEnumerable<TResult>> FindAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingRepo.FindAll<TResult>(options, selector);
        }

        public bool Exists(TKey1 key1, TKey2 key2)
        {
            return _underlyingRepo.Exists(key1, key2);
        }

        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingRepo.Exists(predicate);
        }

        public bool Exists(IQueryOptions<TEntity> options)
        {
            return _underlyingRepo.Exists(options);
        }

        public int Count()
        {
            return _underlyingRepo.Count();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingRepo.Count(predicate);
        }

        public int Count(IQueryOptions<TEntity> options)
        {
            return _underlyingRepo.Count(options);
        }

        public Dictionary<TDictionaryKey, TEntity> ToDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return _underlyingRepo.ToDictionary<TDictionaryKey>(keySelector);
        }

        public IPagedQueryResult<Dictionary<TDictionaryKey, TEntity>> ToDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return _underlyingRepo.ToDictionary<TDictionaryKey>(options, keySelector);
        }

        public Dictionary<TDictionaryKey, TElement> ToDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return _underlyingRepo.ToDictionary<TDictionaryKey, TElement>(keySelector, elementSelector);
        }

        public IPagedQueryResult<Dictionary<TDictionaryKey, TElement>> ToDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return _underlyingRepo.ToDictionary<TDictionaryKey, TElement>(options, keySelector, elementSelector);
        }

        public IEnumerable<TResult> GroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            return _underlyingRepo.GroupBy<TGroupKey, TResult>(keySelector, resultSelector);
        }

        public IPagedQueryResult<IEnumerable<TResult>> GroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            return _underlyingRepo.GroupBy<TGroupKey, TResult>(options, keySelector, resultSelector);
        }

        public Task<TEntity> FindAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync(key1, key2, cancellationToken);
        }

        public Task<TEntity> FindAsync(TKey1 key1, TKey2 key2, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync(key1, key2, fetchStrategy, cancellationToken);
        }

        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync(predicate, cancellationToken);
        }

        public Task<TEntity> FindAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync(options, cancellationToken);
        }

        public Task<TResult> FindAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync<TResult>(predicate, selector, cancellationToken);
        }

        public Task<TResult> FindAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync<TResult>(options, selector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> FindAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync(cancellationToken);
        }

        public Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync(predicate, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TEntity>>> FindAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync(options, cancellationToken);
        }

        public Task<IEnumerable<TResult>> FindAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync<TResult>(selector, cancellationToken);
        }

        public Task<IEnumerable<TResult>> FindAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync<TResult>(predicate, selector, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TResult>>> FindAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync<TResult>(options, selector, cancellationToken);
        }

        public Task<bool> ExistsAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExistsAsync(key1, key2, cancellationToken);
        }

        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExistsAsync(predicate, cancellationToken);
        }

        public Task<bool> ExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExistsAsync(options, cancellationToken);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.CountAsync(cancellationToken);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.CountAsync(predicate, cancellationToken);
        }

        public Task<int> CountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.CountAsync(options, cancellationToken);
        }

        public Task<Dictionary<TDictionaryKey, TEntity>> ToDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ToDictionaryAsync<TDictionaryKey>(keySelector, cancellationToken);
        }

        public Task<IPagedQueryResult<Dictionary<TDictionaryKey, TEntity>>> ToDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ToDictionaryAsync<TDictionaryKey>(options, keySelector, cancellationToken);
        }

        public Task<Dictionary<TDictionaryKey, TElement>> ToDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ToDictionaryAsync<TDictionaryKey, TElement>(keySelector, elementSelector, cancellationToken);
        }

        public Task<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>> ToDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ToDictionaryAsync<TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);
        }

        public Task<IEnumerable<TResult>> GroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.GroupByAsync<TGroupKey, TResult>(keySelector, resultSelector, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TResult>>> GroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.GroupByAsync<TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IReadOnlyRepository{TEntity, TKey}" />.
    /// </summary>
    [ComVisible(false)]
    internal class ReadOnlyRepositoryWrapper<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private readonly IRepository<TEntity, TKey> _underlyingRepo;

        #endregion

        #region Constructors

        public ReadOnlyRepositoryWrapper([NotNull] IRepository<TEntity, TKey> repo)
        {
            _underlyingRepo = Guard.NotNull(repo, nameof(repo));
        }

        #endregion

        #region Implementation of IReadOnlyRepository<TEntity,in TKey>

        public bool CacheEnabled
        {
            get { return _underlyingRepo.CacheEnabled; }
            set { _underlyingRepo.CacheEnabled = value; }
        }

        public void ClearCache()
        {
            _underlyingRepo.ClearCache();
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, CommandType cmdType, object[] parameters)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, cmdType, parameters);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, object[] parameters)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, parameters);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, cmdType, parameters, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, object[] parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, parameters, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, Func<IDataReader, IRepositoryConventions, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, cmdType, parameters, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, object[] parameters, Func<IDataReader, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, parameters, projector);
        }

        public IEnumerable<TEntity> ExecuteSqlQuery(string sql, Func<IDataReader, TEntity> projector)
        {
            return _underlyingRepo.ExecuteSqlQuery(sql, projector);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, CommandType cmdType, object[] parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, cmdType, parameters, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, object[] parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, parameters, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, cmdType, parameters, projector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, object[] parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, parameters, projector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, Func<IDataReader, IRepositoryConventions, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, projector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, cmdType, parameters, projector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, object[] parameters, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, parameters, projector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync(string sql, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExecuteSqlQueryAsync(sql, projector, cancellationToken);
        }

        public TEntity Find(TKey key)
        {
            return _underlyingRepo.Find(key);
        }

        public TEntity Find(TKey key, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            return _underlyingRepo.Find(key, fetchStrategy);
        }

        public TEntity Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingRepo.Find(predicate);
        }

        public TEntity Find(IQueryOptions<TEntity> options)
        {
            return _underlyingRepo.Find(options);
        }

        public TResult Find<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingRepo.Find<TResult>(predicate, selector);
        }

        public TResult Find<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingRepo.Find<TResult>(options, selector);
        }

        public IEnumerable<TEntity> FindAll()
        {
            return _underlyingRepo.FindAll();
        }

        public IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingRepo.FindAll(predicate);
        }

        public IPagedQueryResult<IEnumerable<TEntity>> FindAll(IQueryOptions<TEntity> options)
        {
            return _underlyingRepo.FindAll(options);
        }

        public IEnumerable<TResult> FindAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingRepo.FindAll(selector);
        }

        public IEnumerable<TResult> FindAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingRepo.FindAll<TResult>(predicate, selector);
        }

        public IPagedQueryResult<IEnumerable<TResult>> FindAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingRepo.FindAll<TResult>(options, selector);
        }

        public bool Exists(TKey key)
        {
            return _underlyingRepo.Exists(key);
        }

        public bool Exists(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingRepo.Exists(predicate);
        }

        public bool Exists(IQueryOptions<TEntity> options)
        {
            return _underlyingRepo.Exists(options);
        }

        public int Count()
        {
            return _underlyingRepo.Count();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingRepo.Count(predicate);
        }

        public int Count(IQueryOptions<TEntity> options)
        {
            return _underlyingRepo.Count(options);
        }

        public Dictionary<TDictionaryKey, TEntity> ToDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return _underlyingRepo.ToDictionary<TDictionaryKey>(keySelector);
        }

        public IPagedQueryResult<Dictionary<TDictionaryKey, TEntity>> ToDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return _underlyingRepo.ToDictionary<TDictionaryKey>(options, keySelector);
        }

        public Dictionary<TDictionaryKey, TElement> ToDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return _underlyingRepo.ToDictionary<TDictionaryKey, TElement>(keySelector, elementSelector);
        }

        public IPagedQueryResult<Dictionary<TDictionaryKey, TElement>> ToDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return _underlyingRepo.ToDictionary<TDictionaryKey, TElement>(options, keySelector, elementSelector);
        }

        public IEnumerable<TResult> GroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            return _underlyingRepo.GroupBy<TGroupKey, TResult>(keySelector, resultSelector);
        }

        public IPagedQueryResult<IEnumerable<TResult>> GroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            return _underlyingRepo.GroupBy<TGroupKey, TResult>(options, keySelector, resultSelector);
        }

        public Task<TEntity> FindAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync(key, cancellationToken);
        }

        public Task<TEntity> FindAsync(TKey key, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync(key, fetchStrategy, cancellationToken);
        }

        public Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync(predicate, cancellationToken);
        }

        public Task<TEntity> FindAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync(options, cancellationToken);
        }

        public Task<TResult> FindAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync<TResult>(predicate, selector, cancellationToken);
        }

        public Task<TResult> FindAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAsync<TResult>(options, selector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> FindAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync(cancellationToken);
        }

        public Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync(predicate, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TEntity>>> FindAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync(options, cancellationToken);
        }

        public Task<IEnumerable<TResult>> FindAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync<TResult>(selector, cancellationToken);
        }

        public Task<IEnumerable<TResult>> FindAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync<TResult>(predicate, selector, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TResult>>> FindAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.FindAllAsync<TResult>(options, selector, cancellationToken);
        }

        public Task<bool> ExistsAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExistsAsync(key, cancellationToken);
        }

        public Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExistsAsync(predicate, cancellationToken);
        }

        public Task<bool> ExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ExistsAsync(options, cancellationToken);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.CountAsync(cancellationToken);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.CountAsync(predicate, cancellationToken);
        }

        public Task<int> CountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.CountAsync(options, cancellationToken);
        }

        public Task<Dictionary<TDictionaryKey, TEntity>> ToDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ToDictionaryAsync<TDictionaryKey>(keySelector, cancellationToken);
        }

        public Task<IPagedQueryResult<Dictionary<TDictionaryKey, TEntity>>> ToDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ToDictionaryAsync<TDictionaryKey>(options, keySelector, cancellationToken);
        }

        public Task<Dictionary<TDictionaryKey, TElement>> ToDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ToDictionaryAsync<TDictionaryKey, TElement>(keySelector, elementSelector, cancellationToken);
        }

        public Task<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>> ToDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.ToDictionaryAsync<TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);
        }

        public Task<IEnumerable<TResult>> GroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.GroupByAsync<TGroupKey, TResult>(keySelector, resultSelector, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TResult>>> GroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingRepo.GroupByAsync<TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);
        }

        #endregion
    }
}
