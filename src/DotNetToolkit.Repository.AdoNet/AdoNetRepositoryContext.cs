namespace DotNetToolkit.Repository.AdoNet
{
    using FetchStrategies;
    using Helpers;
    using Internal;
    using Internal.Schema;
    using Properties;
    using Queries;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Transactions;

    /// <summary>
    /// Represents an ado.net repository context.
    /// </summary>
    /// <seealso cref="IRepositoryContextAsync" />
    public class AdoNetRepositoryContext : IRepositoryContextAsync, IHaveRepositoryContextConfiguration
    {
        #region Fields

        private DbTransaction _transaction;
        private readonly DbProviderFactory _factory;
        private readonly string _connectionString;
        private DbConnection _connection;

        private readonly BlockingCollection<EntitySet> _items = new BlockingCollection<EntitySet>();
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, PropertyInfo>> _sqlPropertiesMapping = new ConcurrentDictionary<Type, ConcurrentDictionary<string, PropertyInfo>>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryContext" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public AdoNetRepositoryContext(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            var ccs = ConfigurationManager.ConnectionStrings[connectionString];
            if (ccs == null)
                throw new ArgumentException("The connection string does not exist in your configuration file.");

            _factory = DbProviderFactories.GetFactory(ccs.ProviderName);
            _connectionString = connectionString;
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
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <returns>The new command.</returns>
        public virtual DbCommand CreateCommand()
        {
            var command = _factory.CreateCommand();

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
                var ownsConnection = command.Transaction == null;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

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
            var ownsConnection = command.Transaction == null;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            return command.ExecuteReader(ownsConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default);
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
                var ownsConnection = command.Transaction == null;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

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
        public virtual T ExecuteObject<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
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
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>An entity which has been projected into a new form.</returns>
        public T ExecuteObject<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters) where T : class
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
        public T ExecuteObject<T>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
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
        public T ExecuteObject<T>(string cmdText, Dictionary<string, object> parameters) where T : class
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
        public T ExecuteObject<T>(string cmdText, CommandType cmdType, Func<DbDataReader, T> projector)
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
        public T ExecuteObject<T>(string cmdText, CommandType cmdType) where T : class
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
        public T ExecuteObject<T>(string cmdText, Func<DbDataReader, T> projector)
        {
            return ExecuteObject<T>(cmdText, CommandType.Text, projector);
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <returns>An entity which has been projected into a new form.</returns>
        public T ExecuteObject<T>(string cmdText) where T : class
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
        public virtual IEnumerable<T> ExecuteList<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
        {
            using (var reader = ExecuteReader(cmdText, cmdType, parameters))
            {
                var list = new List<T>();

                while (reader.Read())
                {
                    list.Add(projector(reader));
                }

                return list;
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
        public IEnumerable<T> ExecuteList<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters) where T : class
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
        public IEnumerable<T> ExecuteList<T>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
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
        public IEnumerable<T> ExecuteList<T>(string cmdText, Dictionary<string, object> parameters) where T : class
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
        public IEnumerable<T> ExecuteList<T>(string cmdText, CommandType cmdType, Func<DbDataReader, T> projector)
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
        public IEnumerable<T> ExecuteList<T>(string cmdText, CommandType cmdType) where T : class
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
        public IEnumerable<T> ExecuteList<T>(string cmdText, Func<DbDataReader, T> projector)
        {
            return ExecuteList<T>(cmdText, CommandType.Text, projector);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public IEnumerable<T> ExecuteList<T>(string cmdText) where T : class
        {
            return ExecuteList<T>(cmdText, Mapper<T>.Map);
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="DataTable " />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A new <see cref="DataTable" />.</returns>
        public virtual DataTable ExecuteDataTable(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            using (var reader = ExecuteReader(cmdText, cmdType, parameters))
            {
                var dt = new DataTable();

                dt.Load(reader);

                return dt;
            }
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="DataTable " />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <returns>A new <see cref="DataTable" />.</returns>
        public DataTable ExecuteDataTable(string cmdText, Dictionary<string, object> parameters = null)
        {
            return ExecuteDataTable(cmdText, CommandType.Text, parameters);
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
        public virtual Dictionary<TDictionaryKey, TElement> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector)
        {
            using (var reader = ExecuteReader(cmdText, cmdType, parameters))
            {
                var dict = new Dictionary<TDictionaryKey, TElement>();

                while (reader.Read())
                {
                    dict.Add(keyProjector(reader.GetValue(0)), elementProjector(reader.GetValue(1)));
                }

                return dict;
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
        public Dictionary<TDictionaryKey, TElement> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector)
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
        public Dictionary<TDictionaryKey, TElement> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, Dictionary<string, object> parameters, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector)
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
        public Dictionary<TDictionaryKey, TElement> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector)
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
                var ownsConnection = command.Transaction == null;

                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync(cancellationToken);

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
            var ownsConnection = command.Transaction == null;

            if (connection.State == ConnectionState.Closed)
                await connection.OpenAsync(cancellationToken);

            return await command.ExecuteReaderAsync(ownsConnection ? CommandBehavior.CloseConnection : CommandBehavior.Default, cancellationToken);
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
                var ownsConnection = command.Transaction == null;

                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync(cancellationToken);

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
        public virtual async Task<T> ExecuteObjectAsync<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var reader = await ExecuteReaderAsync(cmdText, cmdType, parameters, cancellationToken))
            {
                return reader.Read() ? projector(reader) : default(T);
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
        public Task<T> ExecuteObjectAsync<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, CancellationToken cancellationToken = new CancellationToken()) where T : class
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
        public Task<T> ExecuteObjectAsync<T>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<T> ExecuteObjectAsync<T>(string cmdText, Dictionary<string, object> parameters, CancellationToken cancellationToken = new CancellationToken()) where T : class
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
        public Task<T> ExecuteObjectAsync<T>(string cmdText, CommandType cmdType, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<T> ExecuteObjectAsync<T>(string cmdText, CommandType cmdType, CancellationToken cancellationToken = new CancellationToken()) where T : class
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
        public Task<T> ExecuteObjectAsync<T>(string cmdText, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<T> ExecuteObjectAsync<T>(string cmdText, CancellationToken cancellationToken = new CancellationToken()) where T : class
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
        public virtual async Task<IEnumerable<T>> ExecuteListAsync<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var reader = await ExecuteReaderAsync(cmdText, cmdType, parameters, cancellationToken))
            {
                var list = new List<T>();

                while (reader.Read())
                {
                    list.Add(projector(reader));
                }

                return list;
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
        public Task<IEnumerable<T>> ExecuteListAsync<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, CancellationToken cancellationToken = new CancellationToken()) where T : class
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
        public Task<IEnumerable<T>> ExecuteListAsync<T>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<IEnumerable<T>> ExecuteListAsync<T>(string cmdText, Dictionary<string, object> parameters, CancellationToken cancellationToken = new CancellationToken()) where T : class
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
        public Task<IEnumerable<T>> ExecuteListAsync<T>(string cmdText, CommandType cmdType, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<IEnumerable<T>> ExecuteListAsync<T>(string cmdText, CommandType cmdType, CancellationToken cancellationToken = new CancellationToken()) where T : class
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
        public Task<IEnumerable<T>> ExecuteListAsync<T>(string cmdText, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<IEnumerable<T>> ExecuteListAsync<T>(string cmdText, CancellationToken cancellationToken = new CancellationToken()) where T : class
        {
            return ExecuteListAsync<T>(cmdText, Mapper<T>.Map, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a new <see cref="DataTable " />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="DataTable" />.</returns>
        public virtual async Task<DataTable> ExecuteDataTableAsync(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var reader = await ExecuteReaderAsync(cmdText, cmdType, parameters, cancellationToken))
            {
                var dt = new DataTable();

                dt.Load(reader);

                return dt;
            }
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a new <see cref="DataTable " />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="DataTable" />.</returns>
        public Task<DataTable> ExecuteDataTableAsync(string cmdText, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDataTableAsync(cmdText, CommandType.Text, parameters, cancellationToken);
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
        public virtual async Task<Dictionary<TDictionaryKey, TElement>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var reader = await ExecuteReaderAsync(cmdText, cmdType, parameters, cancellationToken))
            {
                var dict = new Dictionary<TDictionaryKey, TElement>();

                while (reader.Read())
                {
                    dict.Add(keyProjector(reader.GetValue(0)), elementProjector(reader.GetValue(1)));
                }

                return dict;
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
        public Task<Dictionary<TDictionaryKey, TElement>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<Dictionary<TDictionaryKey, TElement>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, Dictionary<string, object> parameters, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<Dictionary<TDictionaryKey, TElement>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, Func<object, TDictionaryKey> keyProjector, Func<object, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<Dictionary<TDictionaryKey, TElement>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<Dictionary<TDictionaryKey, TElement>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
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

            _connection = _factory.CreateConnection();

            _connection.ConnectionString = _connectionString;

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

        private void PrepareQuery<T>(IQueryOptions<T> options, out Mapper mapper) where T : class
        {
            var parameters = new Dictionary<string, object>();
            var sb = new StringBuilder();
            var mainTableType = typeof(T);
            var mainTableName = ConventionHelper.GetTableName(mainTableType);
            var m = new Mapper(mainTableType);
            var mainTableAlias = m.GenerateTableAlias(mainTableType);
            var mainTableProperties = mainTableType.GetRuntimeProperties().ToList();
            var mainTablePrimaryKeyPropertyInfo = ConventionHelper.GetPrimaryKeyPropertyInfos<T>().First();
            var mainTablePrimaryKeyName = ConventionHelper.GetColumnName(mainTablePrimaryKeyPropertyInfo);
            var fetchStrategy = options?.FetchStrategy;

            // Default select
            var columns = string.Join(",\n\t", m.SqlPropertiesMapping.Select(x =>
            {
                var colAlias = m.GenerateColumnAlias(x.Value);
                var colName = ConventionHelper.GetColumnName(x.Value);

                return $"[{mainTableAlias}].[{colName}] AS [{colAlias}]";
            }));

            // Check to see if we can automatically include some navigation properties (this seems to be the behavior of entity framework as well).
            // Only supports a one to one table join for now...
            if (fetchStrategy == null || !fetchStrategy.IncludePaths.Any())
            {
                // Assumes we want to perform a join when the navigation property from the primary table has also a navigation property of
                // the same type as the primary table
                // Only do a join when the primary table has a foreign key property for the join table
                var paths = mainTableProperties
                    .Where(x => ConventionHelper.IsComplex(x) && ConventionHelper.GetPrimaryKeyPropertyInfos(x.PropertyType).Any())
                    .Select(x => x.Name)
                    .ToList();

                if (paths.Count > 0)
                {
                    if (fetchStrategy == null)
                        fetchStrategy = new FetchStrategy<T>();

                    foreach (var path in paths)
                    {
                        fetchStrategy.Include(path);
                    }
                }
            }

            // -----------------------------------------------------------------------------------------------------------
            // Select clause
            // -----------------------------------------------------------------------------------------------------------

            // Append join tables from fetchStrategy
            // Only supports a one to one table join for now...
            if (fetchStrategy != null && fetchStrategy.IncludePaths.Any())
            {
                var joinStatementSb = new StringBuilder();

                sb.Append($"SELECT\n\t{columns}");

                foreach (var path in fetchStrategy.IncludePaths)
                {
                    var joinTablePropertyInfo = mainTableProperties.Single(x => x.Name.Equals(path));
                    var joinTableType = joinTablePropertyInfo.PropertyType;
                    var joinTableForeignKeyPropertyInfo = ConventionHelper.GetForeignKeyPropertyInfo(joinTableType, mainTableType);

                    // Only do a join when the primary table has a foreign key property for the join table
                    if (joinTableForeignKeyPropertyInfo != null)
                    {
                        var joinTableForeignKeyName = ConventionHelper.GetColumnName(joinTableForeignKeyPropertyInfo);
                        var joinTableProperties = joinTableType.GetRuntimeProperties().ToList();
                        var joinTableName = ConventionHelper.GetTableName(joinTableType);
                        var joinTableAlias = m.GenerateTableAlias(joinTableType);
                        var joinTableColumnNames = string.Join(",\n\t",
                            joinTableProperties
                                .Where(ConventionHelper.IsPrimitive)
                                .Select(x =>
                                {
                                    var colAlias = m.GenerateColumnAlias(x);
                                    var colName = ConventionHelper.GetColumnName(x);

                                    return $"[{joinTableAlias}].[{colName}] AS [{colAlias}]";
                                }));


                        sb.Append(",\n\t");
                        sb.Append(joinTableColumnNames);

                        joinStatementSb.Append("\n");
                        joinStatementSb.Append($"LEFT OUTER JOIN [{joinTableName}] AS [{joinTableAlias}] ON [{mainTableAlias}].[{mainTablePrimaryKeyName}] = [{joinTableAlias}].[{joinTableForeignKeyName}]");

                        m.SqlNavigationPropertiesMapping.Add(joinTableType, joinTableProperties.ToDictionary(ConventionHelper.GetColumnName, x => x));
                    }
                }

                sb.Append($"\nFROM [{mainTableName}] AS [{mainTableAlias}]");
                sb.Append(joinStatementSb);
            }
            else
            {
                sb.Append($"SELECT\n\t{columns}\nFROM [{mainTableName}] AS [{mainTableAlias}]");
            }

            if (options != null)
            {
                // -----------------------------------------------------------------------------------------------------------
                // Where clause
                // -----------------------------------------------------------------------------------------------------------

                if (options.Specification != null)
                {
                    new ExpressionTranslator().Translate(
                        options.Specification.Predicate,
                        m,
                        out string expSql,
                        out Dictionary<string, object> expParameters);

                    sb.Append("\nWHERE ");
                    sb.Append(expSql);

                    foreach (var item in expParameters)
                    {
                        parameters.Add(item.Key, item.Value);
                    }
                }

                // -----------------------------------------------------------------------------------------------------------
                // Sorting clause
                // -----------------------------------------------------------------------------------------------------------

                var sortings = options.SortingPropertiesMapping.ToDictionary(x => x.Key, x => x.Value);

                if (!sortings.Any())
                {
                    // Sorts on the Id key by default if no sorting is provided
                    foreach (var primaryKeyPropertyInfo in ConventionHelper.GetPrimaryKeyPropertyInfos<T>())
                    {
                        sortings.Add(primaryKeyPropertyInfo.Name, SortOrder.Ascending);
                    }
                }

                sb.Append("\n");
                sb.Append("ORDER BY ");

                foreach (var sorting in sortings)
                {
                    var sortingOrder = sorting.Value;
                    var sortingProperty = sorting.Key;
                    var lambda = ExpressionHelper.GetExpression<T>(sortingProperty);
                    var tableType = ExpressionHelper.GetMemberExpression(lambda).Expression.Type;
                    var tableName = m.GetTableName(tableType);
                    var tableAlias = m.GetTableAlias(tableName);
                    var sortingPropertyInfo = ExpressionHelper.GetPropertyInfo(lambda);
                    var columnAlias = m.GetColumnAlias(sortingPropertyInfo);

                    sb.Append($"[{tableAlias}].[{columnAlias}] {(sortingOrder == SortOrder.Descending ? "DESC" : "ASC")}, ");
                }

                sb.Remove(sb.Length - 2, 2);

                // -----------------------------------------------------------------------------------------------------------
                // Paging clause
                // -----------------------------------------------------------------------------------------------------------

                if (options.PageSize != -1)
                {
                    sb.Append("\n");
                    sb.Append($"OFFSET {options.PageSize} * ({options.PageIndex} - 1) ROWS");
                    sb.Append("\n");
                    sb.Append($"FETCH NEXT {options.PageSize} ROWS ONLY");
                }
            }

            // Setup mapper object
            m.Sql = sb.ToString();
            m.Parameters = parameters;

            mapper = m;
        }

        private void PrepareEntitySetQuery(EntitySet entitySet, bool existInDb, bool isIdentity, PropertyInfo primeryKeyPropertyInfo, out string sql, out Dictionary<string, object> parameters)
        {
            var entityType = entitySet.Entity.GetType();
            var tableName = ConventionHelper.GetTableName(entityType);
            var primeryKeyColumnName = ConventionHelper.GetColumnName(primeryKeyPropertyInfo);

            if (!_sqlPropertiesMapping.ContainsKey(entityType))
            {
                var dict = entityType
                    .GetRuntimeProperties()
                    .Where(x => ConventionHelper.IsPrimitive(x) && ConventionHelper.IsMapped(x))
                    .OrderBy(ConventionHelper.GetColumnOrder)
                    .ToDictionary(ConventionHelper.GetColumnName, x => x);

                _sqlPropertiesMapping[entityType] = new ConcurrentDictionary<string, PropertyInfo>(dict);
            }

            var sqlPropertiesMapping = _sqlPropertiesMapping[entityType];
            var properties = (isIdentity ? sqlPropertiesMapping.Where(x => !x.Key.Equals(primeryKeyColumnName)) : sqlPropertiesMapping).ToList();

            sql = string.Empty;
            parameters = new Dictionary<string, object>();

            switch (entitySet.State)
            {
                case EntityState.Added:
                    {
                        if (existInDb)
                            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityAlreadyBeingTrackedInStore, entitySet.Entity.GetType()));

                        var columnNames = string.Join(", ", properties.Select(x => ConventionHelper.GetColumnName(x.Value))).TrimEnd();
                        var values = string.Join(", ", properties.Select(x => $"@{ConventionHelper.GetColumnName(x.Value)}")).TrimEnd();

                        sql = $"INSERT INTO [{tableName}] ({columnNames})\nVALUES ({values})";

                        foreach (var pi in properties)
                        {
                            parameters.Add($"@{ConventionHelper.GetColumnName(pi.Value)}", pi.Value.GetValue(entitySet.Entity, null));
                        }

                        if (isIdentity)
                            parameters.Add($"@{primeryKeyColumnName}", primeryKeyPropertyInfo.GetValue(entitySet.Entity, null));

                        break;
                    }
                case EntityState.Removed:
                    {
                        if (!existInDb)
                            throw new InvalidOperationException(Resources.EntityNotFoundInStore);

                        sql = $"DELETE FROM [{tableName}]\nWHERE {primeryKeyColumnName} = @{primeryKeyColumnName}";

                        parameters.Add($"@{primeryKeyColumnName}", primeryKeyPropertyInfo.GetValue(entitySet.Entity, null));

                        break;
                    }
                case EntityState.Modified:
                    {
                        if (!existInDb)
                            throw new InvalidOperationException(Resources.EntityNotFoundInStore);

                        var values = string.Join(",\n\t", properties.Select(x =>
                        {
                            var columnName = ConventionHelper.GetColumnName(x.Value);
                            return columnName + " = " + $"@{columnName}";
                        }));

                        sql = $"UPDATE [{tableName}]\nSET {values}\nWHERE {primeryKeyColumnName} = @{primeryKeyColumnName}";

                        foreach (var pi in properties)
                        {
                            parameters.Add($"@{ConventionHelper.GetColumnName(pi.Value)}", pi.Value.GetValue(entitySet.Entity, null));
                        }

                        if (isIdentity)
                            parameters.Add($"@{primeryKeyColumnName}", primeryKeyPropertyInfo.GetValue(entitySet.Entity, null));

                        break;
                    }
            }
        }

        private void ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(object[] keyValues) where TEntity : class
        {
            if (keyValues.Length != ConventionHelper.GetPrimaryKeyPropertyInfos<TEntity>().Count())
                throw new ArgumentException(DotNetToolkit.Repository.Properties.Resources.EntityPrimaryKeyValuesLengthMismatch, nameof(keyValues));
        }

        #endregion

        #region Implementation of IContextAsync

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
                var ownsConnection = command.Transaction == null;

                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync(cancellationToken);

                foreach (var entitySet in _items)
                {
                    var entityType = entitySet.Entity.GetType();
                    var hasCompositePrimaryKey = ConventionHelper.HasCompositePrimaryKey(entityType);
                    var primeryKeyPropertyInfo = ConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();
                    var isIdentity = !hasCompositePrimaryKey && ConventionHelper.IsIdentity(primeryKeyPropertyInfo);

                    // Checks if the entity exist in the database
                    var existInDb = await command.ExecuteObjectExistAsync(entitySet.Entity, cancellationToken);

                    // Prepare the sql statement
                    PrepareEntitySetQuery(
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

                    rows += await command.ExecuteNonQueryAsync(cancellationToken);

                    // Checks to see if the model needs to be updated with the new key returned from the database
                    if (entitySet.State == EntityState.Added && isIdentity)
                    {
                        command.CommandText = "SELECT @@IDENTITY";
                        command.Parameters.Clear();

                        var newKey = await command.ExecuteScalarAsync(cancellationToken);
                        var convertedKeyValue = Convert.ChangeType(newKey, primeryKeyPropertyInfo.PropertyType);

                        primeryKeyPropertyInfo.SetValue(entitySet.Entity, convertedKeyValue, null);
                    }
                }

                if (ownsConnection)
                    connection.Dispose();

                // Clears the collection
                while (_items.Count > 0)
                {
                    _items.TryTake(out _);
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
        public Task<TEntity> FindAsync<TEntity>(CancellationToken cancellationToken, IFetchStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(keyValues);

            var options = new QueryOptions<TEntity>().SatisfyBy(ConventionHelper.GetByPrimaryKeySpecification<TEntity>(keyValues));

            if (fetchStrategy != null)
                options.Fetch(fetchStrategy);

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
        public Task<TResult> FindAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var selectorFunc = selector.Compile();

            PrepareQuery(options, out Mapper mapper);

            return ExecuteObjectAsync<TResult>(mapper.Sql, mapper.Parameters, reader => mapper.Map<TEntity, TResult>(reader, selectorFunc), cancellationToken);
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
        public Task<IEnumerable<TResult>> FindAllAsync<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var selectorFunc = selector.Compile();

            PrepareQuery(options, out Mapper mapper);

            return ExecuteListAsync<TResult>(mapper.Sql, mapper.Parameters, reader => mapper.Map<TEntity, TResult>(reader, selectorFunc), cancellationToken);
        }

        /// <summary>
        /// Asynchronously returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public async Task<int> CountAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            PrepareQuery(options, out Mapper mapper);

            using (var reader = await ExecuteReaderAsync(mapper.Sql, mapper.Parameters, cancellationToken))
            {
                var count = 0;

                while (reader.Read())
                {
                    count++;
                }

                return count;
            }
        }

        /// <summary>
        /// Asynchronously determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public async Task<bool> ExistsAsync<TEntity>(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            PrepareQuery(options, out Mapper mapper);

            using (var reader = await ExecuteReaderAsync(mapper.Sql, mapper.Parameters, cancellationToken))
            {
                var hasRows = false;

                while (reader.Read())
                {
                    hasRows = true;

                    break;
                }

                return hasRows;
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
        public async Task<Dictionary<TDictionaryKey, TElement>> ToDictionaryAsync<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            PrepareQuery(options, out Mapper mapper);

            using (var reader = await ExecuteReaderAsync(mapper.Sql, mapper.Parameters, cancellationToken))
            {
                var dict = new Dictionary<TDictionaryKey, TElement>();

                while (reader.Read())
                {
                    dict.Add(mapper.Map<TEntity, TDictionaryKey>(reader, keySelectFunc), mapper.Map<TEntity, TElement>(reader, elementSelectorFunc));
                }

                return dict;
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
        public async Task<IEnumerable<TResult>> GroupByAsync<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector, CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            var keySelectFunc = keySelector.Compile();
            var resultSelectorFunc = resultSelector.Compile();

            return (await FindAllAsync<TEntity, TEntity>(options, IdentityExpression<TEntity>.Instance, cancellationToken))
                .GroupBy(keySelectFunc, resultSelectorFunc)
                .ToList();
        }

        #endregion

        #region Implementation of IContext

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns>The transaction.</returns>
        public ITransactionManager BeginTransaction()
        {
            var connection = CreateConnection();

            connection.Open();

            _transaction = connection.BeginTransaction();

            return new AdoNetTransactionManager(_transaction);
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
                var ownsConnection = command.Transaction == null;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                foreach (var entitySet in _items)
                {
                    var entityType = entitySet.Entity.GetType();
                    var hasCompositePrimaryKey = ConventionHelper.HasCompositePrimaryKey(entityType);
                    var primeryKeyPropertyInfo = ConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();
                    var isIdentity = !hasCompositePrimaryKey && ConventionHelper.IsIdentity(primeryKeyPropertyInfo);

                    // Checks if the entity exist in the database
                    var existInDb = command.ExecuteObjectExist(entitySet.Entity);

                    // Prepare the sql statement
                    PrepareEntitySetQuery(
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

                    // Checks to see if the model needs to be updated with the new key returned from the database
                    if (entitySet.State == EntityState.Added && isIdentity)
                    {
                        command.CommandText = "SELECT @@IDENTITY";
                        command.Parameters.Clear();

                        var newKey = command.ExecuteScalar();
                        var convertedKeyValue = Convert.ChangeType(newKey, primeryKeyPropertyInfo.PropertyType);

                        primeryKeyPropertyInfo.SetValue(entitySet.Entity, convertedKeyValue, null);
                    }
                }

                if (ownsConnection)
                    connection.Dispose();

                // Clears the collection
                while (_items.Count > 0)
                {
                    _items.TryTake(out _);
                }

                return rows;
            }
        }

        /// <summary>
        /// Returns the entity <see cref="T:System.Linq.IQueryable`1" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <returns>The entity <see cref="T:System.Linq.IQueryable`1" />.</returns>
        public IQueryable<TEntity> AsQueryable<TEntity>() where TEntity : class
        {
            return AsQueryable<TEntity>((IFetchStrategy<TEntity>)null);
        }

        /// <summary>
        /// Returns the entity <see cref="T:System.Linq.IQueryable`1" /> using the specified fetching strategy.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="fetchStrategy"></param>
        /// <returns>The entity <see cref="T:System.Linq.IQueryable`1" />.</returns>
        public IQueryable<TEntity> AsQueryable<TEntity>(IFetchStrategy<TEntity> fetchStrategy) where TEntity : class
        {
            var options = new QueryOptions<TEntity>();

            if (fetchStrategy != null)
                options.Fetch(fetchStrategy);

            return FindAll<TEntity, TEntity>(options, IdentityExpression<TEntity>.Instance).AsQueryable();
        }

        /// <summary>
        /// Finds an entity with the given primary key values in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found in the repository.</returns>
        public TEntity Find<TEntity>(IFetchStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(keyValues);

            var options = new QueryOptions<TEntity>().SatisfyBy(ConventionHelper.GetByPrimaryKeySpecification<TEntity>(keyValues));

            if (fetchStrategy != null)
                options.Fetch(fetchStrategy);

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
        public TResult Find<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var selectorFunc = selector.Compile();

            PrepareQuery(options, out Mapper mapper);

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
        public IEnumerable<TResult> FindAll<TEntity, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector) where TEntity : class
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            var selectorFunc = selector.Compile();

            PrepareQuery(options, out Mapper mapper);

            return ExecuteList<TResult>(mapper.Sql, mapper.Parameters, reader => mapper.Map<TEntity, TResult>(reader, selectorFunc));
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public int Count<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            PrepareQuery(options, out Mapper mapper);

            using (var reader = ExecuteReader(mapper.Sql, mapper.Parameters))
            {
                var count = 0;

                while (reader.Read())
                {
                    count++;
                }

                return count;
            }
        }

        /// <summary>
        /// Determines whether the repository contains an entity that match the conditions defined by the specified by the <paramref name="options" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns><c>true</c> if the repository contains one or more elements that match the conditions defined by the specified criteria; otherwise, <c>false</c>.</returns>
        public bool Exists<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            PrepareQuery(options, out Mapper mapper);

            using (var reader = ExecuteReader(mapper.Sql, mapper.Parameters))
            {
                var hasRows = false;

                while (reader.Read())
                {
                    hasRows = true;

                    break;
                }

                return hasRows;
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
        public Dictionary<TDictionaryKey, TElement> ToDictionary<TEntity, TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            var keySelectFunc = keySelector.Compile();
            var elementSelectorFunc = elementSelector.Compile();

            PrepareQuery(options, out Mapper mapper);

            using (var reader = ExecuteReader(mapper.Sql, mapper.Parameters))
            {
                var dict = new Dictionary<TDictionaryKey, TElement>();

                while (reader.Read())
                {
                    dict.Add(mapper.Map<TEntity, TDictionaryKey>(reader, keySelectFunc), mapper.Map<TEntity, TElement>(reader, elementSelectorFunc));
                }

                return dict;
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
        public IEnumerable<TResult> GroupBy<TEntity, TGroupKey, TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TGroupKey, IEnumerable<TEntity>, TResult>> resultSelector) where TEntity : class
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (resultSelector == null)
                throw new ArgumentNullException(nameof(resultSelector));

            var keySelectFunc = keySelector.Compile();
            var resultSelectorFunc = resultSelector.Compile();

            return FindAll<TEntity, TEntity>(options, IdentityExpression<TEntity>.Instance)
                .GroupBy(keySelectFunc, resultSelectorFunc)
                .ToList();
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }

            _transaction = null;

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Nested type: EntitySet

        /// <summary>
        /// Represents an internal entity set in the in-memory store, which holds the entity and it's state representing the operation that was performed at the time.
        /// </summary>
        private class EntitySet
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="EntitySet" /> class.
            /// </summary>
            /// <param name="entity">The entity.</param>
            /// <param name="state">The state.</param>
            public EntitySet(object entity, EntityState state)
            {
                Entity = entity;
                State = state;
            }

            #endregion

            #region Properties

            /// <summary>
            /// Gets the entity.
            /// </summary>
            public object Entity { get; }

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

        #region Implementation of IHaveRepositoryContextConfiguration

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        void IHaveRepositoryContextConfiguration.Initialize<TEntity>()
        {
            var schemaHelper = new SchemaTableHelper(this);

            schemaHelper.Validate<TEntity>();
        }

        #endregion
    }
}
