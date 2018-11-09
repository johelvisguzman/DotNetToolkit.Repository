namespace DotNetToolkit.Repository
{
    using Configuration.Conventions;
    using Factories;
    using Queries;
    using Queries.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// An implementation of <see cref="IService{TEntity, TKey1, TKey2, TKey3}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    /// <typeparam name="TKey3">The type of the third part of the composite primary key.</typeparam>
    public abstract class ServiceBase<TEntity, TKey1, TKey2, TKey3> : IService<TEntity, TKey1, TKey2, TKey3> where TEntity : class
    {
        #region Properties

        /// <summary>
        /// Gets the unit of work factory.
        /// </summary>
        protected IUnitOfWorkFactory UnitOfWorkFactory { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase{TEntity, TKey1, TKey2, TKey3}"/> class.
        /// </summary>
        /// <param name="unitOfWorkFactory">The unit of work factory.</param>
        protected ServiceBase(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            UnitOfWorkFactory = unitOfWorkFactory;

            PrimaryKeyConventionHelper.ThrowsIfInvalidPrimaryKeyDefinition<TEntity>(typeof(TKey1), typeof(TKey2), typeof(TKey3));
        }

        #endregion

        #region Implementation of IService<TEntity, TKey1, TKey2, TKey3>

        /// <inheritdoc />
        public virtual void Create(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Add(entity);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Create(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Add(entities);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual bool TryDelete(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2, TKey3>().TryDelete(key1, key2, key3);

                uow.Commit();

                return result;
            }
        }

        /// <inheritdoc />
        public virtual void Delete(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Delete(key1, key2, key3);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Delete(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Delete(entity);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Delete(predicate);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Delete(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Delete(options);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Delete(entities);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Update(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Update(entity);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Update(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2, TKey3>().Update(entities);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual TEntity Get(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().Find(key1, key2, key3);
            }
        }

        /// <inheritdoc />
        public virtual TEntity Get(TKey1 key1, TKey2 key2, TKey3 key3, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().Find(key1, key2, key3, fetchStrategy);
            }
        }

        /// <inheritdoc />
        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().Find(predicate);
            }
        }

        /// <inheritdoc />
        public virtual TEntity Get(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().Find(options);
            }
        }

        /// <inheritdoc />
        public virtual TResult Get<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().Find<TResult>(predicate, selector);
            }
        }

        /// <inheritdoc />
        public virtual TResult Get<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().Find<TResult>(options, selector);
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> GetAll()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAll();
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAll(predicate);
            }
        }

        /// <inheritdoc />
        public virtual IQueryResult<IEnumerable<TEntity>> GetAll(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAll(options);
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAll<TResult>(selector);
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAll<TResult>(predicate, selector);
            }
        }

        /// <inheritdoc />
        public virtual IQueryResult<IEnumerable<TResult>> GetAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAll<TResult>(options, selector);
            }
        }

        /// <inheritdoc />
        public virtual bool GetExists(TKey1 key1, TKey2 key2, TKey3 key3)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().Exists(key1, key2, key3);
            }
        }

        /// <inheritdoc />
        public virtual bool GetExists(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().Exists(predicate);
            }
        }

        /// <inheritdoc />
        public virtual bool GetExists(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().Exists(options);
            }
        }

        /// <inheritdoc />
        public virtual int GetCount()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().Count();
            }
        }

        /// <inheritdoc />
        public virtual int GetCount(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().Count(predicate);
            }
        }

        /// <inheritdoc />
        public virtual int GetCount(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().Count(options);
            }
        }

        /// <inheritdoc />
        public virtual Dictionary<TDictionaryKey, TEntity> GetDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionary<TDictionaryKey>(keySelector);
            }
        }

        /// <inheritdoc />
        public virtual IQueryResult<Dictionary<TDictionaryKey, TEntity>> GetDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionary<TDictionaryKey>(options, keySelector);
            }
        }

        /// <inheritdoc />
        public virtual Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionary<TDictionaryKey, TElement>(keySelector, elementSelector);
            }
        }

        /// <inheritdoc />
        public virtual IQueryResult<Dictionary<TDictionaryKey, TElement>> GetDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionary<TDictionaryKey, TElement>(options, keySelector, elementSelector);
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<TResult> GetGroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().GroupBy<TGroupKey, TResult>(keySelector, resultSelector);
            }
        }

        /// <inheritdoc />
        public virtual IQueryResult<IEnumerable<TResult>> GetGroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2, TKey3>().GroupBy<TGroupKey, TResult>(options, keySelector, resultSelector);
            }
        }

        /// <inheritdoc />
        public virtual async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().AddAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task CreateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().AddAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> TryDeleteAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2, TKey3>().TryDeleteAsync(key1, key2, key3, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().DeleteAsync(key1, key2, key3, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().DeleteAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().DeleteAsync(predicate, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().DeleteAsync(options, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().DeleteAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().UpdateAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2, TKey3>().UpdateAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAsync(key1, key2, key3, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, TKey3 key3, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAsync(key1, key2, key3, fetchStrategy, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAsync(predicate, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAsync(options, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TResult> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAsync<TResult>(predicate, selector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TResult> GetAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAsync<TResult>(options, selector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAllAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAllAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IQueryResult<IEnumerable<TEntity>>> GetAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAllAsync(options, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAllAsync<TResult>(selector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAllAsync<TResult>(predicate, selector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IQueryResult<IEnumerable<TResult>>> GetAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().FindAllAsync<TResult>(options, selector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> GetExistsAsync(TKey1 key1, TKey2 key2, TKey3 key3, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().ExistsAsync(key1, key2, key3, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().ExistsAsync(predicate, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> GetExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().ExistsAsync(options, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> GetCountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().CountAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().CountAsync(predicate, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().CountAsync(options, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<Dictionary<TDictionaryKey, TEntity>> GetDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionaryAsync<TDictionaryKey>(keySelector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IQueryResult<Dictionary<TDictionaryKey, TEntity>>> GetDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionaryAsync<TDictionaryKey>(options, keySelector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionaryAsync<TDictionaryKey, TElement>(keySelector, elementSelector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IQueryResult<Dictionary<TDictionaryKey, TElement>>> GetDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().ToDictionaryAsync<TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TResult>> GetGroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().GroupByAsync<TGroupKey, TResult>(keySelector, resultSelector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IQueryResult<IEnumerable<TResult>>> GetGroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2, TKey3>().GroupByAsync<TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);
            }
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IService{TEntity, TKey1, TKey2}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey1">The type of the first part of the composite primary key.</typeparam>
    /// <typeparam name="TKey2">The type of the second part of the composite primary key.</typeparam>
    public abstract class ServiceBase<TEntity, TKey1, TKey2> : IService<TEntity, TKey1, TKey2> where TEntity : class
    {
        #region Properties

        /// <summary>
        /// Gets the unit of work factory.
        /// </summary>
        protected IUnitOfWorkFactory UnitOfWorkFactory { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase{TEntity, TKey1, TKey2}"/> class.
        /// </summary>
        /// <param name="unitOfWorkFactory">The unit of work factory.</param>
        protected ServiceBase(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            UnitOfWorkFactory = unitOfWorkFactory;

            PrimaryKeyConventionHelper.ThrowsIfInvalidPrimaryKeyDefinition<TEntity>(typeof(TKey1), typeof(TKey2));
        }

        #endregion

        #region Implementation of IService<TEntity, TKey1, TKey2>

        /// <inheritdoc />
        public virtual void Create(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Add(entity);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Create(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Add(entities);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual bool TryDelete(TKey1 key1, TKey2 key2)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey1, TKey2>().TryDelete(key1, key2);

                uow.Commit();

                return result;
            }
        }

        /// <inheritdoc />
        public virtual void Delete(TKey1 key1, TKey2 key2)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Delete(key1, key2);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Delete(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Delete(entity);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Delete(predicate);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Delete(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Delete(options);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Delete(entities);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Update(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Update(entity);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Update(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey1, TKey2>().Update(entities);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual TEntity Get(TKey1 key1, TKey2 key2)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().Find(key1, key2);
            }
        }

        /// <inheritdoc />
        public virtual TEntity Get(TKey1 key1, TKey2 key2, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().Find(key1, key2, fetchStrategy);
            }
        }

        /// <inheritdoc />
        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().Find(predicate);
            }
        }

        /// <inheritdoc />
        public virtual TEntity Get(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().Find(options);
            }
        }

        /// <inheritdoc />
        public virtual TResult Get<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().Find<TResult>(predicate, selector);
            }
        }

        /// <inheritdoc />
        public virtual TResult Get<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().Find<TResult>(options, selector);
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> GetAll()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().FindAll();
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().FindAll(predicate);
            }
        }

        /// <inheritdoc />
        public virtual IQueryResult<IEnumerable<TEntity>> GetAll(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().FindAll(options);
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().FindAll<TResult>(selector);
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().FindAll<TResult>(predicate, selector);
            }
        }

        /// <inheritdoc />
        public virtual IQueryResult<IEnumerable<TResult>> GetAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().FindAll<TResult>(options, selector);
            }
        }

        /// <inheritdoc />
        public virtual bool GetExists(TKey1 key1, TKey2 key2)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().Exists(key1, key2);
            }
        }

        /// <inheritdoc />
        public virtual bool GetExists(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().Exists(predicate);
            }
        }

        /// <inheritdoc />
        public virtual bool GetExists(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().Exists(options);
            }
        }

        /// <inheritdoc />
        public virtual int GetCount()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().Count();
            }
        }

        /// <inheritdoc />
        public virtual int GetCount(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().Count(predicate);
            }
        }

        /// <inheritdoc />
        public virtual int GetCount(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().Count(options);
            }
        }

        /// <inheritdoc />
        public virtual Dictionary<TDictionaryKey, TEntity> GetDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().ToDictionary<TDictionaryKey>(keySelector);
            }
        }

        /// <inheritdoc />
        public virtual IQueryResult<Dictionary<TDictionaryKey, TEntity>> GetDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().ToDictionary<TDictionaryKey>(options, keySelector);
            }
        }

        /// <inheritdoc />
        public virtual Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().ToDictionary<TDictionaryKey, TElement>(keySelector, elementSelector);
            }
        }

        /// <inheritdoc />
        public virtual IQueryResult<Dictionary<TDictionaryKey, TElement>> GetDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().ToDictionary<TDictionaryKey, TElement>(options, keySelector, elementSelector);
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<TResult> GetGroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().GroupBy<TGroupKey, TResult>(keySelector, resultSelector);
            }
        }

        /// <inheritdoc />
        public virtual IQueryResult<IEnumerable<TResult>> GetGroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey1, TKey2>().GroupBy<TGroupKey, TResult>(options, keySelector, resultSelector);
            }
        }

        /// <inheritdoc />
        public virtual async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().AddAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task CreateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().AddAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> TryDeleteAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey1, TKey2>().TryDeleteAsync(key1, key2, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().DeleteAsync(key1, key2, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().DeleteAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().DeleteAsync(predicate, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().DeleteAsync(options, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().DeleteAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().UpdateAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey1, TKey2>().UpdateAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public async Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().FindAsync(key1, key2, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetAsync(TKey1 key1, TKey2 key2, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().FindAsync(key1, key2, fetchStrategy, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().FindAsync(predicate, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().FindAsync(options, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TResult> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().FindAsync<TResult>(predicate, selector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TResult> GetAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().FindAsync<TResult>(options, selector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().FindAllAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().FindAllAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IQueryResult<IEnumerable<TEntity>>> GetAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().FindAllAsync(options, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().FindAllAsync<TResult>(selector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().FindAllAsync<TResult>(predicate, selector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IQueryResult<IEnumerable<TResult>>> GetAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().FindAllAsync<TResult>(options, selector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> GetExistsAsync(TKey1 key1, TKey2 key2, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().ExistsAsync(key1, key2, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().ExistsAsync(predicate, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> GetExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().ExistsAsync(options, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> GetCountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().CountAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().CountAsync(predicate, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().CountAsync(options, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<Dictionary<TDictionaryKey, TEntity>> GetDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().ToDictionaryAsync<TDictionaryKey>(keySelector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IQueryResult<Dictionary<TDictionaryKey, TEntity>>> GetDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().ToDictionaryAsync<TDictionaryKey>(options, keySelector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().ToDictionaryAsync<TDictionaryKey, TElement>(keySelector, elementSelector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IQueryResult<Dictionary<TDictionaryKey, TElement>>> GetDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().ToDictionaryAsync<TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TResult>> GetGroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().GroupByAsync<TGroupKey, TResult>(keySelector, resultSelector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IQueryResult<IEnumerable<TResult>>> GetGroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey1, TKey2>().GroupByAsync<TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);
            }
        }

        #endregion
    }

    /// <summary>
    /// An implementation of <see cref="IService{TEntity, TKey}" />.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public abstract class ServiceBase<TEntity, TKey> : IService<TEntity, TKey> where TEntity : class
    {
        #region Properties

        /// <summary>
        /// Gets the unit of work factory.
        /// </summary>
        protected IUnitOfWorkFactory UnitOfWorkFactory { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="unitOfWorkFactory">The unit of work factory.</param>
        protected ServiceBase(IUnitOfWorkFactory unitOfWorkFactory)
        {
            if (unitOfWorkFactory == null)
                throw new ArgumentNullException(nameof(unitOfWorkFactory));

            UnitOfWorkFactory = unitOfWorkFactory;

            PrimaryKeyConventionHelper.ThrowsIfInvalidPrimaryKeyDefinition<TEntity>(typeof(TKey));
        }

        #endregion

        #region Implementation of IService<TEntity, TKey>

        /// <inheritdoc />
        public virtual void Create(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Add(entity);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Create(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Add(entities);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual bool TryDelete(TKey key)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = uow.Create<TEntity, TKey>().TryDelete(key);

                uow.Commit();

                return result;
            }
        }

        /// <inheritdoc />
        public virtual void Delete(TKey key)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Delete(key);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Delete(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Delete(entity);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Delete(predicate);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Delete(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Delete(options);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Delete(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Delete(entities);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Update(TEntity entity)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Update(entity);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual void Update(IEnumerable<TEntity> entities)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                uow.Create<TEntity, TKey>().Update(entities);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual TEntity Get(TKey key)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().Find(key);
            }
        }

        /// <inheritdoc />
        public virtual TEntity Get(TKey key, IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().Find(key, fetchStrategy);
            }
        }

        /// <inheritdoc />
        public virtual TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().Find(predicate);
            }
        }

        /// <inheritdoc />
        public virtual TEntity Get(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().Find(options);
            }
        }

        /// <inheritdoc />
        public virtual TResult Get<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().Find<TResult>(predicate, selector);
            }
        }

        /// <inheritdoc />
        public virtual TResult Get<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().Find<TResult>(options, selector);
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> GetAll()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().FindAll();
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().FindAll(predicate);
            }
        }

        /// <inheritdoc />
        public virtual IQueryResult<IEnumerable<TEntity>> GetAll(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().FindAll(options);
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().FindAll<TResult>(selector);
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<TResult> GetAll<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().FindAll<TResult>(predicate, selector);
            }
        }

        /// <inheritdoc />
        public virtual IQueryResult<IEnumerable<TResult>> GetAll<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().FindAll<TResult>(options, selector);
            }
        }

        /// <inheritdoc />
        public virtual bool GetExists(TKey key)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().Exists(key);
            }
        }

        /// <inheritdoc />
        public virtual bool GetExists(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().Exists(predicate);
            }
        }

        /// <inheritdoc />
        public virtual bool GetExists(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().Exists(options);
            }
        }

        /// <inheritdoc />
        public virtual int GetCount()
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().Count();
            }
        }

        /// <inheritdoc />
        public virtual int GetCount(Expression<Func<TEntity, bool>> predicate)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().Count(predicate);
            }
        }

        /// <inheritdoc />
        public virtual int GetCount(IQueryOptions<TEntity> options)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().Count(options);
            }
        }

        /// <inheritdoc />
        public virtual Dictionary<TDictionaryKey, TEntity> GetDictionary<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().ToDictionary<TDictionaryKey>(keySelector);
            }
        }

        /// <inheritdoc />
        public virtual IQueryResult<Dictionary<TDictionaryKey, TEntity>> GetDictionary<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().ToDictionary<TDictionaryKey>(options, keySelector);
            }
        }

        /// <inheritdoc />
        public virtual Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().ToDictionary<TDictionaryKey, TElement>(keySelector, elementSelector);
            }
        }

        /// <inheritdoc />
        public virtual IQueryResult<Dictionary<TDictionaryKey, TElement>> GetDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().ToDictionary<TDictionaryKey, TElement>(options, keySelector, elementSelector);
            }
        }

        /// <inheritdoc />
        public virtual IEnumerable<TResult> GetGroupBy<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().GroupBy<TGroupKey, TResult>(keySelector, resultSelector);
            }
        }

        /// <inheritdoc />
        public virtual IQueryResult<IEnumerable<TResult>> GetGroupBy<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector)
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return uow.Create<TEntity, TKey>().GroupBy<TGroupKey, TResult>(options, keySelector, resultSelector);
            }
        }

        /// <inheritdoc />
        public virtual async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().AddAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task CreateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().AddAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> TryDeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                var result = await uow.Create<TEntity, TKey>().TryDeleteAsync(key, cancellationToken);

                uow.Commit();

                return result;
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().DeleteAsync(key, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().DeleteAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().DeleteAsync(predicate, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().DeleteAsync(options, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().DeleteAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().UpdateAsync(entity, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public virtual async Task UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                await uow.Create<TEntity, TKey>().UpdateAsync(entities, cancellationToken);
                uow.Commit();
            }
        }

        /// <inheritdoc />
        public async Task<TEntity> GetAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().FindAsync(key, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetAsync(TKey key, IFetchQueryStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().FindAsync(key, fetchStrategy, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().FindAsync(predicate, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> GetAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().FindAsync(options, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TResult> GetAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().FindAsync<TResult>(predicate, selector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<TResult> GetAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().FindAsync<TResult>(options, selector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().FindAllAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().FindAllAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IQueryResult<IEnumerable<TEntity>>> GetAllAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().FindAllAsync(options, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().FindAllAsync<TResult>(selector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TResult>> GetAllAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().FindAllAsync<TResult>(predicate, selector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IQueryResult<IEnumerable<TResult>>> GetAllAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().FindAllAsync<TResult>(options, selector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> GetExistsAsync(TKey key, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().ExistsAsync(key, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> GetExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().ExistsAsync(predicate, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<bool> GetExistsAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().ExistsAsync(options, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> GetCountAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().CountAsync(cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().CountAsync(predicate, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().CountAsync(options, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<Dictionary<TDictionaryKey, TEntity>> GetDictionaryAsync<TDictionaryKey>(Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().ToDictionaryAsync<TDictionaryKey>(keySelector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IQueryResult<Dictionary<TDictionaryKey, TEntity>>> GetDictionaryAsync<TDictionaryKey>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().ToDictionaryAsync<TDictionaryKey>(options, keySelector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().ToDictionaryAsync<TDictionaryKey, TElement>(keySelector, elementSelector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IQueryResult<Dictionary<TDictionaryKey, TElement>>> GetDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().ToDictionaryAsync<TDictionaryKey, TElement>(options, keySelector, elementSelector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TResult>> GetGroupByAsync<TGroupKey, TResult>(Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().GroupByAsync<TGroupKey, TResult>(keySelector, resultSelector, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<IQueryResult<IEnumerable<TResult>>> GetGroupByAsync<TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = UnitOfWorkFactory.Create())
            {
                return await uow.Create<TEntity, TKey>().GroupByAsync<TGroupKey, TResult>(options, keySelector, resultSelector, cancellationToken);
            }
        }

        #endregion
    }
}
