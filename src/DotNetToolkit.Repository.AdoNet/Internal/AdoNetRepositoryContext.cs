namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Configuration.Logging;
    using Extensions;
    using Helpers;
    using Queries;
    using Queries.Strategies;
    using Schema;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
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

        private DbTransaction _transaction;
        private readonly DbProviderFactory _factory;
        private readonly string _connectionString;
        private DbConnection _connection;
        private readonly bool _ownsConnection;

        private readonly BlockingCollection<EntitySet> _items = new BlockingCollection<EntitySet>();
        private readonly ConcurrentDictionary<Type, bool> _schemaValidationTypeMapping = new ConcurrentDictionary<Type, bool>();
        private readonly SchemaTableConfigurationHelper _schemaConfigHelper;

        private readonly QueryBuilder _queryBuilder = new QueryBuilder();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the repository context logger.
        /// </summary>
        public ILogger Logger { get; private set; } = NullLogger.Instance;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContext" /> class.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        public AdoNetRepositoryContext(string nameOrConnectionString)
        {
            if (nameOrConnectionString == null)
                throw new ArgumentNullException(nameof(nameOrConnectionString));

            var css = ConfigurationManager.ConnectionStrings[nameOrConnectionString];
            if (css == null)
            {
                for (var i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
                {
                    css = ConfigurationManager.ConnectionStrings[i];

                    if (css.ConnectionString.Equals(nameOrConnectionString))
                        break;
                }
            }

            if (css == null)
                throw new ArgumentException("The connection string does not exist in your configuration file.");

            _factory = DbProviderFactories.GetFactory(css.ProviderName);
            _connectionString = css.ConnectionString;
            _ownsConnection = true;
            _schemaConfigHelper = new SchemaTableConfigurationHelper(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContext" /> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetRepositoryContext(string providerName, string connectionString)
        {
            if (providerName == null)
                throw new ArgumentNullException(nameof(providerName));

            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            _factory = DbProviderFactories.GetFactory(providerName);
            _connectionString = connectionString;
            _ownsConnection = true;
            _schemaConfigHelper = new SchemaTableConfigurationHelper(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContext" /> class.
        /// </summary>
        /// <param name="existingConnection">The existing connection.</param>
        public AdoNetRepositoryContext(DbConnection existingConnection)
        {
            if (existingConnection == null)
                throw new ArgumentNullException(nameof(existingConnection));

            if (existingConnection.State == ConnectionState.Closed)
                existingConnection.Open();

            _connection = existingConnection;
            _ownsConnection = false;
            _schemaConfigHelper = new SchemaTableConfigurationHelper(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <returns>The new command.</returns>
        public virtual DbCommand CreateCommand()
        {
            var command = _ownsConnection
                ? _factory.CreateCommand()
                : _connection.CreateCommand();

            DbConnection connection;

            if (_transaction != null)
            {
                connection = _transaction.Connection;

                try
                {
                    command.Transaction = _transaction;
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            else
            {
                connection = CreateConnection();
            }

            command.Connection = connection;

            return command;
        }

        /// <summary>
        /// Executes a SQL statement against a connection.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>The number of rows affected.</returns>
        public virtual int ExecuteNonQuery(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            using (var command = CreateCommand(cmdText, cmdType, parameters))
            {
                var connection = command.Connection;
                var ownsConnection = _ownsConnection && command.Transaction == null;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                if (Logger.IsEnabled(LogLevel.Debug))
                    Logger.Debug(FormatExecutingDebugQuery("ExecuteNonQuery", parameters, cmdText));

                var result = command.ExecuteNonQuery();

                if (ownsConnection)
                    connection.Dispose();

                return result;
            }
        }

        /// <summary>
        /// Executes a SQL statement against a connection.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteNonQuery(string cmdText, Dictionary<string, object> parameters = null)
        {
            return ExecuteNonQuery(cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DbDataReader" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        public virtual DbDataReader ExecuteReader(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            var command = CreateCommand(cmdText, cmdType, parameters);
            var connection = command.Connection;
            var ownsConnection = _ownsConnection && command.Transaction == null;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            if (Logger.IsEnabled(LogLevel.Debug))
                Logger.Debug(FormatExecutingDebugQuery("ExecuteReader", parameters, cmdText));

            var reader = command.ExecuteReader(ownsConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default);

            return reader;
        }

        /// <summary>
        /// Sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DbDataReader" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        public DbDataReader ExecuteReader(string cmdText, Dictionary<string, object> parameters = null)
        {
            return ExecuteReader(cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>The first column of the first row in the result set returned by the query.</returns>
        public virtual T ExecuteScalar<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            using (var command = CreateCommand(cmdText, cmdType, parameters))
            {
                var connection = command.Connection;
                var ownsConnection = _ownsConnection && command.Transaction == null;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                if (Logger.IsEnabled(LogLevel.Debug))
                    Logger.Debug(FormatExecutingDebugQuery("ExecuteScalar", parameters, cmdText));

                var result = ConvertValue<T>(command.ExecuteScalar());

                if (ownsConnection)
                    connection.Dispose();

                return result;
            }
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>The first column of the first row in the result set returned by the query.</returns>
        public T ExecuteScalar<T>(string cmdText, Dictionary<string, object> parameters = null)
        {
            return ExecuteScalar<T>(cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>An entity which has been projected into a new form.</returns>
        public virtual QueryResult<T> ExecuteObject<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
        {
            using (var reader = ExecuteReader(cmdText, cmdType, parameters))
            {
                var list = new List<T>();

                while (reader.Read())
                {
                    list.Add(projector(reader));
                }

                var result = list.FirstOrDefault();

                return new QueryResult<T>(result);
            }
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>An entity which has been projected into a new form.</returns>
        public QueryResult<T> ExecuteObject<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters) where T : class
        {
            return ExecuteObject<T>(cmdText, cmdType, parameters, Mapper<T>.Map);
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>An entity which has been projected into a new form.</returns>
        public QueryResult<T> ExecuteObject<T>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
        {
            return ExecuteObject<T>(cmdText, CommandType.Text, parameters, projector);
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>An entity which has been projected into a new form.</returns>
        public QueryResult<T> ExecuteObject<T>(string cmdText, Dictionary<string, object> parameters) where T : class
        {
            return ExecuteObject<T>(cmdText, parameters, Mapper<T>.Map);
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>An entity which has been projected into a new form.</returns>
        public QueryResult<T> ExecuteObject<T>(string cmdText, CommandType cmdType, Func<DbDataReader, T> projector)
        {
            return ExecuteObject<T>(cmdText, cmdType, null, projector);
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <returns>An entity which has been projected into a new form.</returns>
        public QueryResult<T> ExecuteObject<T>(string cmdText, CommandType cmdType) where T : class
        {
            return ExecuteObject<T>(cmdText, cmdType, Mapper<T>.Map);
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>An entity which has been projected into a new form.</returns>
        public QueryResult<T> ExecuteObject<T>(string cmdText, Func<DbDataReader, T> projector)
        {
            return ExecuteObject<T>(cmdText, CommandType.Text, projector);
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <returns>An entity which has been projected into a new form.</returns>
        public QueryResult<T> ExecuteObject<T>(string cmdText) where T : class
        {
            return ExecuteObject<T>(cmdText, Mapper<T>.Map);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public virtual QueryResult<IEnumerable<T>> ExecuteList<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
        {
            using (var reader = ExecuteReader(cmdText, cmdType, parameters))
            {
                var list = new List<T>();
                var foundCrossJoinCountColumn = false;
                var total = 0;

                while (reader.Read())
                {
                    // TODO: NEEDS TO FIGURE OUT ANOTHER WAY TO DO THIS
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);

                        if (name.Equals(QueryBuilder.CrossJoinCountColumnName))
                        {
                            total = (int)reader[name];
                            foundCrossJoinCountColumn = true;
                            break;
                        }
                    }

                    list.Add(projector(reader));
                }

                if (!foundCrossJoinCountColumn)
                    total = list.Count;

                return new QueryResult<IEnumerable<T>>(list, total);
            }
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public QueryResult<IEnumerable<T>> ExecuteList<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters) where T : class
        {
            return ExecuteList<T>(cmdText, cmdType, parameters, Mapper<T>.Map);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public QueryResult<IEnumerable<T>> ExecuteList<T>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
        {
            return ExecuteList<T>(cmdText, CommandType.Text, parameters, projector);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public QueryResult<IEnumerable<T>> ExecuteList<T>(string cmdText, Dictionary<string, object> parameters) where T : class
        {
            return ExecuteList<T>(cmdText, parameters, Mapper<T>.Map);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public QueryResult<IEnumerable<T>> ExecuteList<T>(string cmdText, CommandType cmdType, Func<DbDataReader, T> projector)
        {
            return ExecuteList<T>(cmdText, cmdType, null, projector);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public QueryResult<IEnumerable<T>> ExecuteList<T>(string cmdText, CommandType cmdType) where T : class
        {
            return ExecuteList<T>(cmdText, cmdType, Mapper<T>.Map);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public QueryResult<IEnumerable<T>> ExecuteList<T>(string cmdText, Func<DbDataReader, T> projector)
        {
            return ExecuteList<T>(cmdText, CommandType.Text, projector);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public QueryResult<IEnumerable<T>> ExecuteList<T>(string cmdText) where T : class
        {
            return ExecuteList<T>(cmdText, Mapper<T>.Map);
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public virtual QueryResult<Dictionary<TDictionaryKey, TElement>> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector)
        {
            using (var reader = ExecuteReader(cmdText, cmdType, parameters))
            {
                var dict = new Dictionary<TDictionaryKey, TElement>();

                while (reader.Read())
                {
                    dict.Add(keyProjector(reader.GetValue(0)), elementProjector(reader.GetValue(1)));
                }

                return new QueryResult<Dictionary<TDictionaryKey, TElement>>(dict);
            }
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public QueryResult<Dictionary<TDictionaryKey, TElement>> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector)
        {
            return ExecuteDictionary<TDictionaryKey, TElement>(cmdText, cmdType, null, keyProjector, elementProjector);
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public QueryResult<Dictionary<TDictionaryKey, TElement>> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, Dictionary<string, object> parameters, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector)
        {
            return ExecuteDictionary<TDictionaryKey, TElement>(cmdText, CommandType.Text, parameters, keyProjector, elementProjector);
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public QueryResult<Dictionary<TDictionaryKey, TElement>> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector)
        {
            return ExecuteDictionary<TDictionaryKey, TElement>(cmdText, null, keyProjector, elementProjector);
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public QueryResult<Dictionary<TDictionaryKey, TElement>> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            return ExecuteDictionary<TDictionaryKey, TElement>(cmdText, cmdType, parameters, ConvertType<TDictionaryKey>(), ConvertType<TElement>());
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public QueryResult<Dictionary<TDictionaryKey, TElement>> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, Dictionary<string, object> parameters = null)
        {
            return ExecuteDictionary<TDictionaryKey, TElement>(cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Asynchronously executes a SQL statement against a connection.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        public virtual async Task<int> ExecuteNonQueryAsync(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var command = CreateCommand(cmdText, cmdType, parameters))
            {
                var connection = command.Connection;
                var ownsConnection = _ownsConnection && command.Transaction == null;

                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync(cancellationToken);

                if (Logger.IsEnabled(LogLevel.Debug))
                    Logger.Debug(FormatExecutingDebugQuery("ExecuteNonQueryAsync", parameters, cmdText));

                var result = await command.ExecuteNonQueryAsync(cancellationToken);

                if (ownsConnection)
                    connection.Dispose();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously executes a SQL statement against a connection.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        public Task<int> ExecuteNonQueryAsync(string cmdText, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteNonQueryAsync(cmdText, CommandType.Text, parameters, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DbDataReader" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        public virtual async Task<DbDataReader> ExecuteReaderAsync(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            var command = CreateCommand(cmdText, cmdType, parameters);
            var connection = command.Connection;
            var ownsConnection = _ownsConnection && command.Transaction == null;

            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync(cancellationToken);

            if (Logger.IsEnabled(LogLevel.Debug))
                Logger.Debug(FormatExecutingDebugQuery("ExecuteReaderAsync", parameters, cmdText));

            var reader = await command.ExecuteReaderAsync(ownsConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default, cancellationToken);

            return reader;
        }

        /// <summary>
        /// Asynchronously sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DbDataReader" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        public Task<DbDataReader> ExecuteReaderAsync(string cmdText, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteReaderAsync(cmdText, CommandType.Text, parameters, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the first column of the first row in the result set returned by the query.</returns>
        public virtual async Task<T> ExecuteScalarAsync<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var command = CreateCommand(cmdText, cmdType, parameters))
            {
                var connection = command.Connection;
                var ownsConnection = _ownsConnection && command.Transaction == null;

                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync(cancellationToken);

                if (Logger.IsEnabled(LogLevel.Debug))
                    Logger.Debug(FormatExecutingDebugQuery("ExecuteScalarAsync", parameters, cmdText));

                var result = ConvertValue<T>(await command.ExecuteScalarAsync(cancellationToken));

                if (ownsConnection)
                    connection.Dispose();

                return result;
            }
        }

        /// <summary>
        /// Asynchronously executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the first column of the first row in the result set returned by the query.</returns>
        public Task<T> ExecuteScalarAsync<T>(string cmdText, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteScalarAsync<T>(cmdText, CommandType.Text, parameters, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing an entity which has been projected into a new form.</returns>
        public virtual async Task<QueryResult<T>> ExecuteObjectAsync<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var reader = await ExecuteReaderAsync(cmdText, cmdType, parameters, cancellationToken))
            {
                var list = new List<T>();

                while (reader.Read())
                {
                    list.Add(projector(reader));
                }

                var result = list.FirstOrDefault();

                return new QueryResult<T>(result);
            }
        }

        /// <summary>
        /// Asynchronously executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing an entity which has been projected into a new form.</returns>
        public Task<QueryResult<T>> ExecuteObjectAsync<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            return ExecuteObjectAsync<T>(cmdText, cmdType, parameters, Mapper<T>.Map, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing an entity which has been projected into a new form.</returns>
        public Task<QueryResult<T>> ExecuteObjectAsync<T>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteObjectAsync<T>(cmdText, CommandType.Text, parameters, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing an entity which has been projected into a new form.</returns>
        public Task<QueryResult<T>> ExecuteObjectAsync<T>(string cmdText, Dictionary<string, object> parameters, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            return ExecuteObjectAsync<T>(cmdText, parameters, Mapper<T>.Map, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing an entity which has been projected into a new form.</returns>
        public Task<QueryResult<T>> ExecuteObjectAsync<T>(string cmdText, CommandType cmdType, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteObjectAsync<T>(cmdText, cmdType, null, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing an entity which has been projected into a new form.</returns>
        public Task<QueryResult<T>> ExecuteObjectAsync<T>(string cmdText, CommandType cmdType, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            return ExecuteObjectAsync<T>(cmdText, cmdType, Mapper<T>.Map, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing an entity which has been projected into a new form.</returns>
        public Task<QueryResult<T>> ExecuteObjectAsync<T>(string cmdText, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteObjectAsync<T>(cmdText, CommandType.Text, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing an entity which has been projected into a new form.</returns>
        public Task<QueryResult<T>> ExecuteObjectAsync<T>(string cmdText, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            return ExecuteObjectAsync<T>(cmdText, Mapper<T>.Map, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        public virtual async Task<QueryResult<IEnumerable<T>>> ExecuteListAsync<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var reader = await ExecuteReaderAsync(cmdText, cmdType, parameters, cancellationToken))
            {
                var list = new List<T>();
                var foundCrossJoinCountColumn = false;
                var total = 0;

                while (reader.Read())
                {
                    // TODO: NEEDS TO FIGURE OUT ANOTHER WAY TO DO THIS
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);

                        if (name.Equals(QueryBuilder.CrossJoinCountColumnName))
                        {
                            total = (int)reader[name];
                            foundCrossJoinCountColumn = true;
                            break;
                        }
                    }

                    list.Add(projector(reader));
                }

                if (!foundCrossJoinCountColumn)
                    total = list.Count;

                return new QueryResult<IEnumerable<T>>(list, total);
            }
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        public Task<QueryResult<IEnumerable<T>>> ExecuteListAsync<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            return ExecuteListAsync<T>(cmdText, cmdType, parameters, Mapper<T>.Map, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        public Task<QueryResult<IEnumerable<T>>> ExecuteListAsync<T>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteListAsync<T>(cmdText, CommandType.Text, parameters, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        public Task<QueryResult<IEnumerable<T>>> ExecuteListAsync<T>(string cmdText, Dictionary<string, object> parameters, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            return ExecuteListAsync<T>(cmdText, parameters, Mapper<T>.Map, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        public Task<QueryResult<IEnumerable<T>>> ExecuteListAsync<T>(string cmdText, CommandType cmdType, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteListAsync<T>(cmdText, cmdType, null, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        public Task<QueryResult<IEnumerable<T>>> ExecuteListAsync<T>(string cmdText, CommandType cmdType, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            return ExecuteListAsync<T>(cmdText, cmdType, Mapper<T>.Map, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        public Task<QueryResult<IEnumerable<T>>> ExecuteListAsync<T>(string cmdText, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteListAsync<T>(cmdText, CommandType.Text, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        public Task<QueryResult<IEnumerable<T>>> ExecuteListAsync<T>(string cmdText, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            return ExecuteListAsync<T>(cmdText, Mapper<T>.Map, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public virtual async Task<QueryResult<Dictionary<TDictionaryKey, TElement>>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var reader = await ExecuteReaderAsync(cmdText, cmdType, parameters, cancellationToken))
            {
                var dict = new Dictionary<TDictionaryKey, TElement>();

                while (reader.Read())
                {
                    dict.Add(keyProjector(reader.GetValue(0)), elementProjector(reader.GetValue(1)));
                }

                return new QueryResult<Dictionary<TDictionaryKey, TElement>>(dict);
            }
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Task<QueryResult<Dictionary<TDictionaryKey, TElement>>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDictionaryAsync<TDictionaryKey, TElement>(cmdText, cmdType, null, keyProjector, elementProjector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Task<QueryResult<Dictionary<TDictionaryKey, TElement>>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, Dictionary<string, object> parameters, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDictionaryAsync<TDictionaryKey, TElement>(cmdText, CommandType.Text, parameters, keyProjector, elementProjector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Task<QueryResult<Dictionary<TDictionaryKey, TElement>>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDictionaryAsync<TDictionaryKey, TElement>(cmdText, null, keyProjector, elementProjector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Task<QueryResult<Dictionary<TDictionaryKey, TElement>>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDictionaryAsync<TDictionaryKey, TElement>(cmdText, cmdType, parameters, ConvertType<TDictionaryKey>(), ConvertType<TElement>(), cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Task<QueryResult<Dictionary<TDictionaryKey, TElement>>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDictionaryAsync<TDictionaryKey, TElement>(cmdText, CommandType.Text, parameters, cancellationToken);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns>The connection.</returns>
        protected virtual DbConnection CreateConnection()
        {
            if (_transaction != null)
                throw new InvalidOperationException("Unable to create a new connection. A transaction has already been started.");

            if (_ownsConnection)
            {
                _connection = _factory.CreateConnection();
                _connection.ConnectionString = _connectionString;
            }

            return _connection;
        }

        /// <summary>
        /// Creates a command.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>The new command.</returns>
        protected DbCommand CreateCommand(string cmdText, CommandType cmdType, Dictionary<string, object> parameters)
        {
            var command = CreateCommand();

            command.CommandText = cmdText;
            command.CommandType = cmdType;
            command.AddParmeters(parameters);

            return command;
        }

        #endregion

        #region Private Methods

        private static T ConvertValue<T>(object value)
        {
            if (value == null || value is DBNull)
                return default(T);

            if (value is T)
                return (T)value;

            return (T)Convert.ChangeType(value, typeof(T));
        }

        private static Func<object, TElement> ConvertType<TElement>()
        {
            return ConvertValue<TElement>;
        }

        private void ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(object[] keyValues) where TEntity : class
        {
            if (keyValues.Length != PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos<TEntity>().Count())
                throw new ArgumentException(DotNetToolkit.Repository.Properties.Resources.EntityPrimaryKeyValuesLengthMismatch, nameof(keyValues));
        }

        private async Task OnSchemaValidationAsync(Type entityType, CancellationToken cancellationToken)
        {
            // Performs some schema validation for this type (either creates the table if does not exist, or validates)
            if (!_schemaValidationTypeMapping.ContainsKey(entityType))
            {
                try { await _schemaConfigHelper.ExecuteSchemaValidateAsync(entityType, cancellationToken); }
                finally { _schemaValidationTypeMapping[entityType] = true; }
            }
        }

        private void OnSchemaValidation(Type entityType)
        {
            // Performs some schema validation for this type (either creates the table if does not exist, or validates)
            if (!_schemaValidationTypeMapping.ContainsKey(entityType))
            {
                try { _schemaConfigHelper.ExecuteSchemaValidate(entityType); }
                finally { _schemaValidationTypeMapping[entityType] = true; }
            }
        }

        private static string FormatExecutingDebugQuery(string commandName, Dictionary<string, object> parameters, string sql)
        {
            var sb = new StringBuilder();

            sb.Append($"Executing [ Command = {commandName}");
            if (parameters != null && parameters.Count > 0)
            {
                sb.Append(", ");
                sb.Append($"Parameters = {parameters.ToDebugString()}");
            }
            sb.Append(" ]");
            sb.Append(Environment.NewLine);
            sb.Append(sql.Indent(3));

            return sb.ToString();
        }

        #endregion

        #region Implementation of IRepositoryContextAsync

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of state entries written to the database.</returns>
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (var command = CreateCommand())
            {
                var rows = 0;
                var connection = command.Connection;
                var ownsConnection = _ownsConnection && command.Transaction == null;

                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync(cancellationToken);

                try
                {
                    while (_items.TryTake(out var entitySet))
                    {
                        var entityType = entitySet.Entity.GetType();

                        await OnSchemaValidationAsync(entityType, cancellationToken);

                        var primeryKeyPropertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();
                        var isIdentity = primeryKeyPropertyInfo.IsColumnIdentity();

                        // Checks if the entity exist in the database
                        var existInDb = await command.ExecuteObjectExistAsync(entitySet.Entity, cancellationToken);

                        // Prepare the sql statement
                        _queryBuilder.PrepareEntitySetQuery(
                            entitySet,
                            existInDb,
                            isIdentity,
                            primeryKeyPropertyInfo,
                            out string sql,
                            out Dictionary<string, object> parameters);

                        // Executes the sql statement
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.AddParmeters(parameters);

                        if (Logger.IsEnabled(LogLevel.Debug))
                            Logger.Debug(FormatExecutingDebugQuery("ExecuteNonQueryAsync", parameters, sql));

                        rows += await command.ExecuteNonQueryAsync(cancellationToken);

                        // Checks to see if the model needs to be updated with the new key returned from the database
                        if (entitySet.State == EntityState.Added && isIdentity)
                        {
                            command.CommandText = "SELECT @@IDENTITY";
                            command.Parameters.Clear();

                            if (Logger.IsEnabled(LogLevel.Debug))
                                Logger.Debug(FormatExecutingDebugQuery("ExecuteScalarAsync", null, command.CommandText));

                            var newKey = await command.ExecuteScalarAsync(cancellationToken);
                            var convertedKeyValue = Convert.ChangeType(newKey, primeryKeyPropertyInfo.PropertyType);

                            primeryKeyPropertyInfo.SetValue(entitySet.Entity, convertedKeyValue, null);
                        }
                    }
                }
                finally
                {
                    if (ownsConnection)
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
        public Task<QueryResult<TEntity>> FindAsync<TEntity>(CancellationToken cancellationToken, IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(keyValues);

            var options = new QueryOptions<TEntity>().Include(PrimaryKeyConventionHelper.GetByPrimaryKeySpecification<TEntity>(keyValues));

            if (fetchStrategy != null)
                options.Include(fetchStrategy);

            return FindAsync<TEntity, TEntity>(options, IdentityExpression<TEntity>.Instance, cancellationToken);
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

            var selectorFunc = selector.Compile();

            _queryBuilder.PrepareDefaultSelectQuery(options, out Mapper mapper);

            await OnSchemaValidationAsync(typeof(TEntity), cancellationToken);

            return await ExecuteObjectAsync<TResult>(mapper.Sql, mapper.Parameters, reader => mapper.Map<TEntity, TResult>(reader, selectorFunc), cancellationToken);
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

            var selectorFunc = selector.Compile();

            _queryBuilder.PrepareDefaultSelectQuery(options, out Mapper mapper, true);

            await OnSchemaValidationAsync(typeof(TEntity), cancellationToken);

            return await ExecuteListAsync<TResult>(mapper.Sql, mapper.Parameters, reader => mapper.Map<TEntity, TResult>(reader, selectorFunc), cancellationToken);
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
            _queryBuilder.PrepareCountQuery(options, out Mapper mapper);

            OnSchemaValidation(typeof(TEntity));

            var result = await ExecuteScalarAsync<int>(mapper.Sql, mapper.Parameters, cancellationToken);

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

            _queryBuilder.PrepareDefaultSelectQuery(options, out Mapper mapper);

            await OnSchemaValidationAsync(typeof(TEntity), cancellationToken);

            using (var reader = await ExecuteReaderAsync(mapper.Sql, mapper.Parameters, cancellationToken))
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
        public async Task<QueryResult<Dictionary<TDictionaryKey, TElement>>> ToDictionaryAsync<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            _queryBuilder.PrepareDefaultSelectQuery(options, out Mapper mapper, true);

            await OnSchemaValidationAsync(typeof(TEntity), cancellationToken);

            using (var reader = await ExecuteReaderAsync(mapper.Sql, mapper.Parameters, cancellationToken))
            {
                var dict = new Dictionary<TDictionaryKey, TElement>();
                var foundCrossJoinCountColumn = false;
                var total = 0;

                while (reader.Read())
                {
                    // TODO: NEEDS TO FIGURE OUT ANOTHER WAY TO DO THIS
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);

                        if (name.Equals(QueryBuilder.CrossJoinCountColumnName))
                        {
                            total = (int)reader[name];
                            foundCrossJoinCountColumn = true;
                            break;
                        }
                    }

                    dict.Add(mapper.Map<TEntity, TDictionaryKey>(reader, keySelectFunc), mapper.Map<TEntity, TElement>(reader, elementSelectorFunc));
                }

                if (!foundCrossJoinCountColumn)
                    total = dict.Count;

                return new QueryResult<Dictionary<TDictionaryKey, TElement>>(dict, total);
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
        public async Task<QueryResult<IEnumerable<TResult>>> GroupByAsync<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
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

            return new QueryResult<IEnumerable<TResult>>(result, queryResult.Total.GetValueOrDefault());
        }

        #endregion

        #region Implementation of IRepositoryContext

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns>The transaction.</returns>
        public ITransactionManager BeginTransaction()
        {
            _transaction = CreateConnection().BeginTransaction();

            return new AdoNetTransactionManager(_transaction, Logger);
        }

        /// <summary>
        /// Sets the repository context logger provider to use.
        /// </summary>
        /// <param name="loggerProvider">The logger provider.</param>
        public void UseLoggerProvider(ILoggerProvider loggerProvider)
        {
            if (loggerProvider == null)
                throw new ArgumentNullException(nameof(loggerProvider));

            Logger = loggerProvider.Create(GetType().FullName);
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be inserted into the database when <see cref="SaveChanges" /> is called..
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _items.Add(new EntitySet(entity, EntityState.Added));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be updated in the database when <see cref="SaveChanges" /> is called..
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            _items.Add(new EntitySet(entity, EntityState.Modified));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be removed from the database when <see cref="SaveChanges" /> is called..
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
            using (var command = CreateCommand())
            {
                var rows = 0;
                var connection = command.Connection;
                var ownsConnection = _ownsConnection && command.Transaction == null;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                try
                {
                    while (_items.TryTake(out var entitySet))
                    {
                        var entityType = entitySet.Entity.GetType();

                        OnSchemaValidation(entityType);

                        var primeryKeyPropertyInfo =
                            PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();
                        var isIdentity = primeryKeyPropertyInfo.IsColumnIdentity();

                        // Checks if the entity exist in the database
                        var existInDb = command.ExecuteObjectExist(entitySet.Entity);

                        // Prepare the sql statement
                        _queryBuilder.PrepareEntitySetQuery(
                            entitySet,
                            existInDb,
                            isIdentity,
                            primeryKeyPropertyInfo,
                            out string sql,
                            out Dictionary<string, object> parameters);

                        // Executes the sql statement
                        command.CommandText = sql;
                        command.CommandType = CommandType.Text;
                        command.Parameters.Clear();
                        command.AddParmeters(parameters);

                        rows += command.ExecuteNonQuery();

                        if (Logger.IsEnabled(LogLevel.Debug))
                            Logger.Debug(FormatExecutingDebugQuery("ExecuteNonQuery", parameters, sql));

                        // Checks to see if the model needs to be updated with the new key returned from the database
                        if (entitySet.State == EntityState.Added && isIdentity)
                        {
                            command.CommandText = "SELECT @@IDENTITY";
                            command.Parameters.Clear();

                            if (Logger.IsEnabled(LogLevel.Debug))
                                Logger.Debug(FormatExecutingDebugQuery("ExecuteScalar", null, command.CommandText));

                            var newKey = command.ExecuteScalar();
                            var convertedKeyValue = Convert.ChangeType(newKey, primeryKeyPropertyInfo.PropertyType);

                            primeryKeyPropertyInfo.SetValue(entitySet.Entity, convertedKeyValue, null);
                        }
                    }
                }
                finally
                {
                    if (ownsConnection)
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
        public QueryResult<TEntity> Find<TEntity>(IFetchQueryStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(keyValues);

            var options = new QueryOptions<TEntity>().Include(PrimaryKeyConventionHelper.GetByPrimaryKeySpecification<TEntity>(keyValues));

            if (fetchStrategy != null)
                options.Include(fetchStrategy);

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

            var selectorFunc = selector.Compile();

            _queryBuilder.PrepareDefaultSelectQuery(options, out Mapper mapper);

            OnSchemaValidation(typeof(TEntity));

            return ExecuteObject<TResult>(mapper.Sql, mapper.Parameters, reader => mapper.Map<TEntity, TResult>(reader, selectorFunc));
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

            var selectorFunc = selector.Compile();

            _queryBuilder.PrepareDefaultSelectQuery(options, out Mapper mapper, true);

            OnSchemaValidation(typeof(TEntity));

            return ExecuteList<TResult>(mapper.Sql, mapper.Parameters, reader => mapper.Map<TEntity, TResult>(reader, selectorFunc));
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public QueryResult<int> Count<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            _queryBuilder.PrepareCountQuery(options, out Mapper mapper);

            OnSchemaValidation(typeof(TEntity));

            var result = ExecuteScalar<int>(mapper.Sql, mapper.Parameters);

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

            _queryBuilder.PrepareDefaultSelectQuery(options, out Mapper mapper);

            OnSchemaValidation(typeof(TEntity));

            using (var reader = ExecuteReader(mapper.Sql, mapper.Parameters))
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
        public QueryResult<Dictionary<TDictionaryKey, TElement>> ToDictionary<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            _queryBuilder.PrepareDefaultSelectQuery(options, out Mapper mapper, true);

            OnSchemaValidation(typeof(TEntity));

            using (var reader = ExecuteReader(mapper.Sql, mapper.Parameters))
            {
                var dict = new Dictionary<TDictionaryKey, TElement>();
                var foundCrossJoinCountColumn = false;
                var total = 0;

                while (reader.Read())
                {
                    // TODO: NEEDS TO FIGURE OUT ANOTHER WAY TO DO THIS
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);

                        if (name.Equals(QueryBuilder.CrossJoinCountColumnName))
                        {
                            total = (int)reader[name];
                            foundCrossJoinCountColumn = true;
                            break;
                        }
                    }

                    dict.Add(mapper.Map<TEntity, TDictionaryKey>(reader, keySelectFunc), mapper.Map<TEntity, TElement>(reader, elementSelectorFunc));
                }

                if (!foundCrossJoinCountColumn)
                    total = dict.Count;

                return new QueryResult<Dictionary<TDictionaryKey, TElement>>(dict, total);
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
        public QueryResult<IEnumerable<TResult>> GroupBy<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector) where TEntity : class
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

            return new QueryResult<IEnumerable<TResult>>(result, queryResult.Total.GetValueOrDefault());
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_ownsConnection && _connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }

            _transaction = null;

            GC.SuppressFinalize(this);
        }

        #endregion
    }
}