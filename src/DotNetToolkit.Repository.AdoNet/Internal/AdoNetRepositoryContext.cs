﻿namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Configuration.Logging;
    using Extensions;
    using Helpers;
    using Properties;
    using Queries;
    using Queries.Strategies;
    using Schema;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Transactions;

    /// <summary>
    /// Represents an internal ado.net repository context.
    /// </summary>
    /// <seealso cref="IRepositoryContextAsync" />
    internal class AdoNetRepositoryContext : IRepositoryContextAsync
    {
        #region Fields

        private readonly BlockingCollection<EntitySet> _items = new BlockingCollection<EntitySet>();
        private readonly SchemaTableConfigurationHelper _schemaConfigHelper;
        private readonly DbHelper _dbHelper;
        private readonly bool _ensureDatabaseCreated;
        private ILogger _logger = NullLogger.Instance;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the repository context logger.
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set
            {
                _logger = value;

                if (_dbHelper != null)
                    _dbHelper.UseLogger(_logger);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContext" /> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
		/// <param name="ensureDatabaseCreated">
		/// Ensures that the database for the context exists. If it exists, no action is taken.
		/// If it does not exist then the database and all its schema are created.
        /// If the database exists, then no effort is made to ensure it is compatible with the model for this context.
		/// </param>
        public AdoNetRepositoryContext(string nameOrConnectionString, bool ensureDatabaseCreated = false)
        {
            if (nameOrConnectionString == null)
                throw new ArgumentNullException(nameof(nameOrConnectionString));

            _dbHelper = new DbHelper(nameOrConnectionString);
            _schemaConfigHelper = new SchemaTableConfigurationHelper(_dbHelper);
            _ensureDatabaseCreated = ensureDatabaseCreated;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContext" /> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="ensureDatabaseCreated">
		/// Ensures that the database for the context exists. If it exists, no action is taken.
		/// If it does not exist then the database and all its schema are created.
        /// If the database exists, then no effort is made to ensure it is compatible with the model for this context.
		/// </param>
        public AdoNetRepositoryContext(string providerName, string connectionString, bool ensureDatabaseCreated = false)
        {
            if (providerName == null)
                throw new ArgumentNullException(nameof(providerName));

            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            _dbHelper = new DbHelper(providerName, connectionString);
            _schemaConfigHelper = new SchemaTableConfigurationHelper(_dbHelper);
            _ensureDatabaseCreated = ensureDatabaseCreated;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContext" /> class.
        /// </summary>
        /// <param name="existingConnection">The existing connection.</param>
        /// <param name="ensureDatabaseCreated">
		/// Ensures that the database for the context exists. If it exists, no action is taken.
		/// If it does not exist then the database and all its schema are created.
        /// If the database exists, then no effort is made to ensure it is compatible with the model for this context.
		/// </param>
        public AdoNetRepositoryContext(DbConnection existingConnection, bool ensureDatabaseCreated = false)
        {
            if (existingConnection == null)
                throw new ArgumentNullException(nameof(existingConnection));

            _dbHelper = new DbHelper(existingConnection);
            _schemaConfigHelper = new SchemaTableConfigurationHelper(_dbHelper);
            _ensureDatabaseCreated = ensureDatabaseCreated;
        }

        #endregion

        #region Private Methods

        private void PrepareEntitySetQuery(EntitySet entitySet, bool existInDb, out string sql, out Dictionary<string, object> parameters)
        {
            sql = string.Empty;
            parameters = new Dictionary<string, object>();

            switch (entitySet.State)
            {
                case EntityState.Added:
                    {
                        if (existInDb)
                            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityAlreadyBeingTrackedInStore, entitySet.Entity.GetType()));

                        QueryBuilder.CreateInsertStatement(entitySet.Entity, out sql, out parameters);

                        var canGetScopeIdentity = true;

#if NETFULL
                        if (_dbHelper.ProviderType == DataAccessProviderType.SqlServerCompact)
                            canGetScopeIdentity = false;
#endif

                        if (canGetScopeIdentity)
                            sql += $"{Environment.NewLine}SELECT SCOPE_IDENTITY()";

                        break;
                    }
                case EntityState.Removed:
                    {
                        if (!existInDb)
                            throw new InvalidOperationException(Resources.EntityNotFoundInStore);

                        QueryBuilder.CreateDeleteStatement(entitySet.Entity, out sql, out parameters);

                        break;
                    }
                case EntityState.Modified:
                    {
                        if (!existInDb)
                            throw new InvalidOperationException(Resources.EntityNotFoundInStore);

                        QueryBuilder.CreateUpdateStatement(entitySet.Entity, out sql, out parameters);

                        break;
                    }
            }
        }

        private void ExecuteSchemaValidate(Type entityType)
        {
            if (_ensureDatabaseCreated)
                _schemaConfigHelper.ExecuteSchemaValidate(entityType);
        }

        private async Task ExecuteSchemaValidateAsync(Type entityType, CancellationToken cancellationToken)
        {
            if (_ensureDatabaseCreated)
                await _schemaConfigHelper.ExecuteSchemaValidateAsync(entityType, cancellationToken);
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
        public IQueryResult<IEnumerable<TEntity>> ExecuteSqlQuery<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, TEntity> projector) where TEntity : class
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            if (projector == null)
                throw new ArgumentNullException(nameof(projector));

            using (var reader = _dbHelper.ExecuteReader(sql, cmdType, parameters))
            {
                var list = new List<TEntity>();

                while (reader.Read())
                {
                    list.Add(projector(reader));
                }

                return new QueryResult<IEnumerable<TEntity>>(list);
            }
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>The number of rows affected.</returns>
        public IQueryResult<int> ExecuteSqlCommand(string sql, CommandType cmdType, Dictionary<string, object> parameters)
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            return new QueryResult<int>(_dbHelper.ExecuteNonQuery(sql, cmdType, parameters));
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns>The transaction.</returns>
        public ITransactionManager BeginTransaction()
        {
            var transaction = _dbHelper.BeginTransaction();

            CurrentTransaction = new AdoNetTransactionManager(transaction, Logger);

            return CurrentTransaction;
        }

        /// <summary>
        /// Gets the current transaction.
        /// </summary>
        public ITransactionManager CurrentTransaction { get; private set; }

        /// <summary>
        /// Tracks the specified entity in memory and will be inserted into the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _items.Add(new EntitySet(entity, EntityState.Added));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be updated in the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            _items.Add(new EntitySet(entity, EntityState.Modified));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be removed from the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            _items.Add(new EntitySet(entity, EntityState.Removed));
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public int SaveChanges()
        {
            using (var command = _dbHelper.CreateCommand())
            {
                var rows = 0;
                var connection = command.Connection;
                var canCloseConnection = false;
                var ownsConnection = _dbHelper.OwnsConnection;

                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                    canCloseConnection = true;
                }

                try
                {
                    while (_items.TryTake(out var entitySet))
                    {
                        var entityType = entitySet.Entity.GetType();

                        ExecuteSchemaValidate(entityType);

                        var primaryKeyPropertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();
                        var isIdentity = primaryKeyPropertyInfo.IsColumnIdentity();

                        // Checks if the entity exist in the database
                        var existInDb = _dbHelper.ExecuteObjectExist(command, entitySet.Entity);

                        // Prepare the sql statement
                        PrepareEntitySetQuery(
                            entitySet,
                            existInDb,
                            out string sql,
                            out Dictionary<string, object> parameters);

                        // Executes the sql statement
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.AddParameters(parameters);

                        if (entitySet.State == EntityState.Added && isIdentity)
                        {
#if NETFULL
                            if (_dbHelper.ProviderType == DataAccessProviderType.SqlServerCompact)
                            {
                                _dbHelper.ExecuteNonQuery(command);

                                sql = "SELECT @@IDENTITY";
                                parameters.Clear();

                                command.CommandText = sql;
                                command.Parameters.Clear();
                            }
#endif
                            var newKey = _dbHelper.ExecuteScalar<object>(command);
                            var convertedKeyValue = Convert.ChangeType(newKey, primaryKeyPropertyInfo.PropertyType);

                            primaryKeyPropertyInfo.SetValue(entitySet.Entity, convertedKeyValue, null);

                            rows++;
                        }
                        else
                        {
                            rows += _dbHelper.ExecuteNonQuery(command);
                        }
                    }
                }
                finally
                {
                    if (canCloseConnection && ownsConnection)
                        connection.Dispose();

                    // Clears the collection
                    while (_items.Count > 0)
                    {
                        _items.TryTake(out _);
                    }
                }

                return rows;
            }
        }

        /// <summary>
        /// Finds an entity with the given primary key values in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found in the repository.</returns>
        public IQueryResult<TEntity> Find<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            PrimaryKeyConventionHelper.ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(keyValues);

            var options = new QueryOptions<TEntity>().Include(PrimaryKeyConventionHelper.GetByPrimaryKeySpecification<TEntity>(keyValues));

            if (fetchStrategy != null)
                options.Include(fetchStrategy);

            var selectorFunc = IdentityFunction<TEntity>.Instance;

            QueryBuilder.CreateSelectStatement<TEntity>(
                options,
                fetchStrategy != null,
                out var sql,
                out var parameters,
                out var navigationProperties,
                out var getPropertyFromColumnAliasCallback);

            var mapper = new Mapper<TEntity>(navigationProperties, getPropertyFromColumnAliasCallback);

            ExecuteSchemaValidate(typeof(TEntity));

            return _dbHelper.ExecuteObject<TEntity>(sql, parameters, reader => mapper.Map<TEntity>(reader, selectorFunc));
        }

        /// <summary>
        /// Finds the first projected entity result in the repository that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The projected entity result that satisfied the criteria specified by the <paramref name="selector" /> in the repository.</returns>
        public IQueryResult<TResult> Find<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var selectorFunc = selector.Compile();

            QueryBuilder.CreateSelectStatement<TEntity>(
                options,
                out var sql,
                out var parameters,
                out var navigationProperties,
                out var getPropertyFromColumnAliasCallback);

            var mapper = new Mapper<TEntity>(navigationProperties, getPropertyFromColumnAliasCallback);

            ExecuteSchemaValidate(typeof(TEntity));

            return _dbHelper.ExecuteObject<TResult>(sql, parameters, reader => mapper.Map<TResult>(reader, selectorFunc));
        }

        /// <summary>
        /// Finds the collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="selector">A function to project each entity into a new form.</param>
        /// <returns>The collection of projected entity results in the repository that satisfied the criteria specified by the <paramref name="options" />.</returns>
        public IPagedQueryResult<IEnumerable<TResult>> FindAll<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var selectorFunc = selector.Compile();

            QueryBuilder.CreateSelectStatement<TEntity>(
                options,
                out var sql,
                out var parameters,
                out var navigationProperties,
                out var getPropertyFromColumnAliasCallback);

            var mapper = new Mapper<TEntity>(navigationProperties, getPropertyFromColumnAliasCallback);

            ExecuteSchemaValidate(typeof(TEntity));

            return _dbHelper.ExecuteList<TResult>(sql, parameters, reader => mapper.Map<TResult>(reader, selectorFunc));
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public IQueryResult<int> Count<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            QueryBuilder.CreateSelectStatement<TEntity>(options, "COUNT(*)", out var sql, out var parameters);

            ExecuteSchemaValidate(typeof(TEntity));

            var result = _dbHelper.ExecuteScalar<int>(sql, parameters);

            return new QueryResult<int>(result);
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public IQueryResult<bool> Exists<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            QueryBuilder.CreateSelectStatement<TEntity>(options, out var sql, out var parameters);

            ExecuteSchemaValidate(typeof(TEntity));

            using (var reader = _dbHelper.ExecuteReader(sql, parameters))
            {
                var hasRows = false;

                while (reader.Read())
                {
                    hasRows = true;

                    break;
                }

                return new QueryResult<bool>(hasRows);
            }
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
        public IPagedQueryResult<Dictionary<TDictionaryKey, TElement>> ToDictionary<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            QueryBuilder.CreateSelectStatement<TEntity>(
                options,
                out var sql,
                out var parameters,
                out var navigationProperties,
                out var getPropertyFromColumnAliasCallback);

            var mapper = new Mapper<TEntity>(navigationProperties, getPropertyFromColumnAliasCallback);

            ExecuteSchemaValidate(typeof(TEntity));

            using (var reader = _dbHelper.ExecuteReader(sql, parameters))
            {
                var dict = new Dictionary<TDictionaryKey, TElement>();
                var foundCrossJoinCountColumn = false;
                var total = 0;

                QueryBuilder.ExtractCrossJoinColumnName(sql, out var crossJoinColumnName);

                while (reader.Read())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);

                        if (!string.IsNullOrEmpty(crossJoinColumnName) && name.Equals(crossJoinColumnName))
                        {
                            total = (int)reader[name];
                            foundCrossJoinCountColumn = true;
                            break;
                        }
                    }

                    dict.Add(mapper.Map<TDictionaryKey>(reader, keySelectFunc), mapper.Map<TElement>(reader, elementSelectorFunc));
                }

                if (!foundCrossJoinCountColumn)
                    total = dict.Count;

                return new PagedQueryResult<Dictionary<TDictionaryKey, TElement>>(dict, total);
            }
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
        public IPagedQueryResult<IEnumerable<TResult>> GroupBy<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            var keySelectFunc = keySelector.Compile();
            var resultSelectorFunc = resultSelector.Compile();

            var queryResult = FindAll<TEntity, TEntity>(options, IdentityExpression<TEntity>.Instance);

            var result = queryResult.Result
                .GroupBy(keySelectFunc, resultSelectorFunc)
                .ToList();

            return new PagedQueryResult<IEnumerable<TResult>>(result, queryResult.Total);
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
        public async Task<IQueryResult<IEnumerable<TEntity>>> ExecuteSqlQueryAsync<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            if (projector == null)
                throw new ArgumentNullException(nameof(projector));

            using (var reader = await _dbHelper.ExecuteReaderAsync(sql, cmdType, parameters, cancellationToken))
            {
                var list = new List<TEntity>();

                while (await reader.ReadAsync(cancellationToken))
                {
                    list.Add(projector(reader));
                }

                return new QueryResult<IEnumerable<TEntity>>(list);
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
        public async Task<IQueryResult<int>> ExecuteSqlCommandAsync(string sql, CommandType cmdType, Dictionary<string, object> parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            if (sql == null)
                throw new ArgumentNullException(nameof(sql));

            return new QueryResult<int>(await _dbHelper.ExecuteNonQueryAsync(sql, cmdType, parameters, cancellationToken));
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of state entries written to the database.</returns>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (var command = _dbHelper.CreateCommand())
            {
                var rows = 0;
                var connection = command.Connection;
                var canCloseConnection = false;
                var ownsConnection = _dbHelper.OwnsConnection;

                if (connection.State == ConnectionState.Closed)
                {
                    await connection.OpenAsync(cancellationToken);
                    canCloseConnection = true;
                }

                try
                {
                    while (_items.TryTake(out var entitySet))
                    {
                        var entityType = entitySet.Entity.GetType();

                        await ExecuteSchemaValidateAsync(entityType, cancellationToken);

                        var primaryKeyPropertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();
                        var isIdentity = primaryKeyPropertyInfo.IsColumnIdentity();

                        // Checks if the entity exist in the database
                        var existInDb = await _dbHelper.ExecuteObjectExistAsync(command, entitySet.Entity, cancellationToken);

                        // Prepare the sql statement
                        PrepareEntitySetQuery(
                            entitySet,
                            existInDb,
                            out string sql,
                            out Dictionary<string, object> parameters);

                        // Executes the sql statement
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.AddParameters(parameters);

                        if (entitySet.State == EntityState.Added && isIdentity)
                        {
#if NETFULL
                            if (_dbHelper.ProviderType == DataAccessProviderType.SqlServerCompact)
                            {
                                await _dbHelper.ExecuteNonQueryAsync(command, cancellationToken);

                                sql = "SELECT @@IDENTITY";
                                parameters.Clear();

                                command.CommandText = sql;
                                command.Parameters.Clear();
                            }
#endif
                            var newKey = await _dbHelper.ExecuteScalarAsync<object>(command, cancellationToken);
                            var convertedKeyValue = Convert.ChangeType(newKey, primaryKeyPropertyInfo.PropertyType);

                            primaryKeyPropertyInfo.SetValue(entitySet.Entity, convertedKeyValue, null);

                            rows++;
                        }
                        else
                        {
                            rows += await _dbHelper.ExecuteNonQueryAsync(command, cancellationToken);
                        }
                    }
                }
                finally
                {
                    if (canCloseConnection && ownsConnection)
                        connection.Dispose();

                    // Clears the collection
                    while (_items.Count > 0)
                    {
                        _items.TryTake(out _);
                    }
                }

                return rows;
            }
        }

        /// <summary>
        /// Asynchronously finds an entity with the given primary key values in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the entity found in the repository.</returns>
        public async Task<IQueryResult<TEntity>> FindAsync<TEntity>(CancellationToken cancellationToken, IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            PrimaryKeyConventionHelper.ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(keyValues);

            var options = new QueryOptions<TEntity>().Include(PrimaryKeyConventionHelper.GetByPrimaryKeySpecification<TEntity>(keyValues));

            if (fetchStrategy != null)
                options.Include(fetchStrategy);

            var selectorFunc = IdentityFunction<TEntity>.Instance;

            QueryBuilder.CreateSelectStatement<TEntity>(
                options,
                fetchStrategy != null,
                out var sql,
                out var parameters,
                out var navigationProperties,
                out var getPropertyFromColumnAliasCallback);

            var mapper = new Mapper<TEntity>(navigationProperties, getPropertyFromColumnAliasCallback);

            await ExecuteSchemaValidateAsync(typeof(TEntity), cancellationToken);

            return await _dbHelper.ExecuteObjectAsync<TEntity>(sql, parameters, reader => mapper.Map<TEntity>(reader, selectorFunc), cancellationToken);
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
        public async Task<IQueryResult<TResult>> FindAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var selectorFunc = selector.Compile();

            QueryBuilder.CreateSelectStatement<TEntity>(
                options,
                out var sql,
                out var parameters,
                out var navigationProperties,
                out var getPropertyFromColumnAliasCallback);

            var mapper = new Mapper<TEntity>(navigationProperties, getPropertyFromColumnAliasCallback);

            await ExecuteSchemaValidateAsync(typeof(TEntity), cancellationToken);

            return await _dbHelper.ExecuteObjectAsync<TResult>(sql, parameters, reader => mapper.Map<TResult>(reader, selectorFunc), cancellationToken);
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
        public async Task<IPagedQueryResult<IEnumerable<TResult>>> FindAllAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var selectorFunc = selector.Compile();

            QueryBuilder.CreateSelectStatement<TEntity>(
                options,
                out var sql,
                out var parameters,
                out var navigationProperties,
                out var getPropertyFromColumnAliasCallback);

            var mapper = new Mapper<TEntity>(navigationProperties, getPropertyFromColumnAliasCallback);

            await ExecuteSchemaValidateAsync(typeof(TEntity), cancellationToken);

            return await _dbHelper.ExecuteListAsync<TResult>(sql, parameters, reader => mapper.Map<TResult>(reader, selectorFunc), cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public async Task<IQueryResult<int>> CountAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            QueryBuilder.CreateSelectStatement<TEntity>(options, "COUNT(*)", out var sql, out var parameters);

            ExecuteSchemaValidate(typeof(TEntity));

            var result = await _dbHelper.ExecuteScalarAsync<int>(sql, parameters, cancellationToken);

            return new QueryResult<int>(result);
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public async Task<IQueryResult<bool>> ExistsAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            QueryBuilder.CreateSelectStatement<TEntity>(options, out var sql, out var parameters);

            await ExecuteSchemaValidateAsync(typeof(TEntity), cancellationToken);

            using (var reader = await _dbHelper.ExecuteReaderAsync(sql, parameters, cancellationToken))
            {
                var hasRows = false;

                while (reader.Read())
                {
                    hasRows = true;

                    break;
                }

                return new QueryResult<bool>(hasRows);
            }
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
        public async Task<IPagedQueryResult<Dictionary<TDictionaryKey, TElement>>> ToDictionaryAsync<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            QueryBuilder.CreateSelectStatement<TEntity>(
                options,
                out var sql,
                out var parameters,
                out var navigationProperties,
                out var getPropertyFromColumnAliasCallback);

            var mapper = new Mapper<TEntity>(navigationProperties, getPropertyFromColumnAliasCallback);

            await ExecuteSchemaValidateAsync(typeof(TEntity), cancellationToken);

            using (var reader = await _dbHelper.ExecuteReaderAsync(sql, parameters, cancellationToken))
            {
                var dict = new Dictionary<TDictionaryKey, TElement>();
                var foundCrossJoinCountColumn = false;
                var total = 0;

                QueryBuilder.ExtractCrossJoinColumnName(sql, out var crossJoinColumnName);

                while (reader.Read())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);

                        if (!string.IsNullOrEmpty(crossJoinColumnName) && name.Equals(crossJoinColumnName))
                        {
                            total = (int)reader[name];
                            foundCrossJoinCountColumn = true;
                            break;
                        }
                    }

                    dict.Add(mapper.Map<TDictionaryKey>(reader, keySelectFunc), mapper.Map<TElement>(reader, elementSelectorFunc));
                }

                if (!foundCrossJoinCountColumn)
                    total = dict.Count;

                return new PagedQueryResult<Dictionary<TDictionaryKey, TElement>>(dict, total);
            }
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
        public async Task<IPagedQueryResult<IEnumerable<TResult>>> GroupByAsync<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            var keySelectFunc = keySelector.Compile();
            var resultSelectorFunc = resultSelector.Compile();

            var queryResult = await FindAllAsync<TEntity, TEntity>(options, IdentityExpression<TEntity>.Instance, cancellationToken);

            var result = queryResult.Result
                .GroupBy(keySelectFunc, resultSelectorFunc)
                .ToList();

            return new PagedQueryResult<IEnumerable<TResult>>(result, queryResult.Total);
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _dbHelper.Dispose();

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}