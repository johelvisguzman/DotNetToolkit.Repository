namespace DotNetToolkit.Repository.AdoNet
{
    using FetchStrategies;
    using Logging;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Represents a repository for ado.net.
    /// </summary>
    public abstract class AdoNetRepositoryBase<TEntity, TKey> : RepositoryBase<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private Internal.ThreadSafeCollection<EntitySet<TEntity>> _context = new Internal.ThreadSafeCollection<EntitySet<TEntity>>();
        private readonly Dictionary<string, PropertyInfo> _runtimePropertyDictionary;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the factory.
        /// </summary>
        protected DbProviderFactory Factory { get; }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        protected string ConnectionString { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="logger">The logger.</param>
        protected AdoNetRepositoryBase(string providerName, string connectionString, ILogger logger = null) : base(logger)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmptyString, nameof(connectionString));

#if NETFULL
            Factory = System.Data.Common.DbProviderFactories.GetFactory(providerName);
#else
            Factory = Internal.DbProviderFactoryHelper.GetDbProviderFactory(providerName);
#endif

            ConnectionString = connectionString;

            _runtimePropertyDictionary = typeof(TEntity)
                .GetRuntimeProperties()
                .Where(x => x.PropertyType.Namespace == "System")
                .ToDictionary(x => x.Name, x => x);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns>The connection.</returns>
        protected DbConnection CreateConnection()
        {
            var connection = Factory.CreateConnection();

            connection.ConnectionString = ConnectionString;

            return connection;
        }

        /// <summary>
        /// Creates a command.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
		/// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
		/// <returns>The new command.</returns>
        protected DbCommand CreateCommand(DbConnection connection, string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            var command = Factory.CreateCommand();

            command.CommandText = cmdText;
            command.Connection = connection;
            command.CommandType = cmdType;

            if (parameters != null)
            {
                command.Parameters.AddRange(parameters);
            }

            return command;
        }

        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The new parameter.</returns>
        protected DbParameter CreateParameter(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmptyString, nameof(name));

            var parameter = Factory.CreateParameter();

            parameter.Value = value ?? DBNull.Value;
            parameter.ParameterName = name;

            return parameter;
        }

        /// <summary>
        /// Executes a SQL statement against a connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
		/// <param name="parameters">The command parameters</param>
        /// <returns>The number of rows affected.</returns>
        protected int ExecuteNonQuery(DbConnection connection, string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            using (var command = CreateCommand(connection, cmdText, cmdType, parameters))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                return command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes a SQL statement against a connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>The number of rows affected.</returns>
        protected int ExecuteNonQuery(DbConnection connection, string cmdText, params DbParameter[] parameters)
        {
            return ExecuteNonQuery(connection, cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Executes a SQL statement against a connection.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>The number of rows affected.</returns>
        protected int ExecuteNonQuery(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            using (var connection = CreateConnection())
            {
                return ExecuteNonQuery(connection, cmdText, cmdType, parameters);
            }
        }

        /// <summary>
        /// Executes a SQL statement against a connection.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>The number of rows affected.</returns>
        protected int ExecuteNonQuery(string cmdText, params DbParameter[] parameters)
        {
            return ExecuteNonQuery(cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Sends the <see cref="System.Data.SqlClient.SqlCommand.CommandText" /> to the <see cref="System.Data.SqlClient.SqlCommand.Connection" /> and builds a <see cref="System.Data.SqlClient.SqlDataReader "/>.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>A <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        protected DbDataReader ExecuteReader(DbConnection connection, string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var command = CreateCommand(connection, cmdText, cmdType, parameters);

            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }

        /// <summary>
        /// Sends the <see cref="System.Data.SqlClient.SqlCommand.CommandText" /> to the <see cref="System.Data.SqlClient.SqlCommand.Connection" /> and builds a <see cref="System.Data.SqlClient.SqlDataReader "/>.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>A <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        protected DbDataReader ExecuteReader(DbConnection connection, string cmdText, params DbParameter[] parameters)
        {
            return ExecuteReader(connection, cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Sends the <see cref="System.Data.SqlClient.SqlCommand.CommandText" /> to the <see cref="System.Data.SqlClient.SqlCommand.Connection" /> and builds a <see cref="System.Data.SqlClient.SqlDataReader "/>.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
		/// <param name="parameters">The command parameters</param>
        /// <returns>A <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        protected DbDataReader ExecuteReader(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            return ExecuteReader(CreateConnection(), cmdText, cmdType, parameters);
        }

        /// <summary>
        /// Sends the <see cref="System.Data.SqlClient.SqlCommand.CommandText" /> to the <see cref="System.Data.SqlClient.SqlCommand.Connection" /> and builds a <see cref="System.Data.SqlClient.SqlDataReader "/>.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>A <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        protected DbDataReader ExecuteReader(string cmdText, params DbParameter[] parameters)
        {
            return ExecuteReader(cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>the first column of the first row in the result set returned by the query.</returns>
        protected T ExecuteScalar<T>(DbConnection connection, string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            using (var command = CreateCommand(connection, cmdText, cmdType, parameters))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                var result = command.ExecuteScalar();

                if (result == null || result is DBNull)
                    return default(T);

                if (result is T)
                    return (T)result;

                return (T)Convert.ChangeType(result, typeof(T));
            }
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>the first column of the first row in the result set returned by the query.</returns>
        protected T ExecuteScalar<T>(DbConnection connection, string cmdText, params DbParameter[] parameters)
        {
            return ExecuteScalar<T>(connection, cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>the first column of the first row in the result set returned by the query.</returns>
        protected T ExecuteScalar<T>(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();

                return ExecuteScalar<T>(connection, cmdText, cmdType, parameters);
            }
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>the first column of the first row in the result set returned by the query.</returns>
        protected T ExecuteScalar<T>(string cmdText, params DbParameter[] parameters)
        {
            return ExecuteScalar<T>(cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public T ExecuteObject<T>(string cmdText, CommandType cmdType, DbParameter[] parameters, Func<DbDataReader, T> projector)
        {
            using (var reader = ExecuteReader(cmdText, cmdType, parameters))
            {
                return reader.Read() ? projector(reader) : default(T);
            }
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public T ExecuteObject<T>(string cmdText, DbParameter[] parameters, Func<DbDataReader, T> projector)
        {
            return ExecuteObject<T>(cmdText, CommandType.Text, parameters, projector);
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
		/// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public T ExecuteObject<T>(string cmdText, CommandType cmdType, Func<DbDataReader, T> projector)
        {
            using (var reader = ExecuteReader(cmdText, cmdType))
            {
                return reader.Read() ? projector(reader) : default(T);
            }
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public T ExecuteObject<T>(string cmdText, Func<DbDataReader, T> projector)
        {
            return ExecuteObject<T>(cmdText, CommandType.Text, projector);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public IEnumerable<T> ExecuteList<T>(string cmdText, CommandType cmdType, DbParameter[] parameters, Func<DbDataReader, T> projector)
        {
            using (var reader = ExecuteReader(cmdText, cmdType, parameters))
            {
                while (reader.Read())
                {
                    yield return projector(reader);
                }
            }
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public IEnumerable<T> ExecuteList<T>(string cmdText, DbParameter[] parameters, Func<DbDataReader, T> projector)
        {
            return ExecuteList<T>(cmdText, CommandType.Text, parameters, projector);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public IEnumerable<T> ExecuteList<T>(string cmdText, CommandType cmdType, Func<DbDataReader, T> projector)
        {
            return ExecuteList<T>(cmdText, cmdType, null, projector);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public IEnumerable<T> ExecuteList<T>(string cmdText, Func<DbDataReader, T> projector)
        {
            return ExecuteList<T>(cmdText, CommandType.Text, projector);
        }

        #endregion

        #region Private Methods

        private TEntity AutoMap(DbDataReader r)
        {
            var entity = Activator.CreateInstance<TEntity>();

            for (var i = 0; i < r.FieldCount; i++)
            {
                var name = r.GetName(i);

                if (_runtimePropertyDictionary.ContainsKey(name) && !r.IsDBNull(r.GetOrdinal(name)))
                {
                    _runtimePropertyDictionary[name].SetValue(entity, r[name]);
                }
            }

            return entity;
        }

        #endregion

        #region Overrides of RepositoryBase<TEntity,TKey>

        /// <summary>
        /// A protected overridable method for adding the specified <paramref name="entity" /> into the repository.
        /// </summary>
        protected override void AddItem(TEntity entity)
        {
            _context.Add(new EntitySet<TEntity>(entity, EntityState.Added));
        }

        /// <summary>
        /// A protected overridable method for deleting the specified <paramref name="entity" /> from the repository.
        /// </summary>
        protected override void DeleteItem(TEntity entity)
        {
            _context.Add(new EntitySet<TEntity>(entity, EntityState.Removed));
        }

        /// <summary>
        /// A protected overridable method for updating the specified <paramref name="entity" /> in the repository.
        /// </summary>
        protected override void UpdateItem(TEntity entity)
        {
            _context.Add(new EntitySet<TEntity>(entity, EntityState.Modified));
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            using (var connection = CreateConnection())
            {
                connection.Open();

                var tableName = $"{typeof(TEntity).Name}";
                var primaryKey = GetPrimaryKeyPropertyInfo();
                var propertyDictionary = _runtimePropertyDictionary.ToList();

                DataRow autoIncrementDataRow = null;

                using (var reader = ExecuteReader(connection, $"SELECT * FROM {tableName}"))
                {
                    var schema = reader.GetSchemaTable();

                    for (int i = 0; i < schema.Rows.Count; i++)
                    {
                        var dc = schema.Rows[i];
                        if ((bool)dc["IsAutoIncrement"])
                        {
                            autoIncrementDataRow = dc;
                            break;
                        }
                    }

                    if (autoIncrementDataRow != null)
                    {
                        var p = propertyDictionary.First(x => x.Key == (string)autoIncrementDataRow["ColumnName"]);
                        propertyDictionary.Remove(p);
                    }
                }

                foreach (var entitySet in _context)
                {
                    var parameters = new List<DbParameter>();
                    var query = string.Empty;

                    switch (entitySet.State)
                    {
                        case EntityState.Added:
                            {
                                var propertyNames = string.Join(",", propertyDictionary.Select(x => x.Key));
                                var propertyNamesAndValues = string.Join(",\n", propertyDictionary.Select(x => $"@{x.Key}"));

                                query = $@"INSERT INTO {tableName} ({propertyNames})
                                           VALUES ({propertyNamesAndValues})";

                                parameters.AddRange(propertyDictionary.Select(pi => CreateParameter($"@{pi.Key}", pi.Value.GetValue(entitySet.Entity, null))));

                                break;
                            }
                        case EntityState.Removed:
                            {
                                query = $@"DELETE FROM {tableName}
                                           WHERE {primaryKey.Name} = @{primaryKey.Name}";

                                parameters.Add(CreateParameter($"@{primaryKey.Name}", primaryKey.GetValue(entitySet.Entity, null)));

                                break;
                            }
                        case EntityState.Modified:
                            {
                                var propertyNamesAndValues = string.Join(",\n", propertyDictionary.Select(x => x.Key + " = " + $"@{x.Key}"));

                                query = $@"UPDATE {tableName}
                                           SET {propertyNamesAndValues}
                                           WHERE {primaryKey.Name} = @{primaryKey.Name}";

                                parameters.Add(CreateParameter($"@{primaryKey.Name}", primaryKey.GetValue(entitySet.Entity, null)));
                                parameters.AddRange(propertyDictionary.Select(pi => CreateParameter($"@{pi.Key}", pi.Value.GetValue(entitySet.Entity, null))));

                                break;
                            }
                    }

                    ExecuteNonQuery(connection, query, parameters.ToArray());

                    if (entitySet.State == EntityState.Added && autoIncrementDataRow != null)
                    {
                        var key = ExecuteScalar<TKey>(connection, "SELECT @@IDENTITY");

                        SetPrimaryKey(entitySet.Entity, key);
                    }
                }

                _context.Clear();
            }
        }

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected override IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null)
        {
            var tableName = $"{typeof(TEntity).Name}";
            var query = $"SELECT * FROM {tableName}";

            return ExecuteList<TEntity>(query, AutoMap).AsQueryable();
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override TEntity GetEntity(TKey key, IFetchStrategy<TEntity> fetchStrategy)
        {
            var tableName = $"{typeof(TEntity).Name}";
            var primaryKeyName = GetPrimaryKeyPropertyInfo().Name;
            var query = $@"SELECT * FROM {tableName}
                           WHERE {primaryKeyName} = @{primaryKeyName}";
            var parameters = new[] { CreateParameter($"@{primaryKeyName}", key) };

            return ExecuteObject<TEntity>(query, parameters, AutoMap);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            _context.Clear();
            _context = null;
        }

        #endregion

        #region Nested type: EntitySet<TEntity, TKey>

        /// <summary>
        /// Represents an internal entity set in the in-memory store, which holds the entity and it's state representing the operation that was performed at the time.
        /// </summary>
        private class EntitySet<TEntity> where TEntity : class
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="EntitySet{TEntity}"/> class.
            /// </summary>
            /// <param name="entity">The entity.</param>
            /// <param name="key">The entity primary key value.</param>
            /// <param name="state">The state.</param>
            public EntitySet(TEntity entity, EntityState state)
            {
                Entity = entity;
                State = state;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the entity.
            /// </summary>
            public TEntity Entity { get; }

            /// <summary>
            /// Gets the state.
            /// </summary>
            public EntityState State { get; }

            #endregion
        }

        #endregion

        #region Nested type: EntityState

        /// <summary>
        /// Represents an internal state for an entity in the in-memory store.
        /// </summary>
        private enum EntityState
        {
            Added,
            Removed,
            Modified
        }

        #endregion
    }
}
