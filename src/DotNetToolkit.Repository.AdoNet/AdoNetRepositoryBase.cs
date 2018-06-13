namespace DotNetToolkit.Repository.AdoNet
{
    using FetchStrategies;
    using Helpers;
    using Interceptors;
    using Internal;
    using Properties;
    using Queries;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a repository for ado.net.
    /// </summary>
    public abstract class AdoNetRepositoryBase<TEntity, TKey> : RepositoryBaseAsync<TEntity, TKey> where TEntity : class
    {
        #region Fields

        private bool _disposed;

        private BlockingCollection<EntitySet> _items = new BlockingCollection<EntitySet>();

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

        /// <summary>
        /// Gets a value indicating whether this instance is identity.
        /// </summary>
        protected bool IsIdentity { get; private set; }

        /// <summary>
        /// Gets the name of the identity property.
        /// </summary>
        protected PropertyInfo SqlIdentityPropertyInfo { get; private set; }

        /// <summary>
        /// Gets the SQL properties.
        /// </summary>
        protected Dictionary<string, PropertyInfo> SqlPropertiesMapping { get; private set; }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        protected string TableName { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected AdoNetRepositoryBase(string connectionString, IEnumerable<IRepositoryInterceptor> interceptors = null) : base(interceptors)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmptyString, nameof(connectionString));

            var ccs = ConfigurationManager.ConnectionStrings[connectionString];
            if (ccs == null)
                throw new ArgumentException(Resources.ConnectionStringDoestNotExistInConfigFile);

            Factory = Internal.DbProviderFactories.GetFactory(ccs.ProviderName);
            ConnectionString = connectionString;

            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetRepositoryBase{TEntity, TKey}"/> class.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="interceptors">The interceptors.</param>
        protected AdoNetRepositoryBase(string providerName, string connectionString, IEnumerable<IRepositoryInterceptor> interceptors = null) : base(interceptors)
        {
            if (string.IsNullOrEmpty(providerName))
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmptyString, nameof(providerName));

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException(Resources.ArgumentCannotBeNullOrEmptyString, nameof(connectionString));

            Factory = Internal.DbProviderFactories.GetFactory(providerName);
            ConnectionString = connectionString;

            Initialize();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Executes a SQL statement against a connection.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>The number of rows affected.</returns>
        public int ExecuteNonQuery(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
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
        public int ExecuteNonQuery(string cmdText, Dictionary<string, object> parameters = null)
        {
            return ExecuteNonQuery(cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Sends the <see cref="System.Data.SqlClient.SqlCommand.CommandText" /> to the <see cref="System.Data.SqlClient.SqlCommand.Connection" /> and builds a <see cref="System.Data.SqlClient.SqlDataReader "/>.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>A <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        public DbDataReader ExecuteReader(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            return ExecuteReader(CreateConnection(), cmdText, cmdType, parameters);
        }

        /// <summary>
        /// Sends the <see cref="System.Data.SqlClient.SqlCommand.CommandText" /> to the <see cref="System.Data.SqlClient.SqlCommand.Connection" /> and builds a <see cref="System.Data.SqlClient.SqlDataReader "/>.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
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
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>The first column of the first row in the result set returned by the query.</returns>
        public T ExecuteScalar<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            using (var connection = CreateConnection())
            {
                return ExecuteScalar<T>(connection, cmdText, cmdType, parameters);
            }
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
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
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public T ExecuteObject<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
        {
            return ExecuteObject<T>(CreateConnection(), cmdText, cmdType, parameters, projector);
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public T ExecuteObject<T>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
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
            return ExecuteObject<T>(cmdText, cmdType, null, projector);
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
        public IEnumerable<T> ExecuteList<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
        {
            return ExecuteList<T>(CreateConnection(), cmdText, cmdType, parameters, projector);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
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

        /// <summary>
        /// Executes the query, and returns a new <see cref="DataTable " />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>A new <see cref="DataTable" />.</returns>
        public DataTable ExecuteDataTable(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            using (var connection = CreateConnection())
            {
                return ExecuteDataTable(connection, cmdText, cmdType, parameters);
            }
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="DataTable " />.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
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
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Dictionary<TDictionaryKey, TElement> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, TDictionaryKey> keyProjector, Func<DbDataReader, TElement> elementProjector)
        {
            return ExecuteDictionary<TDictionaryKey, TElement>(CreateConnection(), cmdText, cmdType, parameters, keyProjector, elementProjector);
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Dictionary<TDictionaryKey, TElement> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, TDictionaryKey> keyProjector, Func<DbDataReader, TElement> elementProjector)
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
        public Dictionary<TDictionaryKey, TElement> ExecuteDictionary<TDictionaryKey, TElement>(string cmdText, Func<DbDataReader, TDictionaryKey> keyProjector, Func<DbDataReader, TElement> elementProjector)
        {
            return ExecuteDictionary<TDictionaryKey, TElement>(cmdText, null, keyProjector, elementProjector);
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="IGrouping{TGroupKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="IGrouping{TGroupKey, TElement}" /> that contains keys and values.</returns>
        public IEnumerable<IGrouping<TGroupKey, TElement>> ExecuteGroup<TGroupKey, TElement>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, TGroupKey> keyProjector, Func<DbDataReader, TElement> elementProjector)
        {
            return ExecuteGroup<TGroupKey, TElement>(CreateConnection(), cmdText, cmdType, parameters, keyProjector, elementProjector);
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="IGrouping{TGroupKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="IGrouping{TGroupKey, TElement}" /> that contains keys and values.</returns>
        public IEnumerable<IGrouping<TGroupKey, TElement>> ExecuteGroup<TGroupKey, TElement>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, TGroupKey> keyProjector, Func<DbDataReader, TElement> elementProjector)
        {
            return ExecuteGroup<TGroupKey, TElement>(cmdText, CommandType.Text, parameters, keyProjector, elementProjector);
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="IGrouping{TGroupKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="IGrouping{TGroupKey, TElement}" /> that contains keys and values.</returns>
        public IEnumerable<IGrouping<TGroupKey, TElement>> ExecuteGroup<TGroupKey, TElement>(string cmdText, Func<DbDataReader, TGroupKey> keyProjector, Func<DbDataReader, TElement> elementProjector)
        {
            return ExecuteGroup<TGroupKey, TElement>(cmdText, null, keyProjector, elementProjector);
        }

        /// <summary>
        /// Asynchronously executes a SQL statement against a connection.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        public Task<int> ExecuteNonQueryAsync(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var connection = CreateConnection())
            {
                return ExecuteNonQueryAsync(connection, cmdText, cmdType, parameters, cancellationToken);
            }
        }

        /// <summary>
        /// Asynchronously executes a SQL statement against a connection.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        public Task<int> ExecuteNonQueryAsync(string cmdText, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteNonQueryAsync(cmdText, CommandType.Text, parameters, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends the <see cref="System.Data.SqlClient.SqlCommand.CommandText" /> to the <see cref="System.Data.SqlClient.SqlCommand.Connection" /> and builds a <see cref="System.Data.SqlClient.SqlDataReader "/>.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        public Task<DbDataReader> ExecuteReaderAsync(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteReaderAsync(CreateConnection(), cmdText, cmdType, parameters, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends the <see cref="System.Data.SqlClient.SqlCommand.CommandText" /> to the <see cref="System.Data.SqlClient.SqlCommand.Connection" /> and builds a <see cref="System.Data.SqlClient.SqlDataReader "/>.
        /// </summary>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
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
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the first column of the first row in the result set returned by the query.</returns>
        public Task<T> ExecuteScalarAsync<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var connection = CreateConnection())
            {
                return ExecuteScalarAsync<T>(connection, cmdText, cmdType, parameters, cancellationToken);
            }
        }

        /// <summary>
        /// Asynchronously executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
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
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        public Task<T> ExecuteObjectAsync<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteObjectAsync<T>(CreateConnection(), cmdText, cmdType, parameters, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        public Task<T> ExecuteObjectAsync<T>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteObjectAsync<T>(cmdText, CommandType.Text, parameters, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        public Task<T> ExecuteObjectAsync<T>(string cmdText, CommandType cmdType, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteObjectAsync<T>(cmdText, cmdType, null, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        public Task<T> ExecuteObjectAsync<T>(string cmdText, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteObjectAsync<T>(cmdText, CommandType.Text, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        public Task<IEnumerable<T>> ExecuteListAsync<T>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteListAsync<T>(CreateConnection(), cmdText, cmdType, parameters, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
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
        /// <param name="cmdType">The command type</param>
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
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        public Task<IEnumerable<T>> ExecuteListAsync<T>(string cmdText, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteListAsync<T>(cmdText, CommandType.Text, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Task<Dictionary<TDictionaryKey, TElement>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, TDictionaryKey> keyProjector, Func<DbDataReader, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDictionaryAsync<TDictionaryKey, TElement>(CreateConnection(), cmdText, cmdType, parameters, keyProjector, elementProjector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        public Task<Dictionary<TDictionaryKey, TElement>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, TDictionaryKey> keyProjector, Func<DbDataReader, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
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
        public Task<Dictionary<TDictionaryKey, TElement>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(string cmdText, Func<DbDataReader, TDictionaryKey> keyProjector, Func<DbDataReader, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteDictionaryAsync<TDictionaryKey, TElement>(cmdText, null, keyProjector, elementProjector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a new <see cref="IGrouping{TGroupKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IGrouping{TGroupKey, TElement}" /> that contains keys and values.</returns>
        public Task<IEnumerable<IGrouping<TGroupKey, TElement>>> ExecuteGroupAsync<TGroupKey, TElement>(string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, TGroupKey> keyProjector, Func<DbDataReader, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteGroupAsync<TGroupKey, TElement>(CreateConnection(), cmdText, cmdType, parameters, keyProjector, elementProjector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a new <see cref="IGrouping{TGroupKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IGrouping{TGroupKey, TElement}" /> that contains keys and values.</returns>
        public Task<IEnumerable<IGrouping<TGroupKey, TElement>>> ExecuteGroupAsync<TGroupKey, TElement>(string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, TGroupKey> keyProjector, Func<DbDataReader, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteGroupAsync<TGroupKey, TElement>(cmdText, CommandType.Text, parameters, keyProjector, elementProjector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a new <see cref="IGrouping{TGroupKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="cmdText">The command text.</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IGrouping{TGroupKey, TElement}" /> that contains keys and values.</returns>
        public Task<IEnumerable<IGrouping<TGroupKey, TElement>>> ExecuteGroupAsync<TGroupKey, TElement>(string cmdText, Func<DbDataReader, TGroupKey> keyProjector, Func<DbDataReader, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteGroupAsync<TGroupKey, TElement>(cmdText, null, keyProjector, elementProjector, cancellationToken);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns>The connection.</returns>
        protected virtual DbConnection CreateConnection()
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
        protected virtual DbCommand CreateCommand(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            var command = Factory.CreateCommand();

            command.CommandText = cmdText;
            command.Connection = connection;
            command.CommandType = cmdType;

            if (parameters != null && parameters.Count > 0)
            {
                command.Parameters.Clear();

                foreach (var item in parameters)
                {
                    command.Parameters.Add(CreateParameter(item.Key, item.Value));
                }

            }

            return command;
        }

        /// <summary>
        /// Creates a parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns>The new parameter.</returns>
        protected virtual DbParameter CreateParameter(string name, object value)
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
        protected int ExecuteNonQuery(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
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
        protected int ExecuteNonQuery(DbConnection connection, string cmdText, Dictionary<string, object> parameters = null)
        {
            return ExecuteNonQuery(connection, cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Sends the <see cref="System.Data.SqlClient.SqlCommand.CommandText" /> to the <see cref="System.Data.SqlClient.SqlCommand.Connection" /> and builds a <see cref="System.Data.SqlClient.SqlDataReader "/>.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>A <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        protected virtual DbDataReader ExecuteReader(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
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
        protected DbDataReader ExecuteReader(DbConnection connection, string cmdText, Dictionary<string, object> parameters = null)
        {
            return ExecuteReader(connection, cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>The first column of the first row in the result set returned by the query.</returns>
        protected virtual T ExecuteScalar<T>(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
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
        /// <returns>The first column of the first row in the result set returned by the query.</returns>
        protected T ExecuteScalar<T>(DbConnection connection, string cmdText, Dictionary<string, object> parameters = null)
        {
            return ExecuteScalar<T>(connection, cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        protected virtual T ExecuteObject<T>(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
        {
            using (var reader = ExecuteReader(connection, cmdText, cmdType, parameters))
            {
                return reader.Read() ? projector(reader) : default(T);
            }
        }

        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        protected T ExecuteObject<T>(DbConnection connection, string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
        {
            return ExecuteObject<T>(connection, cmdText, CommandType.Text, parameters, projector);
        }


        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        protected T ExecuteObject<T>(DbConnection connection, string cmdText, CommandType cmdType, Func<DbDataReader, T> projector)
        {
            return ExecuteObject<T>(connection, cmdText, cmdType, null, projector);
        }


        /// <summary>
        /// Executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        protected T ExecuteObject<T>(DbConnection connection, string cmdText, Func<DbDataReader, T> projector)
        {
            return ExecuteObject<T>(connection, cmdText, CommandType.Text, projector);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        protected virtual IEnumerable<T> ExecuteList<T>(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
        {
            using (var reader = ExecuteReader(connection, cmdText, cmdType, parameters))
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
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        protected IEnumerable<T> ExecuteList<T>(DbConnection connection, string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector)
        {
            return ExecuteList<T>(connection, cmdText, CommandType.Text, parameters, projector);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        protected IEnumerable<T> ExecuteList<T>(DbConnection connection, string cmdText, CommandType cmdType, Func<DbDataReader, T> projector)
        {
            return ExecuteList<T>(connection, cmdText, cmdType, null, projector);
        }

        /// <summary>
        /// Executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        protected IEnumerable<T> ExecuteList<T>(DbConnection connection, string cmdText, Func<DbDataReader, T> projector)
        {
            return ExecuteList<T>(connection, cmdText, CommandType.Text, projector);
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="DataTable " />.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>A new <see cref="DataTable" />.</returns>
        protected virtual DataTable ExecuteDataTable(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null)
        {
            using (var adapter = Factory.CreateDataAdapter())
            {
                var command = CreateCommand(connection, cmdText, cmdType, parameters);

                adapter.SelectCommand = command;

                var table = new DataTable();

                adapter.Fill(table);

                return table;
            }
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="DataTable " />.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <returns>A new <see cref="DataTable" />.</returns>
        protected DataTable ExecuteDataTable(DbConnection connection, string cmdText, Dictionary<string, object> parameters = null)
        {
            return ExecuteDataTable(connection, cmdText, CommandType.Text, parameters);
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        protected virtual Dictionary<TDictionaryKey, TElement> ExecuteDictionary<TDictionaryKey, TElement>(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, TDictionaryKey> keyProjector, Func<DbDataReader, TElement> elementProjector)
        {
            using (var reader = ExecuteReader(connection, cmdText, cmdType, parameters))
            {
                var dict = new Dictionary<TDictionaryKey, TElement>();

                while (reader.Read())
                {
                    dict.Add(keyProjector(reader), elementProjector(reader));
                }

                return dict;
            }
        }

        /// <summary>
        /// Executes the query, and returns a new <see cref="IGrouping{TGroupKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <returns>A new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        protected virtual IEnumerable<IGrouping<TGroupKey, TElement>> ExecuteGroup<TGroupKey, TElement>(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, TGroupKey> keyProjector, Func<DbDataReader, TElement> elementProjector)
        {
            using (var reader = ExecuteReader(connection, cmdText, cmdType, parameters))
            {
                var dict = new Dictionary<TGroupKey, TElement>();

                while (reader.Read())
                {
                    dict.Add(keyProjector(reader), elementProjector(reader));
                }

                return dict.GroupBy(x => x.Key, x => x.Value);
            }
        }

        /// <summary>
        /// Asynchronously executes a SQL statement against a connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        protected virtual Task<int> ExecuteNonQueryAsync(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var command = CreateCommand(connection, cmdText, cmdType, parameters))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                return command.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Asynchronously executes a SQL statement against a connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        protected Task<int> ExecuteNonQueryAsync(DbConnection connection, string cmdText, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteNonQueryAsync(connection, cmdText, CommandType.Text, parameters, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends the <see cref="System.Data.SqlClient.SqlCommand.CommandText" /> to the <see cref="System.Data.SqlClient.SqlCommand.Connection" /> and builds a <see cref="System.Data.SqlClient.SqlDataReader "/>.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        protected virtual Task<DbDataReader> ExecuteReaderAsync(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            var command = CreateCommand(connection, cmdText, cmdType, parameters);

            return command.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken);
        }

        /// <summary>
        /// Asynchronously sends the <see cref="System.Data.SqlClient.SqlCommand.CommandText" /> to the <see cref="System.Data.SqlClient.SqlCommand.Connection" /> and builds a <see cref="System.Data.SqlClient.SqlDataReader "/>.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a <see cref="System.Data.SqlClient.SqlDataReader" /> object.</returns>
        protected Task<DbDataReader> ExecuteReaderAsync(DbConnection connection, string cmdText, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteReaderAsync(connection, cmdText, CommandType.Text, parameters, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the first column of the first row in the result set returned by the query.</returns>
        protected virtual async Task<T> ExecuteScalarAsync<T>(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var command = CreateCommand(connection, cmdText, cmdType, parameters))
            {
                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync(cancellationToken);

                var result = await command.ExecuteScalarAsync(cancellationToken);

                if (result == null || result is DBNull)
                    return default(T);

                if (result is T)
                    return (T)result;

                return (T)Convert.ChangeType(result, typeof(T));
            }
        }

        /// <summary>
        /// Asynchronously executes the query, and returns the first column of the first row in the result set returned by the query. Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the first column of the first row in the result set returned by the query.</returns>
        protected Task<T> ExecuteScalarAsync<T>(DbConnection connection, string cmdText, Dictionary<string, object> parameters = null, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteScalarAsync<T>(connection, cmdText, CommandType.Text, parameters, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        protected virtual async Task<T> ExecuteObjectAsync<T>(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var reader = await ExecuteReaderAsync(connection, cmdText, cmdType, parameters, cancellationToken))
            {
                return await reader.ReadAsync(cancellationToken) ? projector(reader) : default(T);
            }
        }

        /// <summary>
        /// Asynchronously executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        protected Task<T> ExecuteObjectAsync<T>(DbConnection connection, string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteObjectAsync<T>(connection, cmdText, CommandType.Text, parameters, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        protected Task<T> ExecuteObjectAsync<T>(DbConnection connection, string cmdText, CommandType cmdType, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteObjectAsync<T>(connection, cmdText, cmdType, null, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns an object which has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        protected Task<T> ExecuteObjectAsync<T>(DbConnection connection, string cmdText, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteObjectAsync<T>(connection, cmdText, CommandType.Text, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        protected virtual async Task<IEnumerable<T>> ExecuteListAsync<T>(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var reader = await ExecuteReaderAsync(connection, cmdText, cmdType, parameters, cancellationToken))
            {
                var list = new List<T>();

                while (await reader.ReadAsync(cancellationToken))
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
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        protected Task<IEnumerable<T>> ExecuteListAsync<T>(DbConnection connection, string cmdText, Dictionary<string, object> parameters, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteListAsync<T>(connection, cmdText, CommandType.Text, parameters, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        protected Task<IEnumerable<T>> ExecuteListAsync<T>(DbConnection connection, string cmdText, CommandType cmdType, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteListAsync<T>(connection, cmdText, cmdType, null, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a list which each entity has been projected into a new form.
        /// </summary>
        /// <typeparam name="T">The type of the result set returned by the query.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns>
        protected Task<IEnumerable<T>> ExecuteListAsync<T>(DbConnection connection, string cmdText, Func<DbDataReader, T> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            return ExecuteListAsync<T>(connection, cmdText, CommandType.Text, projector, cancellationToken);
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TDictionaryKey">The type of the dictionary key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="Dictionary{TDictionaryKey, TEntity}" /> that contains keys and values.</returns>
        protected virtual async Task<Dictionary<TDictionaryKey, TElement>> ExecuteDictionaryAsync<TDictionaryKey, TElement>(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, TDictionaryKey> keyProjector, Func<DbDataReader, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var reader = await ExecuteReaderAsync(connection, cmdText, cmdType, parameters, cancellationToken))
            {
                var dict = new Dictionary<TDictionaryKey, TElement>();

                while (await reader.ReadAsync(cancellationToken))
                {
                    dict.Add(keyProjector(reader), elementProjector(reader));
                }

                return dict;
            }
        }

        /// <summary>
        /// Asynchronously executes the query, and returns a new <see cref="IGrouping{TGroupKey, TElement}" /> according to the specified <paramref name="keyProjector" />, and <paramref name="elementProjector" />.
        /// </summary>
        /// <typeparam name="TGroupKey">The type of the group key.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by elementSelector.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="cmdText">The command text.</param>
        /// <param name="cmdType">The command type</param>
        /// <param name="parameters">The command parameters</param>
        /// <param name="keyProjector">A function to extract a key from each entity.</param>
        /// <param name="elementProjector">A transform function to produce a result element value from each element.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a new <see cref="IGrouping{TGroupKey, TEntity}" /> that contains keys and values.</returns>
        protected virtual async Task<IEnumerable<IGrouping<TGroupKey, TElement>>> ExecuteGroupAsync<TGroupKey, TElement>(DbConnection connection, string cmdText, CommandType cmdType, Dictionary<string, object> parameters, Func<DbDataReader, TGroupKey> keyProjector, Func<DbDataReader, TElement> elementProjector, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var reader = await ExecuteReaderAsync(connection, cmdText, cmdType, parameters, cancellationToken))
            {
                var dict = new Dictionary<TGroupKey, TElement>();

                while (await reader.ReadAsync(cancellationToken))
                {
                    dict.Add(keyProjector(reader), elementProjector(reader));
                }

                return dict.GroupBy(x => x.Key, x => x.Value);
            }
        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            SqlPropertiesMapping = typeof(TEntity)
                .GetRuntimeProperties()
                .Where(x => x.IsPrimitive() && x.IsMapped())
                .OrderBy(x => x.GetColumnOrder())
                .ToDictionary(ConventionHelper.GetColumnName, x => x);

            TableName = typeof(TEntity).GetTableName();

            DataRow autoIncrementDataRow = null;

            using (var reader = ExecuteReader($"SELECT * FROM {TableName}"))
            {
                var schema = reader.GetSchemaTable();

                for (var i = 0; i < schema.Rows.Count; i++)
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
                    var columnName = (string)autoIncrementDataRow["ColumnName"];

                    SqlIdentityPropertyInfo = SqlPropertiesMapping.FirstOrDefault(x => x.Key.Equals(columnName)).Value;

                    if (SqlIdentityPropertyInfo == null)
                        throw new InvalidOperationException(string.Format(Resources.InvalidColumnName, columnName));

                    IsIdentity = true;
                }
            }
        }

        private void PrepareSelectStatement(IFetchStrategy<TEntity> fetchStrategy, out DbSqlSelectStatementConfig config)
        {
            var sb = new StringBuilder();
            var cfg = new DbSqlSelectStatementConfig();

            var mainTableType = typeof(TEntity);
            var mainTableAlias = cfg.GenerateTableAlias(mainTableType);
            var mainTableProperties = mainTableType.GetRuntimeProperties();
            var mainTablePrimaryKeyName = typeof(TEntity).GetPrimaryKeyPropertyInfo().GetColumnName();

            // Default select
            var columns = string.Join(",\n\t",
                SqlPropertiesMapping.Select(x =>
                {
                    var colAlias = cfg.GenerateColumnAlias(x.Value);
                    var colName = x.Value.GetColumnName();

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
                    .Where(x => x.IsComplex() && !string.IsNullOrEmpty(x.PropertyType.GetPrimaryKeyPropertyInfo()?.GetColumnName()))
                    .Select(x => x.Name)
                    .ToList();

                if (paths.Count > 0)
                {
                    if (fetchStrategy == null)
                        fetchStrategy = new FetchStrategy<TEntity>();

                    foreach (var path in paths)
                    {
                        fetchStrategy.Include(path);
                    }
                }
            }

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
                    var joinTableForeignKeyName = joinTableType.GetForeignKeyPropertyInfo(mainTableType)?.GetColumnName();

                    // Only do a join when the primary table has a foreign key property for the join table
                    if (!string.IsNullOrEmpty(joinTableForeignKeyName))
                    {
                        var joinTableProperties = joinTableType.GetRuntimeProperties();
                        var joinTableName = joinTableType.GetTableName();
                        var joinTableAlias = cfg.GenerateTableAlias(joinTableType);
                        var joinTableColumnNames = string.Join(",\n\t",
                            joinTableProperties
                                .Where(x => x.IsPrimitive())
                                .Select(x =>
                                {
                                    var colAlias = cfg.GenerateColumnAlias(x);
                                    var colName = x.GetColumnName();

                                    return $"[{joinTableAlias}].[{colName}] AS [{colAlias}]";
                                }));


                        sb.Append(",\n\t");
                        sb.Append(joinTableColumnNames);

                        joinStatementSb.Append("\n");
                        joinStatementSb.Append($"LEFT OUTER JOIN [{joinTableName}] AS [{joinTableAlias}] ON [{mainTableAlias}].[{mainTablePrimaryKeyName}] = [{joinTableAlias}].[{joinTableForeignKeyName}]");

                        cfg.JoinTablePropertiesMapping.Add(joinTableType, joinTableProperties.ToDictionary(x => x.GetColumnName(), x => x));
                    }
                }

                sb.Append($"\nFROM [{TableName}] AS [{mainTableAlias}]");
                sb.Append(joinStatementSb);
            }
            else
            {
                sb.Append($"SELECT\n\t{columns}\nFROM [{TableName}] AS [{mainTableAlias}]");
            }

            cfg.Sql = sb.ToString();

            config = cfg;
        }

        private void PrepareSelectStatement(IQueryOptions<TEntity> options, out DbSqlSelectStatementConfig config)
        {
            var parameters = new Dictionary<string, object>();
            var sb = new StringBuilder();

            // Append initial select statement
            PrepareSelectStatement(options?.FetchStrategy, out config);

            sb.Append(config.Sql);

            if (options != null)
            {
                // Append where statement
                if (options.Specification != null)
                {
                    new DbSqlExpressionTranslator().Translate(
                        options.Specification.Predicate,
                        config,
                        out string whereSql,
                        out Dictionary<string, object> whereParameters);

                    sb.Append("\n");
                    sb.Append(whereSql);

                    foreach (var item in whereParameters)
                    {
                        parameters.Add(item.Key, item.Value);
                    }
                }

                // Append options (paging, sorting)
                var sortings = options.SortingPropertiesMapping.ToDictionary(x => x.Key, x => x.Value);

                if (!sortings.Any())
                {
                    // Sorts on the Id key by default if no sorting is provided
                    var primaryKeyPropertyInfo = typeof(TEntity).GetPrimaryKeyPropertyInfo();
                    var primaryKeyPropertyName = primaryKeyPropertyInfo.Name;

                    sortings.Add(primaryKeyPropertyName, false);
                }

                sb.Append("\n");
                sb.Append("ORDER BY ");

                foreach (var sorting in sortings)
                {
                    var isSortingDecending = sorting.Value;
                    var sortingProperty = sorting.Key;
                    var lambda = ExpressionHelper.GetExpression<TEntity>(sortingProperty);
                    var tableType = ExpressionHelper.GetMemberExpression(lambda).Expression.Type;
                    var tableName = config.GetTableName(tableType);
                    var tableAlias = config.GetTableAlias(tableName);
                    var sortingPropertyInfo = ExpressionHelper.GetPropertyInfo(lambda);
                    var columnAlias = config.GetColumnAlias(sortingPropertyInfo);

                    sb.Append($"[{tableAlias}].[{columnAlias}] {(isSortingDecending ? "DESC" : "ASC")}, ");
                }

                sb.Remove(sb.Length - 2, 2);

                if (options.PageSize != -1)
                {
                    sb.Append("\n");
                    sb.Append($"OFFSET {options.PageSize} * ({options.PageIndex} - 1) ROWS");
                    sb.Append("\n");
                    sb.Append($"FETCH NEXT {options.PageSize} ROWS ONLY");
                }
            }

            config.Sql = sb.ToString();
            config.Parameters = parameters;
        }

        private void PrepareEntitySetStatement(EntitySet entitySet, out string sql, out Dictionary<string, object> parameters)
        {
            var primeryKeyColumnName = SqlIdentityPropertyInfo.GetColumnName();
            var properties = IsIdentity ? SqlPropertiesMapping.Where(x => !x.Key.Equals(primeryKeyColumnName)) : SqlPropertiesMapping;

            parameters = new Dictionary<string, object>();
            sql = string.Empty;

            switch (entitySet.State)
            {
                case EntityState.Added:
                    {
                        var columnNames = string.Join(", ", properties.Select(x => x.Value.GetColumnName())).TrimEnd();
                        var values = string.Join(", ", properties.Select(x => $"@{x.Value.GetColumnName()}")).TrimEnd();

                        sql = $"INSERT INTO [{TableName}] ({columnNames})\nVALUES ({values})";

                        foreach (var pi in properties)
                        {
                            parameters.Add($"@{pi.Value.GetColumnName()}", pi.Value.GetValue(entitySet.Entity, null));
                        }

                        if (IsIdentity)
                            parameters.Add($"@{primeryKeyColumnName}", SqlIdentityPropertyInfo.GetValue(entitySet.Entity, null));

                        break;
                    }
                case EntityState.Removed:
                    {
                        sql = $"DELETE FROM [{TableName}]\nWHERE {primeryKeyColumnName} = @{primeryKeyColumnName}";

                        parameters.Add($"@{primeryKeyColumnName}", SqlIdentityPropertyInfo.GetValue(entitySet.Entity, null));

                        break;
                    }
                case EntityState.Modified:
                    {
                        var values = string.Join(",\n\t", properties.Select(x =>
                        {
                            var columnName = x.Value.GetColumnName();
                            return columnName + " = " + $"@{columnName}";
                        }));

                        sql = $"UPDATE [{TableName}]\nSET {values}\nWHERE {primeryKeyColumnName} = @{primeryKeyColumnName}";

                        foreach (var pi in properties)
                        {
                            parameters.Add($"@{pi.Value.GetColumnName()}", pi.Value.GetValue(entitySet.Entity, null));
                        }

                        if (IsIdentity)
                            parameters.Add($"@{primeryKeyColumnName}", SqlIdentityPropertyInfo.GetValue(entitySet.Entity, null));

                        break;
                    }
            }
        }

        private TElement AutoMap<TElement>(DbDataReader r, Expression<Func<TEntity, TElement>> elementSelector, DbSqlSelectStatementConfig config)
        {
            var entity = Activator.CreateInstance<TEntity>();
            var entityType = typeof(TEntity);

            Dictionary<Type, object> joinTableInstances = null;

            if (config != null)
            {
                joinTableInstances = config.JoinTablePropertiesMapping.Keys.ToDictionary(x => x, Activator.CreateInstance);
            }

            for (var i = 0; i < r.FieldCount; i++)
            {
                var name = r.GetName(i);
                var value = r[name];

                if (value == DBNull.Value)
                    value = null;

                if (SqlPropertiesMapping.ContainsKey(name) && !r.IsDBNull(r.GetOrdinal(name)))
                {
                    SqlPropertiesMapping[name].SetValue(entity, value);
                }
                else if (joinTableInstances != null)
                {
                    var joinTableType = config.GetTableTypeByColumnAlias(name);

                    if (joinTableType != null)
                    {
                        var columnPropertyInfosMapping = config.JoinTablePropertiesMapping.Single(x => x.Key == joinTableType).Value;
                        var columnName = config.GetColumnName(name);

                        columnPropertyInfosMapping[columnName].SetValue(joinTableInstances[joinTableType], value);
                    }
                }
            }

            if (joinTableInstances != null)
            {
                var mainTableProperties = entityType.GetRuntimeProperties();

                foreach (var item in joinTableInstances)
                {
                    var joinTableInstance = item.Value;
                    var joinTableType = item.Key;

                    // Sets the main table property in the join table
                    var mainTablePropertyInfo = joinTableType.GetRuntimeProperties().Single(x => x.PropertyType == entityType);

                    mainTablePropertyInfo.SetValue(joinTableInstance, entity);

                    // Sets the join table property in the main table
                    var joinTablePropertyInfo = mainTableProperties.Single(x => x.PropertyType == joinTableType);

                    joinTablePropertyInfo.SetValue(entity, joinTableInstance);
                }
            }

            var elementSelectorFunc = elementSelector.Compile();

            return elementSelectorFunc(entity);
        }

        private TEntity AutoMap(DbDataReader r, DbSqlSelectStatementConfig config)
        {
            return AutoMap<TEntity>(r, IdentityExpression<TEntity>.Instance, config);
        }

        #endregion

        #region Overrides of RepositoryBase<TEntity, TKey>

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Clears the collection
                while (_items.Count > 0)
                {
                    _items.TryTake(out _);
                }

                _items = null;
            }

            _disposed = true;
        }

        /// <summary>
        /// A protected overridable method for adding the specified <paramref name="entity" /> into the repository.
        /// </summary>
        protected override void AddItem(TEntity entity)
        {
            _items.Add(new EntitySet(entity, EntityState.Added));
        }

        /// <summary>
        /// A protected overridable method for deleting the specified <paramref name="entity" /> from the repository.
        /// </summary>
        protected override void DeleteItem(TEntity entity)
        {
            _items.Add(new EntitySet(entity, EntityState.Removed));
        }

        /// <summary>
        /// A protected overridable method for updating the specified <paramref name="entity" /> in the repository.
        /// </summary>
        protected override void UpdateItem(TEntity entity)
        {
            _items.Add(new EntitySet(entity, EntityState.Modified));
        }

        /// <summary>
        /// A protected overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override void SaveChanges()
        {
            using (var connection = CreateConnection())
            {
                connection.Open();

                foreach (var entitySet in _items)
                {
                    PrepareEntitySetStatement(
                        entitySet,
                        out string sql,
                        out Dictionary<string, object> parameters);

                    ExecuteNonQuery(connection, sql, parameters);

                    if (entitySet.State == EntityState.Added && IsIdentity)
                    {
                        var key = ExecuteScalar<TKey>(connection, "SELECT @@IDENTITY");

                        entitySet.Entity.SetPrimaryKeyPropertyValue(key);
                    }
                }

                // Clears the collection
                while (_items.Count > 0)
                {
                    _items.TryTake(out _);
                }
            }
        }

        /// <summary>
        /// A protected overridable method for getting an entity query that supplies the specified fetching strategy from the repository.
        /// </summary>
        protected override IQueryable<TEntity> GetQuery(IFetchStrategy<TEntity> fetchStrategy = null)
        {
            PrepareSelectStatement(fetchStrategy, out DbSqlSelectStatementConfig config);

            return ExecuteList<TEntity>(config.Sql, r => AutoMap(r, config)).AsQueryable();
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override TEntity GetEntity(TKey key, IFetchStrategy<TEntity> fetchStrategy)
        {
            var options = new QueryOptions<TEntity>().SatisfyBy(GetByPrimaryKeySpecification(key));

            if (fetchStrategy != null)
                options.Fetch(fetchStrategy);

            PrepareSelectStatement(options, out DbSqlSelectStatementConfig config);

            return ExecuteObject<TEntity>(config.Sql, config.Parameters, r => AutoMap(r, config));
        }

        /// <summary>
        /// Gets an entity that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override TResult GetEntity<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            PrepareSelectStatement(options, out DbSqlSelectStatementConfig config);

            return ExecuteObject<TResult>(config.Sql, config.Parameters, r => AutoMap<TResult>(r, selector, config));
        }

        /// <summary>
        /// Gets a collection of entities that satisfies the criteria specified by the <paramref name="criteria" /> from the repository.
        /// </summary>
        protected override IEnumerable<TResult> GetEntities<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector)
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            PrepareSelectStatement(options, out DbSqlSelectStatementConfig config);

            return ExecuteList<TResult>(config.Sql, config.Parameters, r => AutoMap<TResult>(r, selector, config));
        }

        /// <summary>
        /// Gets the number of entities that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override int GetCount(IQueryOptions<TEntity> options)
        {
            PrepareSelectStatement(options, out DbSqlSelectStatementConfig config);

            using (var reader = ExecuteReader(config.Sql, config.Parameters))
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
        /// A protected overridable method for determining whether the repository contains an entity that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override bool GetExist(IQueryOptions<TEntity> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            PrepareSelectStatement(options, out DbSqlSelectStatementConfig config);

            using (var reader = ExecuteReader(config.Sql, config.Parameters))
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
        /// Gets a new <see cref="Dictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected override Dictionary<TDictionaryKey, TElement> GetDictionary<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            PrepareSelectStatement(options, out DbSqlSelectStatementConfig config);

            return ExecuteDictionary<TDictionaryKey, TElement>(
                config.Sql,
                config.Parameters,
                r => AutoMap<TDictionaryKey>(r, keySelector, config),
                r => AutoMap<TElement>(r, elementSelector, config));
        }

        /// <summary>
        /// Gets a new <see cref="IGrouping{TGroupKey, TElement}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected override IEnumerable<IGrouping<TGroupKey, TElement>> GetGroupBy<TGroupKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector)
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            PrepareSelectStatement(options, out DbSqlSelectStatementConfig config);

            return ExecuteGroup<TGroupKey, TElement>(
                config.Sql,
                config.Parameters,
                r => AutoMap<TGroupKey>(r, keySelector, config),
                r => AutoMap<TElement>(r, elementSelector, config));
        }

        #endregion

        #region Overrides of RepositoryBaseAsync<TEntity, TKey>

        /// <summary>
        /// A protected asynchronous overridable method for saving changes made in the current unit of work in the repository.
        /// </summary>
        protected override async Task SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (var connection = CreateConnection())
            {
                await connection.OpenAsync(cancellationToken);

                foreach (var entitySet in _items)
                {
                    PrepareEntitySetStatement(
                        entitySet,
                        out string sql,
                        out Dictionary<string, object> parameters);

                    await ExecuteNonQueryAsync(connection, sql, parameters, cancellationToken);

                    if (entitySet.State == EntityState.Added && IsIdentity)
                    {
                        var key = await ExecuteScalarAsync<TKey>(connection, "SELECT @@IDENTITY", null, cancellationToken);

                        entitySet.Entity.SetPrimaryKeyPropertyValue(key);
                    }
                }

                // Clears the collection
                while (_items.Count > 0)
                {
                    _items.TryTake(out _);
                }
            }
        }

        /// <summary>
        /// Gets an entity query with the given primary key value from the repository.
        /// </summary>
        protected override Task<TEntity> GetEntityAsync(TKey key, IFetchStrategy<TEntity> fetchStrategy, CancellationToken cancellationToken = new CancellationToken())
        {
            var options = new QueryOptions<TEntity>().SatisfyBy(GetByPrimaryKeySpecification(key));

            if (fetchStrategy != null)
                options.Fetch(fetchStrategy);

            PrepareSelectStatement(options, out DbSqlSelectStatementConfig config);

            return ExecuteObjectAsync<TEntity>(config.Sql, config.Parameters, r => AutoMap(r, config), cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting an entity that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override Task<TResult> GetEntityAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            PrepareSelectStatement(options, out DbSqlSelectStatementConfig config);

            return ExecuteObjectAsync<TResult>(config.Sql, config.Parameters, r => AutoMap<TResult>(r, selector, config), cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting a collection of entities that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override Task<IEnumerable<TResult>> GetEntitiesAsync<TResult>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = new CancellationToken())
        {
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));

            PrepareSelectStatement(options, out DbSqlSelectStatementConfig config);

            return ExecuteListAsync<TResult>(config.Sql, config.Parameters, r => AutoMap<TResult>(r, selector, config), cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting a the number of entities that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override async Task<int> GetCountAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            PrepareSelectStatement(options, out DbSqlSelectStatementConfig config);

            using (var reader = await ExecuteReaderAsync(config.Sql, config.Parameters, cancellationToken))
            {
                var count = 0;

                while (await reader.ReadAsync(cancellationToken))
                {
                    count++;
                }

                return count;
            }
        }

        /// <summary>
        /// A protected asynchronous overridable method for determining whether the repository contains an entity that satisfies the criteria specified by the <paramref name="options" /> from the repository.
        /// </summary>
        protected override async Task<bool> GetExistAsync(IQueryOptions<TEntity> options, CancellationToken cancellationToken = new CancellationToken())
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            PrepareSelectStatement(options, out DbSqlSelectStatementConfig config);

            using (var reader = await ExecuteReaderAsync(config.Sql, config.Parameters, cancellationToken))
            {
                var hasRows = false;

                while (await reader.ReadAsync(cancellationToken))
                {
                    hasRows = true;

                    break;
                }

                return hasRows;
            }
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting a new <see cref="IDictionary{TDictionaryKey, TElement}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected override Task<Dictionary<TDictionaryKey, TElement>> GetDictionaryAsync<TDictionaryKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TDictionaryKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            PrepareSelectStatement(options, out DbSqlSelectStatementConfig config);

            return ExecuteDictionaryAsync<TDictionaryKey, TElement>(
                config.Sql,
                config.Parameters,
                r => AutoMap<TDictionaryKey>(r, keySelector, config),
                r => AutoMap<TElement>(r, elementSelector, config),
                cancellationToken);
        }

        /// <summary>
        /// A protected asynchronous overridable method for getting a new <see cref="IGrouping{TGroupKey, TElemen}" /> according to the specified <paramref name="keySelector" />, an element selector.
        /// </summary>
        protected override Task<IEnumerable<IGrouping<TGroupKey, TElement>>> GetGroupByAsync<TGroupKey, TElement>(IQueryOptions<TEntity> options, Expression<Func<TEntity, TGroupKey>> keySelector, Expression<Func<TEntity, TElement>> elementSelector, CancellationToken cancellationToken = new CancellationToken())
        {
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));

            if (elementSelector == null)
                throw new ArgumentNullException(nameof(elementSelector));

            PrepareSelectStatement(options, out DbSqlSelectStatementConfig config);

            return ExecuteGroupAsync<TGroupKey, TElement>(
                config.Sql,
                config.Parameters,
                r => AutoMap<TGroupKey>(r, keySelector, config),
                r => AutoMap<TElement>(r, elementSelector, config),
                cancellationToken);
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
            /// Initializes a new instance of the <see cref="EntitySet"/> class.
            /// </summary>
            /// <param name="entity">The entity.</param>
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
