namespace DotNetToolkit.Repository.EntityFrameworkCore.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Extensions;
    using Extensions.Internal;
    using Microsoft.EntityFrameworkCore;
    using Properties;
    using Query;
    using Query.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Transactions;
    using Utility;

    internal class EfCoreRepositoryContext : LinqQueryableRepositoryContextBase, IEfCoreRepositoryContext
    {
        #region Fields

        private readonly DbContext _context;

        #endregion

        #region Constructors

        public EfCoreRepositoryContext(DbContext context)
        {
            _context = Guard.NotNull(context, nameof(context));
            _context.ConfigureLogging(s => Logger.Debug(s.TrimEnd(Environment.NewLine.ToCharArray())));

            ConfigureConventions(context);
        }

        #endregion

        #region Private Methods

        private void ConfigureConventions(DbContext context)
        {
            var helper = new EfCoreRepositoryConventionHelper(context);

            Conventions = new RepositoryConventions()
            {
                PrimaryKeysCallback = type => helper.GetPrimaryKeyPropertyInfos(type)
            };
        }

        #endregion

        #region Overrides of LinqQueryableRepositoryContextBase

        protected override IQueryable<TEntity> AsQueryable<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy)
        {
            return _context
                .Set<TEntity>()
                .AsQueryable()
                .ApplyFetchingOptions(Conventions, fetchStrategy);
        }

        #endregion

        #region Implementation of IEfCoreRepositoryContext

        public DbContext UnderlyingContext { get { return _context; } }

        #endregion

        #region Implementation of IRepositoryContext

        public override IEnumerable<TEntity> ExecuteSqlQuery<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, TEntity> projector)
        {
            Guard.NotEmpty(sql, nameof(sql));
            Guard.NotNull(projector, nameof(projector));

            var connection = _context.Database.GetDbConnection();
            var command = connection.CreateCommand();
            var shouldOpenConnection = connection.State != ConnectionState.Open;

            if (shouldOpenConnection)
                connection.Open();

            command.CommandText = sql;
            command.CommandType = cmdType;
            command.Parameters.Clear();
            command.AddParameters(parameters);

            using (var reader = command.ExecuteReader(shouldOpenConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default))
            {
                var list = new List<TEntity>();

                while (reader.Read())
                {
                    list.Add(projector(reader));
                }

                return list;
            }
        }

        public override int ExecuteSqlCommand(string sql, CommandType cmdType, Dictionary<string, object> parameters)
        {
            Guard.NotEmpty(sql, nameof(sql));

            var connection = _context.Database.GetDbConnection();
            var shouldOpenConnection = connection.State != ConnectionState.Open;

            if (shouldOpenConnection)
                connection.Open();

            try
            {
                using (var command = connection.CreateCommand())
                {
                    if (shouldOpenConnection)
                        connection.Open();

                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    command.Parameters.Clear();
                    command.AddParameters(parameters);

                    return command.ExecuteNonQuery();
                }
            }
            finally
            {

                if (shouldOpenConnection)
                    connection.Close();
            }
        }

        public override ITransactionManager BeginTransaction()
        {
            CurrentTransaction = new EfCoreTransactionManager(_context.Database.BeginTransaction());

            return CurrentTransaction;
        }

        public override void Add<TEntity>(TEntity entity)
        {
            _context.Set<TEntity>().Add(Guard.NotNull(entity, nameof(entity)));
        }

        public override void Update<TEntity>(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            var entry = _context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                var keyValues = Conventions.GetPrimaryKeyValues(entity);

                var entityInDb = _context.Set<TEntity>().Find(keyValues);

                if (entityInDb != null)
                {
                    _context.Entry(entityInDb).CurrentValues.SetValues(entity);
                }
            }
            else
            {
                entry.State = EntityState.Modified;
            }
        }

        public override void Remove<TEntity>(TEntity entity)
        {
            Guard.NotNull(entity, nameof(entity));

            if (_context.Entry(entity).State == EntityState.Detached)
            {
                var keyValues = Conventions.GetPrimaryKeyValues(entity);

                var entityInDb = _context.Set<TEntity>().Find(keyValues);

                if (entityInDb != null)
                {
                    _context.Set<TEntity>().Remove(entityInDb);
                }
            }
            else
            {
                _context.Set<TEntity>().Remove(entity);
            }
        }

        public override int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public override TEntity Find<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues)
        {
            Guard.NotEmpty(keyValues, nameof(keyValues));

            if (fetchStrategy == null)
            {
                var result = _context.Set<TEntity>().Find(keyValues);

                return result;
            }

            return base.Find(fetchStrategy, keyValues);
        }

        #endregion

        #region Implementation of IRepositoryContextAsync

        public async Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            await _context.Set<TEntity>().AddAsync(Guard.NotNull(entity, nameof(entity)), cancellationToken);
        }

        public async Task UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            Guard.NotNull(entity, nameof(entity));

            var entry = _context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                var keyValues = Conventions.GetPrimaryKeyValues(entity);

                var entityInDb = await _context.Set<TEntity>().FindAsync(keyValues, cancellationToken);

                if (entityInDb != null)
                {
                    _context.Entry(entityInDb).CurrentValues.SetValues(entity);
                }
            }
            else
            {
                entry.State = EntityState.Modified;
            }
        }

        public async Task RemoveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            Guard.NotNull(entity, nameof(entity));

            if (_context.Entry(entity).State == EntityState.Detached)
            {
                var keyValues = Conventions.GetPrimaryKeyValues(entity);

                var entityInDb = await _context.Set<TEntity>().FindAsync(keyValues, cancellationToken);

                if (entityInDb != null)
                {
                    _context.Set<TEntity>().Remove(entityInDb);
                }
            }
            else
            {
                _context.Set<TEntity>().Remove(entity);
            }
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            Guard.NotEmpty(sql, nameof(sql));
            Guard.NotNull(projector, nameof(projector));

            var connection = _context.Database.GetDbConnection();
            var command = connection.CreateCommand();
            var shouldOpenConnection = connection.State != ConnectionState.Open;

            if (shouldOpenConnection)
                await connection.OpenAsync(cancellationToken);

            command.CommandText = sql;
            command.CommandType = cmdType;
            command.Parameters.Clear();
            command.AddParameters(parameters);

            using (var reader = await command.ExecuteReaderAsync(shouldOpenConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default, cancellationToken))
            {
                var list = new List<TEntity>();

                while (await reader.ReadAsync(cancellationToken))
                {
                    list.Add(projector(reader));
                }

                return list;
            }
        }

        public async Task<int> ExecuteSqlCommandAsync(string sql, CommandType cmdType, Dictionary<string, object> parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.NotEmpty(sql, nameof(sql));

            var connection = _context.Database.GetDbConnection();
            var shouldOpenConnection = connection.State != ConnectionState.Open;

            try
            {
                using (var command = connection.CreateCommand())
                {
                    if (shouldOpenConnection)
                        await connection.OpenAsync(cancellationToken);

                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    command.Parameters.Clear();
                    command.AddParameters(parameters);

                    return await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
            finally
            {

                if (shouldOpenConnection)
                    connection.Close();
            }
        }

        public async Task<TEntity> FindAsync<TEntity>(CancellationToken cancellationToken, IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            Guard.NotEmpty(keyValues, nameof(keyValues));

            if (fetchStrategy == null)
            {
                return await _context.Set<TEntity>().FindAsync(keyValues, cancellationToken);
            }

            var options = new QueryOptions<TEntity>()
                .Include(Conventions.GetByPrimaryKeySpecification<TEntity>(keyValues));

            var query = AsQueryable(fetchStrategy);

            var result = await query
                .ApplySpecificationOptions(options)
                .FirstOrDefaultAsync(cancellationToken);

            return result;
        }

        public Task<TResult> FindAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            Guard.NotNull(selector, nameof(selector));

            var result = AsQueryable(options.FetchStrategy)
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options)
                .ApplyPagingOptions(options)
                .Select(selector)
                .FirstOrDefaultAsync(cancellationToken);

            return result;
        }

        public async Task<PagedQueryResult<IEnumerable<TResult>>> FindAllAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            Guard.NotNull(selector, nameof(selector));

            var query = AsQueryable(options.FetchStrategy)
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options);

            var total = await query.CountAsync(cancellationToken);

            var result = await query
                .ApplyPagingOptions(options)
                .Select(selector)
                .ToListAsync(cancellationToken);

            return new PagedQueryResult<IEnumerable<TResult>>(result, total);
        }

        public Task<int> CountAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            var result = AsQueryable(options.FetchStrategy)
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options)
                .ApplyPagingOptions(options)
                .CountAsync(cancellationToken);

            return result;
        }

        public Task<bool> ExistsAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            Guard.NotNull(options, nameof(options));

            var result = AsQueryable(options.FetchStrategy)
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options)
                .ApplyPagingOptions(options)
                .AnyAsync(cancellationToken);

            return result;
        }

        public async Task<PagedQueryResult<Dictionary<TDictionaryKey, TElement>>> ToDictionaryAsync<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            Guard.NotNull(keySelector, nameof(keySelector));
            Guard.NotNull(elementSelector, nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            var query = AsQueryable(options.FetchStrategy)
                .ApplySpecificationOptions(options)
                .ApplySortingOptions(Conventions, options);

            Dictionary<TDictionaryKey, TElement> result;
            int total;

            if (options != null && options.PageSize != -1)
            {
                total = await query.CountAsync(cancellationToken);

                result = await query
                    .ApplyPagingOptions(options)
                    .ToDictionaryAsync(keySelectFunc, elementSelectorFunc, cancellationToken);
            }
            else
            {
                // Gets the total count from memory
                result = await query.ToDictionaryAsync(keySelectFunc, elementSelectorFunc, cancellationToken);
                total = result.Count;
            }

            return new PagedQueryResult<Dictionary<TDictionaryKey, TElement>>(result, total);
        }

        public async Task<PagedQueryResult<IEnumerable<TResult>>> GroupByAsync<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<IGrouping<TGroupKey, TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            Guard.NotNull(keySelector, nameof(keySelector));
            Guard.NotNull(resultSelector, nameof(resultSelector));

            var query = AsQueryable(options.FetchStrategy)
                .ApplySpecificationOptions(options);

            if (options?.SortingProperties.Count > 0)
            {
                throw new InvalidOperationException(Resources.GroupBySortingNotSupported);
            }

            var total = await query.CountAsync(cancellationToken);

            var result = await query
                .ApplyPagingOptions(options)
                .GroupBy(keySelector)
                .OrderBy(x => x.Key)
                .Select(resultSelector)
                .ToListAsync(cancellationToken);

            return new PagedQueryResult<IEnumerable<TResult>>(result, total);
        }

        #endregion

        #region Implementation of IDisposable

        public override void Dispose()
        {
            _context.Dispose();

            base.Dispose();
        }

        #endregion

    }
}