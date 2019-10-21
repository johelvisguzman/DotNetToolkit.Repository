namespace DotNetToolkit.Repository.AdoNet
{
    using Configuration.Conventions;
    using Configuration.Logging;
    using Configuration.Logging.Internal;
    using Extensions;
    using Extensions.Internal;
    using Internal;
    using Queries;
    using Queries.Internal;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Utility;

    /// <summary>
    /// Represents a database helper which contains various methods for retrieving and manipulating data in a database.
    /// </summary>
    public sealed class DbHelper : IDisposable
    {
        #region Fields

        private DbTransaction _underlyingTransaction;
        private readonly DbProviderFactory _factory;
        private readonly string _connectionString;
        private DbConnection _connection;
        private readonly bool _ownsConnection;
        private readonly IRepositoryConventions _conventions;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the provider factory.
        /// </summary>
        public DbProviderFactory DbProviderFactory { get { return _factory; } }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public string ConnectionString { get { return _connectionString; } }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        public DbConnection Connection { get { return _connection; } }

        /// <summary>
        /// Gets a value indicating whether this instance owns the connection.
        /// </summary>
        public bool OwnsConnection { get { return _ownsConnection && _underlyingTransaction == null; } }

        /// <summary>
        /// Gets the repository context logger.
        /// </summary>
        internal ILogger Logger { get; set; } = NullLogger.Instance;

        /// <summary>
        /// Gets the repository conventions.
        /// </summary>
        public IRepositoryConventions Conventions { get { return _conventions; } }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DbHelper" /> class.
        /// </summary>
        /// <param name="conventions">The configurable conventions.</param>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        internal DbHelper(IRepositoryConventions conventions, string nameOrConnectionString)
        {
            Guard.NotNull(conventions, nameof(conventions));
            Guard.NotEmpty(nameOrConnectionString, nameof(nameOrConnectionString));

            var css = Guard.EnsureNotNull(
                GetConnectionStringSettings(nameOrConnectionString),
                "The connection string does not exist in your configuration file.");

            _conventions = conventions;
            _factory = DbProviderFactories.GetFactory(css.ProviderName);
            _connectionString = css.ConnectionString;
            _ownsConnection = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbHelper" /> class.
        /// </summary>
        /// <param name="conventions">The configurable conventions.</param>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        internal DbHelper(IRepositoryConventions conventions, string providerName, string connectionString)
        {
            Guard.NotNull(conventions, nameof(conventions));
            Guard.NotEmpty(providerName, nameof(providerName));
            Guard.NotEmpty(connectionString, nameof(connectionString));

            _conventions = conventions;
            _factory = DbProviderFactories.GetFactory(providerName);
            _connectionString = connectionString;
            _ownsConnection = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbHelper" /> class.
        /// </summary>
        /// <param name="conventions">The configurable conventions.</param>
        /// <param name="existingConnection">The existing connection.</param>
        internal DbHelper(IRepositoryConventions conventions, DbConnection existingConnection)
        {
            Guard.NotNull(conventions, nameof(conventions));
            Guard.NotNull(existingConnection, nameof(existingConnection));

            _conventions = conventions;
            _factory = DbProviderFactories.GetFactory(existingConnection);
            _connection = existingConnection;
            _ownsConnection = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts a database transaction.
        /// </summary>
        /// <returns>The new transaction.</returns>
        public DbTransaction BeginTransaction()
        {
            _underlyingTransaction = CreateConnection().BeginTransaction();

            return _underlyingTransaction;
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns>The connection.</returns>
        public DbConnection CreateConnection()
        {
            if (_underlyingTransaction != null)
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
        public DbCommand CreateCommand(string cmdText, CommandType cmdType, Dictionary<string, object> parameters)
        {
            Guard.NotEmpty(cmdText, nameof(cmdText));

            var command = CreateCommand();

            command.CommandText = cmdText;
            command.CommandType = cmdType;
            command.Parameters.Clear();
            command.AddParameters(parameters);

            return command;
        }

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <returns>The new command.</returns>
        public DbCommand CreateCommand()
        {
            var command = _ownsConnection
                ? _factory.CreateCommand()
                : _connection.CreateCommand();

            Guard.EnsureNotNull(command, "Unable to create a command.");

            DbConnection connection;

            if (_underlyingTransaction != null)
            {
                connection = _underlyingTransaction.Connection;

                try
                {
                    command.Transaction = _underlyingTransaction;
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
        /// <param name="command">The command.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteNonQuery(DbCommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            var connection = command.Connection;
            var ownsConnection = _ownsConnection && command.Transaction == null;
            var canCloseConnection = false;

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                canCloseConnection = true;
            }

            LogExecutingCommandQuery(command);

            var result = command.ExecuteNonQuery();

            if (canCloseConnection && ownsConnection)
                connection.Dispose();

            return result;
        }

        /// <summary>
        /// Executes a SQL statement against a connection.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteNonQuery(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            using (var command = CreateCommand(cmdText, cmdType, parameters))
            {
                return ExecuteNonQuery(command);
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
        /// <param name="command">The command.</param>
        /// <returns>A <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        public DbDataReader ExecuteReader(DbCommand command)
        {
            Guard.NotNull(command, nameof(command));

            var connection = command.Connection;
            var ownsConnection = _ownsConnection && command.Transaction == null;
            var canCloseConnection = false;

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                canCloseConnection = true;
            }

            LogExecutingCommandQuery(command);

            var reader = command.ExecuteReader(canCloseConnection && ownsConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default);

            return reader;
        }

        /// <summary>
        /// Sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DbDataReader" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        public DbDataReader ExecuteReader(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            return ExecuteReader(CreateCommand(cmdText, cmdType, parameters));
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
        /// <param name="command">The command.</param>
        /// <returns>The first column of the first row in the result set returned by the query.</returns>
        public T ExecuteScalar<T>(DbCommand command)
        {
            Guard.NotNull(command, nameof(command));

            var connection = command.Connection;
            var ownsConnection = _ownsConnection && command.Transaction == null;
            var canCloseConnection = false;

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                canCloseConnection = true;
            }

            LogExecutingCommandQuery(command);

            var result = ConvertValue<T>(command.ExecuteScalar());

            if (canCloseConnection && ownsConnection)
                connection.Dispose();

            return result;
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>The first column of the first row in the result set returned by the query.</returns>
        public T ExecuteScalar<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            using (var command = CreateCommand(cmdText, cmdType, parameters))
            {
                return ExecuteScalar<T>(command);
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
        public T ExecuteObject<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
        {
            using (var reader = ExecuteReader(cmdText, cmdType, parameters))
            {
                var list = new List<T>();

                while (reader.Read())
                {
                    list.Add(projector(reader));
                }

                var result = list.FirstOrDefault();

                return result;
            }
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>An entity which has been projected into a new form.</returns>
        public T ExecuteObject<T>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
        {
            return ExecuteObject<T>(cmdText, CommandType.Text, parameters, projector);
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
        internal IPagedQueryResult<IEnumerable<T>> ExecuteList<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
        {
            using (var reader = ExecuteReader(cmdText, cmdType, parameters))
            {
                var list = new List<T>();
                var foundCrossJoinCountColumn = false;
                var total = 0;

                QueryBuilder.ExtractCrossJoinColumnName(cmdText, out var crossJoinColumnName);

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

                    list.Add(projector(reader));
                }

                if (!foundCrossJoinCountColumn)
                    total = list.Count;

                return new PagedQueryResult<IEnumerable<T>>(list, total);
            }
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        internal IPagedQueryResult<IEnumerable<T>> ExecuteList<T>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
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
        internal IPagedQueryResult<IEnumerable<T>> ExecuteList<T>(string cmdText, Dictionary<string, object> parameters) where T : class
        {
            var mapper = new Mapper<T>(_conventions);

            return ExecuteList<T>(cmdText, parameters, mapper.Map);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        internal IPagedQueryResult<IEnumerable<T>> ExecuteList<T>(string cmdText, CommandType cmdType, Func<DbDataReader, T> projector)
        {
            return ExecuteList<T>(cmdText, cmdType, null, projector);
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
        public Dictionary<TDictionaryKey, TElement> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector)
        {
            using (var reader = ExecuteReader(cmdText, cmdType, parameters))
            {
                var dict = new Dictionary<TDictionaryKey, TElement>();

                while (reader.Read())
                {
                    dict.Add(keyProjector(reader.GetValue(0)), elementProjector(reader.GetValue(1)));
                }

                return new Dictionary<TDictionaryKey, TElement>(dict);
            }
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
        public Dictionary<TDictionaryKey, TElement> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
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
        public Dictionary<TDictionaryKey, TElement> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, Dictionary<string, object> parameters = null)
        {
            return ExecuteDictionary<TDictionaryKey, TElement>(cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="startRecord">The zero-based record number to start with.</param>
        /// <param name="maxRecords">The maximum number of records to retrieve.</param>
        /// <returns>A <see cref="System.Data.DataTable" /> object.</returns>
        public DataTable ExecuteDataTable(DbCommand command, int startRecord, int maxRecords)
        {
            Guard.NotNull(command, nameof(command));

            var connection = command.Connection;
            var ownsConnection = _ownsConnection && command.Transaction == null;
            var canCloseConnection = false;

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                canCloseConnection = true;
            }

            LogExecutingCommandQuery(command);

            var adapter = _factory.CreateDataAdapter();

            adapter.SelectCommand = command;

            var dt = new DataTable();

            if (startRecord >= 0 || maxRecords >= 0)
                adapter.Fill(startRecord, maxRecords, dt);
            else
                adapter.Fill(dt);

            if (canCloseConnection && ownsConnection)
                connection.Dispose();

            return dt;
        }

        /// <summary>
        /// Sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>A <see cref="System.Data.DataTable" /> object.</returns>
        public DataTable ExecuteDataTable(DbCommand command)
        {
            return ExecuteDataTable(command, -1, -1);
        }

        /// <summary>
        /// Sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="startRecord">The zero-based record number to start with.</param>
        /// <param name="maxRecords">The maximum number of records to retrieve.</param>
        /// <returns>A <see cref="System.Data.DataTable" /> object.</returns>
        public DataTable ExecuteDataTable(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, int startRecord, int maxRecords)
        {
            return ExecuteDataTable(CreateCommand(cmdText, cmdType, parameters), startRecord, maxRecords);
        }

        /// <summary>
        /// Sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="startRecord">The zero-based record number to start with.</param>
        /// <param name="maxRecords">The maximum number of records to retrieve.</param>
        /// <returns>A <see cref="System.Data.DataTable" /> object.</returns>
        public DataTable ExecuteDataTable(string cmdText, CommandType cmdType, int startRecord, int maxRecords)
        {
            return ExecuteDataTable(cmdText, cmdType, null, startRecord, maxRecords);
        }

        /// <summary>
        /// Sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="System.Data.DataTable" /> object.</returns>
        public DataTable ExecuteDataTable(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            return ExecuteDataTable(cmdText, cmdType, parameters, -1, -1);
        }

        /// <summary>
        /// Sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="startRecord">The zero-based record number to start with.</param>
        /// <param name="maxRecords">The maximum number of records to retrieve.</param>
        /// <returns>A <see cref="System.Data.DataTable" /> object.</returns>
        public DataTable ExecuteDataTable(string cmdText, Dictionary<string, object> parameters, int startRecord, int maxRecords)
        {
            return ExecuteDataTable(cmdText, CommandType.Text, parameters, startRecord, maxRecords);
        }

        /// <summary>
        /// Sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="startRecord">The zero-based record number to start with.</param>
        /// <param name="maxRecords">The maximum number of records to retrieve.</param>
        /// <returns>A <see cref="System.Data.DataTable" /> object.</returns>
        public DataTable ExecuteDataTable(string cmdText, int startRecord, int maxRecords)
        {
            return ExecuteDataTable(cmdText, null, startRecord, maxRecords);
        }

        /// <summary>
        /// Sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="System.Data.DataTable" /> object.</returns>
        public DataTable ExecuteDataTable(string cmdText, Dictionary<string, object> parameters = null)
        {
            return ExecuteDataTable(cmdText, CommandType.Text, parameters, -1, -1);
        }

        /// <summary>
        /// Sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataSet" />.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>A <see cref="System.Data.DataSet" /> object.</returns>
        public DataSet ExecuteDataSet(DbCommand command)
        {
            Guard.NotNull(command, nameof(command));

            var connection = command.Connection;
            var ownsConnection = _ownsConnection && command.Transaction == null;
            var canCloseConnection = false;

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                canCloseConnection = true;
            }

            LogExecutingCommandQuery(command);

            var adapter = _factory.CreateDataAdapter();

            adapter.SelectCommand = command;

            var ds = new DataSet();

            adapter.Fill(ds);

            if (canCloseConnection && ownsConnection)
                connection.Dispose();

            return ds;
        }

        /// <summary>
        /// Sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataSet" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="System.Data.DataSet" /> object.</returns>
        public DataSet ExecuteDataSet(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            return ExecuteDataSet(CreateCommand(cmdText, cmdType, parameters));
        }

        /// <summary>
        /// Sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataSet" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A <see cref="System.Data.DataSet" /> object.</returns>
        public DataSet ExecuteDataSet(string cmdText, Dictionary<string, object> parameters = null)
        {
            return ExecuteDataSet(cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Asynchronously executes a SQL statement against a connection.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        public async Task<int> ExecuteNonQueryAsync(DbCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.NotNull(command, nameof(command));

            var connection = command.Connection;
            var ownsConnection = _ownsConnection && command.Transaction == null;
            var canCloseConnection = false;

            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync(cancellationToken);
                canCloseConnection = true;
            }

            LogExecutingCommandQuery(command);

            var result = await command.ExecuteNonQueryAsync(cancellationToken);

            if (canCloseConnection && ownsConnection)
                connection.Dispose();

            return result;
        }

        /// <summary>
        /// Asynchronously executes a SQL statement against a connection.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        public Task<int> ExecuteNonQueryAsync(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var command = CreateCommand(cmdText, cmdType, parameters))
            {
                return ExecuteNonQueryAsync(command, cancellationToken);
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
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        public async Task<DbDataReader> ExecuteReaderAsync(DbCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.NotNull(command, nameof(command));

            var connection = command.Connection;
            var ownsConnection = _ownsConnection && command.Transaction == null;
            var canCloseConnection = false;

            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync(cancellationToken);
                canCloseConnection = true;
            }

            LogExecutingCommandQuery(command);

            var reader = await command.ExecuteReaderAsync(canCloseConnection && ownsConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default, cancellationToken);

            return reader;
        }

        /// <summary>
        /// Asynchronously sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DbDataReader" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        public Task<DbDataReader> ExecuteReaderAsync(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteReaderAsync(CreateCommand(cmdText, cmdType, parameters), cancellationToken);
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
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the first column of the first row in the result set returned by the query.</returns>
        public async Task<T> ExecuteScalarAsync<T>(DbCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.NotNull(command, nameof(command));

            var connection = command.Connection;
            var ownsConnection = _ownsConnection && command.Transaction == null;
            var canCloseConnection = false;

            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync(cancellationToken);
                canCloseConnection = true;
            }

            LogExecutingCommandQuery(command);

            var result = ConvertValue<T>(await command.ExecuteScalarAsync(cancellationToken));

            if (canCloseConnection && ownsConnection)
                connection.Dispose();

            return result;
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
        public Task<T> ExecuteScalarAsync<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var command = CreateCommand(cmdText, cmdType, parameters))
            {
                return ExecuteScalarAsync<T>(command, cancellationToken);
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
        public async Task<T> ExecuteObjectAsync<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var reader = await ExecuteReaderAsync(cmdText, cmdType, parameters, cancellationToken))
            {
                var list = new List<T>();

                while (reader.Read())
                {
                    list.Add(projector(reader));
                }

                var result = list.FirstOrDefault();

                return result;
            }
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
        public Task<T> ExecuteObjectAsync<T>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteObjectAsync<T>(cmdText, CommandType.Text, parameters, projector, cancellationToken);
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
        internal async Task<IPagedQueryResult<IEnumerable<T>>> ExecuteListAsync<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var reader = await ExecuteReaderAsync(cmdText, cmdType, parameters, cancellationToken))
            {
                var list = new List<T>();
                var foundCrossJoinCountColumn = false;
                var total = 0;

                QueryBuilder.ExtractCrossJoinColumnName(cmdText, out var crossJoinColumnName);

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

                    list.Add(projector(reader));
                }

                if (!foundCrossJoinCountColumn)
                    total = list.Count;

                return new PagedQueryResult<IEnumerable<T>>(list, total);
            }
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
        internal Task<IPagedQueryResult<IEnumerable<T>>> ExecuteListAsync<T>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
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
        internal Task<IPagedQueryResult<IEnumerable<T>>> ExecuteListAsync<T>(string cmdText, Dictionary<string, object> parameters, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            var mapper = new Mapper<T>(_conventions);

            return ExecuteListAsync<T>(cmdText, parameters, mapper.Map, cancellationToken);
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
        public async Task<Dictionary<TDictionaryKey, TElement>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var reader = await ExecuteReaderAsync(cmdText, cmdType, parameters, cancellationToken))
            {
                var dict = new Dictionary<TDictionaryKey, TElement>();

                while (reader.Read())
                {
                    dict.Add(keyProjector(reader.GetValue(0)), elementProjector(reader.GetValue(1)));
                }

                return new Dictionary<TDictionaryKey, TElement>(dict);
            }
        }

        /// <summary>
        /// Asynchronously sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="startRecord">The zero-based record number to start with.</param>
        /// <param name="maxRecords">The maximum number of records to retrieve.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.DataTable" /> object.</returns>
        public async Task<DataTable> ExecuteDataTableAsync(DbCommand command, int startRecord, int maxRecords, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.NotNull(command, nameof(command));

            var connection = command.Connection;
            var ownsConnection = _ownsConnection && command.Transaction == null;
            var canCloseConnection = false;

            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync(cancellationToken);
                canCloseConnection = true;
            }

            LogExecutingCommandQuery(command);

            var adapter = _factory.CreateDataAdapter();

            adapter.SelectCommand = command;

            var dt = new DataTable();

            if (startRecord >= 0 || maxRecords >= 0)
                adapter.Fill(startRecord, maxRecords, dt);
            else
                adapter.Fill(dt);

            if (canCloseConnection && ownsConnection)
                connection.Dispose();

            return dt;
        }

        /// <summary>
        /// Asynchronously sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.DataTable" /> object.</returns>
        public Task<DataTable> ExecuteDataTableAsync(DbCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDataTableAsync(command, -1, -1, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="startRecord">The zero-based record number to start with.</param>
        /// <param name="maxRecords">The maximum number of records to retrieve.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.DataTable" /> object.</returns>
        public Task<DataTable> ExecuteDataTableAsync(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, int startRecord, int maxRecords, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDataTableAsync(CreateCommand(cmdText, cmdType, parameters), startRecord, maxRecords, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="startRecord">The zero-based record number to start with.</param>
        /// <param name="maxRecords">The maximum number of records to retrieve.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.DataTable" /> object.</returns>
        public Task<DataTable> ExecuteDataTableAsync(string cmdText, CommandType cmdType, int startRecord, int maxRecords, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDataTableAsync(cmdText, cmdType, null, startRecord, maxRecords, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.DataTable" /> object.</returns>
        public Task<DataTable> ExecuteDataTableAsync(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDataTableAsync(cmdText, cmdType, parameters, -1, -1, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="startRecord">The zero-based record number to start with.</param>
        /// <param name="maxRecords">The maximum number of records to retrieve.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.DataTable" /> object.</returns>
        public Task<DataTable> ExecuteDataTableAsync(string cmdText, Dictionary<string, object> parameters, int startRecord, int maxRecords, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDataTableAsync(cmdText, CommandType.Text, parameters, startRecord, maxRecords, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="startRecord">The zero-based record number to start with.</param>
        /// <param name="maxRecords">The maximum number of records to retrieve.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.DataTable" /> object.</returns>
        public Task<DataTable> ExecuteDataTableAsync(string cmdText, int startRecord, int maxRecords, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDataTableAsync(cmdText, null, startRecord, maxRecords, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataTable" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.DataTable" /> object.</returns>
        public Task<DataTable> ExecuteDataTableAsync(string cmdText, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDataTableAsync(cmdText, CommandType.Text, parameters, -1, -1, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataSet" />.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.DataSet" /> object.</returns>
        public async Task<DataSet> ExecuteDataSetAsync(DbCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.NotNull(command, nameof(command));

            var connection = command.Connection;
            var ownsConnection = _ownsConnection && command.Transaction == null;
            var canCloseConnection = false;

            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync(cancellationToken);
                canCloseConnection = true;
            }

            LogExecutingCommandQuery(command);

            var adapter = _factory.CreateDataAdapter();

            adapter.SelectCommand = command;

            var ds = new DataSet();

            adapter.Fill(ds);

            if (canCloseConnection && ownsConnection)
                connection.Dispose();

            return ds;
        }

        /// <summary>
        /// Asynchronously sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataSet" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.DataSet" /> object.</returns>
        public Task<DataSet> ExecuteDataSetAsync(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDataSetAsync(CreateCommand(cmdText, cmdType, parameters), cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends the query string to the <see cref="DbConnection" /> and builds a <see cref="DataSet" />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.DataSet" /> object.</returns>
        public Task<DataSet> ExecuteDataSetAsync(string cmdText, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDataSetAsync(cmdText, CommandType.Text, parameters, cancellationToken);
        }

        /// <summary>
        /// Maps the specified string sql data type value to <see cref="System.Type" />.
        /// </summary>
        public static Type MapToType(string sqlDataType)
        {
            return MapToType(MapToSqlDbType(sqlDataType));
        }

        /// <summary>
        /// Maps the specified type to <see cref="System.Data.SqlDbType" />.
        /// </summary>
        public static SqlDbType MapToSqlDbType(Type type)
        {
            var typeMap = new Dictionary<Type, SqlDbType>
            {
                [typeof(string)] = SqlDbType.NVarChar,
                [typeof(char[])] = SqlDbType.NVarChar,
                [typeof(byte)] = SqlDbType.TinyInt,
                [typeof(short)] = SqlDbType.SmallInt,
                [typeof(int)] = SqlDbType.Int,
                [typeof(long)] = SqlDbType.BigInt,
                [typeof(byte[])] = SqlDbType.Image,
                [typeof(bool)] = SqlDbType.Bit,
                [typeof(DateTime)] = SqlDbType.DateTime2,
                [typeof(DateTimeOffset)] = SqlDbType.DateTimeOffset,
                [typeof(decimal)] = SqlDbType.Money,
                [typeof(float)] = SqlDbType.Real,
                [typeof(double)] = SqlDbType.Float,
                [typeof(TimeSpan)] = SqlDbType.Time
            };

            type = Nullable.GetUnderlyingType(type) ?? type;

            if (typeMap.ContainsKey(type))
            {
                return typeMap[type];
            }

            throw new ArgumentException($"{type.FullName} is not a supported .NET class");
        }

        #endregion

        #region Private Methods

        private static ConnectionStringSettings GetConnectionStringSettings(string nameOrConnectionString)
        {
            var css = ConfigurationManager.ConnectionStrings[nameOrConnectionString];

            if (css != null)
                return css;

            for (var i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
            {
                css = ConfigurationManager.ConnectionStrings[i];

                if (css.ConnectionString.Equals(nameOrConnectionString))
                    return css;
            }

            return null;
        }

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

        private void LogExecutingCommandQuery(DbCommand command, [CallerMemberName] string commandName = null)
        {
            if (Logger.IsEnabled(LogLevel.Debug))
                Logger.Debug(FormatCommandDebugQuery("Executing", command, commandName));
        }

        private static string FormatCommandDebugQuery(string action, DbCommand command, string commandName)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            var sb = new StringBuilder();

            sb.Append($"{action} [ Command = {commandName}, CommandType = {command.CommandType}");

            if (command.Parameters.Count > 0)
            {
                sb.Append(", ");
                sb.Append($"Parameters = {command.Parameters.ToDebugString()}");
            }

            sb.Append(" ]");
            sb.Append(Environment.NewLine);
            sb.Append(command.CommandText.Indent(3));

            return sb.ToString();
        }

        private static Type MapToType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return typeof(long);
                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    return typeof(byte[]);
                case SqlDbType.Bit:
                    return typeof(bool);
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    return typeof(string);
                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    return typeof(DateTime);
                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return typeof(decimal);
                case SqlDbType.Float:
                    return typeof(double);
                case SqlDbType.Int:
                    return typeof(int);
                case SqlDbType.Real:
                    return typeof(float?);
                case SqlDbType.UniqueIdentifier:
                    return typeof(Guid);
                case SqlDbType.SmallInt:
                    return typeof(short);
                case SqlDbType.TinyInt:
                    return typeof(byte);
                case SqlDbType.Variant:
                case SqlDbType.Udt:
                    return typeof(object);
                case SqlDbType.Structured:
                    return typeof(DataTable);
                case SqlDbType.DateTimeOffset:
                    return typeof(DateTimeOffset?);
                default:
                    throw new ArgumentOutOfRangeException(nameof(sqlType));
            }
        }

        private static SqlDbType MapToSqlDbType(string sqlDataType)
        {
            return (SqlDbType)Enum.Parse(typeof(SqlDbType), sqlDataType, true);
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

            _underlyingTransaction = null;
        }

        #endregion
    }
}
