namespace DotNetToolkit.Repository.EntityFramework.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Configuration.Logging;
    using Extensions;
    using Helpers;
    using Queries;
    using Queries.Strategies;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Transactions;

    /// <summary>
    /// Represents an internal entity framework repository context.
    /// </summary>
    /// <seealso cref="IRepositoryContextAsync" />
    internal class EfRepositoryContext : IRepositoryContextAsync
    {
        #region Fields

        private readonly DbContext _context;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the repository context logger.
        /// </summary>
        public ILogger Logger { get; private set; } = NullLogger.Instance;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryContext" /> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public EfRepositoryContext(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        #endregion

        #region Private Methods

        private void ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(object[] keyValues) where TEntity : class
        {
            if (keyValues.Length != PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos<TEntity>().Count())
                throw new ArgumentException(DotNetToolkit.Repository.Properties.Resources.EntityPrimaryKeyValuesLengthMismatch, nameof(keyValues));
        }

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
        public IEnumerable<TEntity> ExecuteQuery<TEntity>(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, TEntity> projector) where TEntity : class
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            if (projector == null)
                throw new ArgumentNullException(nameof(projector));

            var connection = _context.Database.Connection;
            var command = connection.CreateCommand();
            var shouldOpenConnection = connection.State != ConnectionState.Open;

            if (shouldOpenConnection)
                connection.Open();

            command.CommandText = sql;
            command.CommandType = cmdType;
            command.Parameters.Clear();

            if (parameters != null && parameters.Any())
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    var p = command.CreateParameter();

                    p.ParameterName = $"@p{i}";
                    p.Value = parameters[i] ?? DBNull.Value;

                    command.Parameters.Add(p);
                }
            }

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

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteQuery(string sql, CommandType cmdType, object[] parameters)
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            var connection = _context.Database.Connection;
            var shouldOpenConnection = connection.State != ConnectionState.Open;

            try
            {
                using (var command = connection.CreateCommand())
                {
                    if (connection.State != ConnectionState.Open)
                        connection.Open();

                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    command.Parameters.Clear();

                    if (parameters != null && parameters.Any())
                    {
                        for (var i = 0; i < parameters.Length; i++)
                        {
                            var p = command.CreateParameter();

                            p.ParameterName = $"@p{i}";
                            p.Value = parameters[i] ?? DBNull.Value;

                            command.Parameters.Add(p);
                        }
                    }

                    return command.ExecuteNonQuery();
                }
            }
            finally
            {

                if (shouldOpenConnection)
                    connection.Close();
            }
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns>The transaction.</returns>
        public ITransactionManager BeginTransaction()
        {
            CurrentTransaction = new EfTransactionManager(_context.Database.BeginTransaction());

            return CurrentTransaction;
        }

        /// <summary>
        /// Gets the current transaction.
        /// </summary>
        public ITransactionManager CurrentTransaction { get; private set; }

        /// <summary>
        /// Sets the repository context logger provider to use.
        /// </summary>
        /// <param name="loggerProvider">The logger provider.</param>
        public void UseLoggerProvider(ILoggerProvider loggerProvider)
        {
            if (loggerProvider == null)
                throw new ArgumentNullException(nameof(loggerProvider));

            Logger = loggerProvider.Create(typeof(DbContext).FullName);

            _context.Database.Log = s => Logger.Debug(s.TrimEnd(Environment.NewLine.ToCharArray()));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be inserted into the database when <see cref="M:DotNetToolkit.Repository.IContext.SaveChanges" /> is called..
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _context.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be updated in the database when <see cref="M:DotNetToolkit.Repository.IContext.SaveChanges" /> is called..
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var entry = _context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                var keyValues = PrimaryKeyConventionHelper.GetPrimaryKeyValues(entity);

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

        /// <summary>
        /// Tracks the specified entity in memory and will be removed from the database when <see cref="M:DotNetToolkit.Repository.IContext.SaveChanges" /> is called..
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (_context.Entry(entity).State == EntityState.Detached)
            {
                var keyValues = PrimaryKeyConventionHelper.GetPrimaryKeyValues(entity);

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

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>
        /// The number of state entries written to the database.
        /// </returns>
        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        /// <summary>
        /// Finds an entity with the given primary key values in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found in the repository.</returns>
        public QueryResult<TEntity> Find<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(keyValues);

            if (fetchStrategy == null)
            {
                var result = _context.Set<TEntity>().Find(keyValues);

                return new QueryResult<TEntity>(result);
            }

            var options = new QueryOptions<TEntity>()
                .Include(PrimaryKeyConventionHelper.GetByPrimaryKeySpecification<TEntity>(keyValues))
                .Include(fetchStrategy);

            return Find<TEntity, TEntity>(options, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public QueryResult<TResult> Find<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var result = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
                .ApplySortingOptions(options)
                .ApplyPagingOptions(options)
                .Select(selector)
                .FirstOrDefault();

            return new QueryResult<TResult>(result);
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public QueryResult<IEnumerable<TResult>> FindAll<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var query = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
                .ApplySortingOptions(options);

            var data = query
                .ApplyPagingOptions(options)
                .Select(selector)
                .Select(x => new
                {
                    Result = x,
                    Total = query.Count()
                })
                .ToList();

            var result = data.Select(x => x.Result);
            var total = data.FirstOrDefault()?.Total ?? 0;

            return new QueryResult<IEnumerable<TResult>>(result, total);
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public QueryResult<int> Count<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            var result = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
                .ApplySortingOptions(options)
                .ApplyPagingOptions(options)
                .Count();

            return new QueryResult<int>(result);
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public QueryResult<bool> Exists<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var result = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
                .ApplySortingOptions(options)
                .ApplyPagingOptions(options)
                .Any();

            return new QueryResult<bool>(result);
        }

        /// <summary>
        /// Returns a new <see cref="T:System.Collections.Generic.Dictionary`2" /> according to the specified <paramref name="keySelector" />, and an element selector function with entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public QueryResult<Dictionary<TDictionaryKey, TElement>> ToDictionary<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            var query = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
                .ApplySortingOptions(options);

            Dictionary<TDictionaryKey, TElement> result;
            int total;

            if (options != null && options.PageSize != -1)
            {
                // Tries to get the count in one query
                var data = query
                    .ApplyPagingOptions(options)
                    .Select(x => new
                    {
                        Result = x,
                        Total = query.Count()
                    });

                result = data.Select(x => x.Result).ToDictionary(keySelectFunc, elementSelectorFunc);
                total = data.FirstOrDefault()?.Total ?? 0;
            }
            else
            {
                // Gets the total count from memory
                result = query.ToDictionary(keySelectFunc, elementSelectorFunc);
                total = result.Count;
            }

            return new QueryResult<Dictionary<TDictionaryKey, TElement>>(result, total);
        }

        /// <summary>
        /// Returns a new <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <returns>A new <see cref="T:System.Linq.IGrouping`2" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public QueryResult<IEnumerable<TResult>> GroupBy<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            var keySelectFunc = keySelector.Compile();
            var resultSelectorFunc = resultSelector.Compile();

            var query = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
                .ApplySortingOptions(options);

            var data = query
                .ApplyPagingOptions(options)
                .Select(x => new
                {
                    Result = x,
                    Total = query.Count()
                });

            var result = data.Select(x => x.Result).GroupBy(keySelectFunc, resultSelectorFunc).ToList();
            var total = data.FirstOrDefault()?.Total ?? 0;

            return new QueryResult<IEnumerable<TResult>>(result, total);
        }

        #endregion

        #region Implementation of IRepositoryContextAsync

        /// <summary>
        /// Asynchronously creates raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns> 
        public async Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(string sql, CommandType cmdType, object[] parameters, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            if (projector == null)
                throw new ArgumentNullException(nameof(projector));

            var connection = _context.Database.Connection;
            var command = connection.CreateCommand();
            var shouldOpenConnection = connection.State != ConnectionState.Open;

            if (shouldOpenConnection)
                await connection.OpenAsync(cancellationToken);

            command.CommandText = sql;
            command.CommandType = cmdType;
            command.Parameters.Clear();

            if (parameters != null && parameters.Any())
            {
                for (var i = 0; i < parameters.Length; i++)
                {
                    var p = command.CreateParameter();

                    p.ParameterName = $"@p{i}";
                    p.Value = parameters[i] ?? DBNull.Value;

                    command.Parameters.Add(p);
                }
            }

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

        /// <summary>
        /// Asynchronously creates raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        public async Task<int> ExecuteQueryAsync(string sql, CommandType cmdType, object[] parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            var connection = _context.Database.Connection;
            var shouldOpenConnection = connection.State != ConnectionState.Open;

            try
            {
                using (var command = connection.CreateCommand())
                {
                    if (connection.State != ConnectionState.Open)
                        await connection.OpenAsync(cancellationToken);

                    command.CommandText = sql;
                    command.CommandType = cmdType;
                    command.Parameters.Clear();

                    if (parameters != null && parameters.Any())
                    {
                        for (var i = 0; i < parameters.Length; i++)
                        {
                            var p = command.CreateParameter();

                            p.ParameterName = $"@p{i}";
                            p.Value = parameters[i] ?? DBNull.Value;

                            command.Parameters.Add(p);
                        }
                    }

                    return await command.ExecuteNonQueryAsync(cancellationToken);
                }
            }
            finally
            {

                if (shouldOpenConnection)
                    connection.Close();
            }
        }

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of state entries written to the database.</returns>
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds an entity with the given primary key values in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found in the repository.</returns>
        public async Task<QueryResult<TEntity>> FindAsync<TEntity>(CancellationToken cancellationToken, IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(keyValues);

            if (fetchStrategy == null)
            {
                var result = await _context.Set<TEntity>().FindAsync(cancellationToken, keyValues);

                return new QueryResult<TEntity>(result);
            }

            var options = new QueryOptions<TEntity>()
                .Include(PrimaryKeyConventionHelper.GetByPrimaryKeySpecification<TEntity>(keyValues))
                .Include(fetchStrategy);

            return await FindAsync<TEntity, TEntity>(options, IdentityExpression<TEntity>.Instance, cancellationToken);
        }

        /// <summary>
        /// Asynchronously finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public async Task<QueryResult<TResult>> FindAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var result = await _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
                .ApplySortingOptions(options)
                .ApplyPagingOptions(options)
                .Select(selector)
                .FirstOrDefaultAsync(cancellationToken);

            return new QueryResult<TResult>(result);
        }

        /// <summary>
        /// Asynchronously finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public async Task<QueryResult<IEnumerable<TResult>>> FindAllAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var query = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
                .ApplySortingOptions(options);

            var data = await query
                .ApplyPagingOptions(options)
                .Select(selector)
                .Select(x => new
                {
                    Result = x,
                    Total = query.Count()
                })
                .ToListAsync(cancellationToken);

            var result = data.Select(x => x.Result);
            var total = data.FirstOrDefault()?.Total ?? 0;

            return new QueryResult<IEnumerable<TResult>>(result, total);
        }

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public async Task<QueryResult<int>> CountAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            var result = await _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
                .ApplySortingOptions(options)
                .ApplyPagingOptions(options)
                .CountAsync(cancellationToken);

            return new QueryResult<int>(result);
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public async Task<QueryResult<bool>> ExistsAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            var result = await _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
                .ApplySortingOptions(options)
                .ApplyPagingOptions(options)
                .AnyAsync(cancellationToken);

            return new QueryResult<bool>(result);
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="T:System.Collections.Generic.Dictionary`2" /> according to the specified <paramref name="keySelector" />, and an element selector function with entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="elementSelector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="T:System.Collections.Generic.Dictionary`2" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public async Task<QueryResult<Dictionary<TDictionaryKey, TElement>>> ToDictionaryAsync<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            var query = _context.Set<TEntity>()
                .AsQueryable()
                .ApplySpecificationOptions(options)
                .ApplyFetchingOptions(options)
                .ApplySortingOptions(options);

            Dictionary<TDictionaryKey, TElement> result;
            int total;

            if (options != null && options.PageSize != -1)
            {
                // Tries to get the count in one query
                var data = query
                    .ApplyPagingOptions(options)
                    .Select(x => new
                    {
                        Result = x,
                        Total = query.Count()
                    });

                result = await data.Select(x => x.Result).ToDictionaryAsync(keySelectFunc, elementSelectorFunc, cancellationToken);
                total = (await data.FirstOrDefaultAsync(cancellationToken))?.Total ?? 0;
            }
            else
            {
                // Gets the total count from memory
                result = await query.ToDictionaryAsync(keySelectFunc, elementSelectorFunc, cancellationToken);
                total = result.Count;
            }

            return new QueryResult<Dictionary<TDictionaryKey, TElement>>(result, total);
        }

        /// <summary>
        /// Asynchronously returns a new <see cref="T:System.Collections.Generic.IEnumerable`1" /> according to the specified <paramref name="keySelector" />, and an element selector function.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by resultSelector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="keySelector">A function to extract a key from each entity.</param>
        /// <param name="resultSelector">A function to project each entity into a new form</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="T:System.Linq.IGrouping`2" /> that contains keys and values that satisfies the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public Task<QueryResult<IEnumerable<TResult>>> GroupByAsync<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            return Task.FromResult<QueryResult<IEnumerable<TResult>>>(GroupBy<TEntity, TGroupKey, TResult>(options, keySelector, resultSelector));
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
        }

        #endregion

    }
}