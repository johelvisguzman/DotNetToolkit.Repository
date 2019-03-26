namespace DotNetToolkit.Repository.Internal
{
    using Queries;
    using Queries.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An implementation of <see cref="IReadOnlyService{TEntity, TKey1, TKey2, TKey3}" />.
    /// </summary>
    [ComVisible(false)]
    internal class ReadOnlyServiceWrapper<TEntity, TKey1, TKey2, TKey3> : IReadOnlyService<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Fields

        private readonly IService<TEntity, TKey1, TKey2, TKey3> _underlyingService;

        #endregion

        #region Constructors

        public ReadOnlyServiceWrapper(IService<TEntity, TKey1, TKey2, TKey3> service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _underlyingService = service;
        }

        #endregion

        #region Implementation of IReadOnlyService<TEntity,in TKey1, TKey2, TKey3>

        public TEntity Get(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            return _underlyingService.Get(key1, key2, key3);
        }

        public TEntity Get(TKey1 key1, TKey2 key2, TKey3 key3, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            return _underlyingService.Get(key1, key2, key3, fetchStrategy);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingService.Get(predicate);
        }

        public TEntity Get(IQueryOptions<TEntity> options)
        {
            return _underlyingService.Get(options);
        }

        public TResult Get<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingService.Get<TResult>(predicate, selector);
        }

        public TResult Get<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingService.Get<TResult>(options, selector);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _underlyingService.GetAll();
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingService.GetAll(predicate);
        }

        public IPagedQueryResult<IEnumerable<TEntity>> GetAll(IQueryOptions<TEntity> options)
        {
            return _underlyingService.GetAll(options);
        }

        public IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingService.GetAll(selector);
        }

        public IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingService.GetAll<TResult>(predicate, selector);
        }

        public IPagedQueryResult<IEnumerable<TResult>> GetAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingService.GetAll<TResult>(options, selector);
        }

        public bool GetExists(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            return _underlyingService.GetExists(key1, key2, key3);
        }

        public bool GetExists(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingService.GetExists(predicate);
        }

        public bool GetExists(IQueryOptions<TEntity> options)
        {
            return _underlyingService.GetExists(options);
        }

        public int GetCount()
        {
            return _underlyingService.GetCount();
        }

        public int GetCount(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingService.GetCount(predicate);
        }

        public int GetCount(IQueryOptions<TEntity> options)
        {
            return _underlyingService.GetCount(options);
        }

        public Dictionary<TDictionaryKey, TEntity> GetDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return _underlyingService.GetDictionary<TDictionaryKey>(keySelector);
        }

        public IPagedQueryResult<Dictionary<TDictionaryKey, TEntity>> GetDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return _underlyingService.GetDictionary<TDictionaryKey>(options, keySelector);
        }

        public Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return _underlyingService.GetDictionary<TDictionaryKey, TElement>(keySelector, elementSelector);
        }

        public IPagedQueryResult<Dictionary<TDictionaryKey, TElement>> GetDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return _underlyingService.GetDictionary<TDictionaryKey, TElement>(options, keySelector, elementSelector);
        }

        public IEnumerable<TResult> GetGroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            return _underlyingService.GetGroupBy<TGroupKey, TResult>(keySelector, resultSelector);
        }

        public IPagedQueryResult<IEnumerable<TResult>> GetGroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            return _underlyingService.GetGroupBy<TGroupKey, TResult>(options, keySelector, resultSelector);
        }

        public Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync(key1, key2, key3, cancellationToken);
        }

        public Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, TKey3 key3, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync(key1, key2, key3, fetchStrategy, cancellationToken);
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync(predicate, cancellationToken);
        }

        public Task<TEntity> GetAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync(options, cancellationToken);
        }

        public Task<TResult> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync<TResult>(predicate, selector, cancellationToken);
        }

        public Task<TResult> GetAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync<TResult>(options, selector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync(cancellationToken);
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync(predicate, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TEntity>>> GetAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync(options, cancellationToken);
        }

        public Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync<TResult>(selector, cancellationToken);
        }

        public Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync<TResult>(predicate, selector, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TResult>>> GetAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync<TResult>(options, selector, cancellationToken);
        }

        public Task<bool> GetExistsAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetExistsAsync(key1, key2, key3, cancellationToken);
        }

        public Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetExistsAsync(predicate, cancellationToken);
        }

        public Task<bool> GetExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetExistsAsync(options, cancellationToken);
        }

        public Task<int> GetCountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetCountAsync(cancellationToken);
        }

        public Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetCountAsync(predicate, cancellationToken);
        }

        public Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetCountAsync(options, cancellationToken);
        }

        public Task<Dictionary<TDictionaryKey, TEntity>> GetDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetDictionaryAsync<TDictionaryKey>(keySelector, cancellationToken);
        }

        public Task<IPagedQueryResult<Dictionary<TDictionaryKey, TEntity>>> GetDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetDictionaryAsync<TDictionaryKey>(options, keySelector, cancellationToken);
        }

        public Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetDictionaryAsync<TDictionaryKey, TElement>(keySelector, elementSelector, cancellationToken);
        }

        public Task<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>> GetDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetDictionaryAsync<TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);
        }

        public Task<IEnumerable<TResult>> GetGroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetGroupByAsync<TGroupKey, TResult>(keySelector, resultSelector, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TResult>>> GetGroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetGroupByAsync<TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IReadOnlyService{TEntity, TKey1, TKey2}" />.
    /// </summary>
    [ComVisible(false)]
    internal class ReadOnlyServiceWrapper<TEntity, TKey1, TKey2> : IReadOnlyService<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Fields

        private readonly IService<TEntity, TKey1, TKey2> _underlyingService;

        #endregion

        #region Constructors

        public ReadOnlyServiceWrapper(IService<TEntity, TKey1, TKey2> service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _underlyingService = service;
        }

        #endregion

        #region Implementation of IReadOnlyService<TEntity,in TKey1, TKey2>

        public TEntity Get(TKey1 key1, TKey2 key2)
        {
            return _underlyingService.Get(key1, key2);
        }

        public TEntity Get(TKey1 key1, TKey2 key2, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            return _underlyingService.Get(key1, key2, fetchStrategy);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingService.Get(predicate);
        }

        public TEntity Get(IQueryOptions<TEntity> options)
        {
            return _underlyingService.Get(options);
        }

        public TResult Get<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingService.Get<TResult>(predicate, selector);
        }

        public TResult Get<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingService.Get<TResult>(options, selector);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _underlyingService.GetAll();
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingService.GetAll(predicate);
        }

        public IPagedQueryResult<IEnumerable<TEntity>> GetAll(IQueryOptions<TEntity> options)
        {
            return _underlyingService.GetAll(options);
        }

        public IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingService.GetAll(selector);
        }

        public IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingService.GetAll<TResult>(predicate, selector);
        }

        public IPagedQueryResult<IEnumerable<TResult>> GetAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingService.GetAll<TResult>(options, selector);
        }

        public bool GetExists(TKey1 key1, TKey2 key2)
        {
            return _underlyingService.GetExists(key1, key2);
        }

        public bool GetExists(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingService.GetExists(predicate);
        }

        public bool GetExists(IQueryOptions<TEntity> options)
        {
            return _underlyingService.GetExists(options);
        }

        public int GetCount()
        {
            return _underlyingService.GetCount();
        }

        public int GetCount(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingService.GetCount(predicate);
        }

        public int GetCount(IQueryOptions<TEntity> options)
        {
            return _underlyingService.GetCount(options);
        }

        public Dictionary<TDictionaryKey, TEntity> GetDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return _underlyingService.GetDictionary<TDictionaryKey>(keySelector);
        }

        public IPagedQueryResult<Dictionary<TDictionaryKey, TEntity>> GetDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return _underlyingService.GetDictionary<TDictionaryKey>(options, keySelector);
        }

        public Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return _underlyingService.GetDictionary<TDictionaryKey, TElement>(keySelector, elementSelector);
        }

        public IPagedQueryResult<Dictionary<TDictionaryKey, TElement>> GetDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return _underlyingService.GetDictionary<TDictionaryKey, TElement>(options, keySelector, elementSelector);
        }

        public IEnumerable<TResult> GetGroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            return _underlyingService.GetGroupBy<TGroupKey, TResult>(keySelector, resultSelector);
        }

        public IPagedQueryResult<IEnumerable<TResult>> GetGroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            return _underlyingService.GetGroupBy<TGroupKey, TResult>(options, keySelector, resultSelector);
        }

        public Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync(key1, key2, cancellationToken);
        }

        public Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync(key1, key2, fetchStrategy, cancellationToken);
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync(predicate, cancellationToken);
        }

        public Task<TEntity> GetAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync(options, cancellationToken);
        }

        public Task<TResult> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync<TResult>(predicate, selector, cancellationToken);
        }

        public Task<TResult> GetAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync<TResult>(options, selector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync(cancellationToken);
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync(predicate, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TEntity>>> GetAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync(options, cancellationToken);
        }

        public Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync<TResult>(selector, cancellationToken);
        }

        public Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync<TResult>(predicate, selector, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TResult>>> GetAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync<TResult>(options, selector, cancellationToken);
        }

        public Task<bool> GetExistsAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetExistsAsync(key1, key2, cancellationToken);
        }

        public Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetExistsAsync(predicate, cancellationToken);
        }

        public Task<bool> GetExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetExistsAsync(options, cancellationToken);
        }

        public Task<int> GetCountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetCountAsync(cancellationToken);
        }

        public Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetCountAsync(predicate, cancellationToken);
        }

        public Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetCountAsync(options, cancellationToken);
        }

        public Task<Dictionary<TDictionaryKey, TEntity>> GetDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetDictionaryAsync<TDictionaryKey>(keySelector, cancellationToken);
        }

        public Task<IPagedQueryResult<Dictionary<TDictionaryKey, TEntity>>> GetDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetDictionaryAsync<TDictionaryKey>(options, keySelector, cancellationToken);
        }

        public Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetDictionaryAsync<TDictionaryKey, TElement>(keySelector, elementSelector, cancellationToken);
        }

        public Task<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>> GetDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetDictionaryAsync<TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);
        }

        public Task<IEnumerable<TResult>> GetGroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetGroupByAsync<TGroupKey, TResult>(keySelector, resultSelector, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TResult>>> GetGroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetGroupByAsync<TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IReadOnlyService{TEntity, TKey}" />.
    /// </summary>
    [ComVisible(false)]
    internal class ReadOnlyServiceWrapper<TEntity, TKey> : IReadOnlyService<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private readonly IService<TEntity, TKey> _underlyingService;

        #endregion

        #region Constructors

        public ReadOnlyServiceWrapper(IService<TEntity, TKey> service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            _underlyingService = service;
        }

        #endregion

        #region Implementation of IReadOnlyService<TEntity,in TKey>

        public TEntity Get(TKey key)
        {
            return _underlyingService.Get(key);
        }

        public TEntity Get(TKey key, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            return _underlyingService.Get(key, fetchStrategy);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingService.Get(predicate);
        }

        public TEntity Get(IQueryOptions<TEntity> options)
        {
            return _underlyingService.Get(options);
        }

        public TResult Get<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingService.Get<TResult>(predicate, selector);
        }

        public TResult Get<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingService.Get<TResult>(options, selector);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _underlyingService.GetAll();
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingService.GetAll(predicate);
        }

        public IPagedQueryResult<IEnumerable<TEntity>> GetAll(IQueryOptions<TEntity> options)
        {
            return _underlyingService.GetAll(options);
        }

        public IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingService.GetAll(selector);
        }

        public IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingService.GetAll<TResult>(predicate, selector);
        }

        public IPagedQueryResult<IEnumerable<TResult>> GetAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            return _underlyingService.GetAll<TResult>(options, selector);
        }

        public bool GetExists(TKey key)
        {
            return _underlyingService.GetExists(key);
        }

        public bool GetExists(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingService.GetExists(predicate);
        }

        public bool GetExists(IQueryOptions<TEntity> options)
        {
            return _underlyingService.GetExists(options);
        }

        public int GetCount()
        {
            return _underlyingService.GetCount();
        }

        public int GetCount(Expression<Func<TEntity, bool>> predicate)
        {
            return _underlyingService.GetCount(predicate);
        }

        public int GetCount(IQueryOptions<TEntity> options)
        {
            return _underlyingService.GetCount(options);
        }

        public Dictionary<TDictionaryKey, TEntity> GetDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return _underlyingService.GetDictionary<TDictionaryKey>(keySelector);
        }

        public IPagedQueryResult<Dictionary<TDictionaryKey, TEntity>> GetDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            return _underlyingService.GetDictionary<TDictionaryKey>(options, keySelector);
        }

        public Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return _underlyingService.GetDictionary<TDictionaryKey, TElement>(keySelector, elementSelector);
        }

        public IPagedQueryResult<Dictionary<TDictionaryKey, TElement>> GetDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            return _underlyingService.GetDictionary<TDictionaryKey, TElement>(options, keySelector, elementSelector);
        }

        public IEnumerable<TResult> GetGroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            return _underlyingService.GetGroupBy<TGroupKey, TResult>(keySelector, resultSelector);
        }

        public IPagedQueryResult<IEnumerable<TResult>> GetGroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            return _underlyingService.GetGroupBy<TGroupKey, TResult>(options, keySelector, resultSelector);
        }

        public Task<TEntity> GetAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync(key, cancellationToken);
        }

        public Task<TEntity> GetAsync(TKey key, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync(key, fetchStrategy, cancellationToken);
        }

        public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync(predicate, cancellationToken);
        }

        public Task<TEntity> GetAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync(options, cancellationToken);
        }

        public Task<TResult> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync<TResult>(predicate, selector, cancellationToken);
        }

        public Task<TResult> GetAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAsync<TResult>(options, selector, cancellationToken);
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync(cancellationToken);
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync(predicate, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TEntity>>> GetAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync(options, cancellationToken);
        }

        public Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync<TResult>(selector, cancellationToken);
        }

        public Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync<TResult>(predicate, selector, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TResult>>> GetAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetAllAsync<TResult>(options, selector, cancellationToken);
        }

        public Task<bool> GetExistsAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetExistsAsync(key, cancellationToken);
        }

        public Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetExistsAsync(predicate, cancellationToken);
        }

        public Task<bool> GetExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetExistsAsync(options, cancellationToken);
        }

        public Task<int> GetCountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetCountAsync(cancellationToken);
        }

        public Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetCountAsync(predicate, cancellationToken);
        }

        public Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetCountAsync(options, cancellationToken);
        }

        public Task<Dictionary<TDictionaryKey, TEntity>> GetDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetDictionaryAsync<TDictionaryKey>(keySelector, cancellationToken);
        }

        public Task<IPagedQueryResult<Dictionary<TDictionaryKey, TEntity>>> GetDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetDictionaryAsync<TDictionaryKey>(options, keySelector, cancellationToken);
        }

        public Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetDictionaryAsync<TDictionaryKey, TElement>(keySelector, elementSelector, cancellationToken);
        }

        public Task<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>> GetDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetDictionaryAsync<TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);
        }

        public Task<IEnumerable<TResult>> GetGroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetGroupByAsync<TGroupKey, TResult>(keySelector, resultSelector, cancellationToken);
        }

        public Task<IPagedQueryResult<IEnumerable<TResult>>> GetGroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            return _underlyingService.GetGroupByAsync<TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);
        }

        #endregion
    }
}
