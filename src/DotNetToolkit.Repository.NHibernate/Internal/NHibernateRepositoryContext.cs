namespace DotNetToolkit.Repository.NHibernate.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Extensions;
    using Extensions.Internal;
    using global::NHibernate;
    using global::NHibernate.Linq;
    using Query;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Transactions;
    using Utility;

    /// <summary>
    /// An implementation of <see cref="INHibernateRepositoryContext" />.
    /// </summary>
    /// <seealso cref="INHibernateRepositoryContext" />
    internal class NHibernateRepositoryContext : LinqRepositoryContextBaseAsync, INHibernateRepositoryContext
    {
        #region Fields

        private readonly ISessionFactory _sessionFactory;
        private ISession _session;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NHibernateRepositoryContext"/> class.
        /// </summary>
        /// <param name="sessionFactory">The session factory.</param>
        public NHibernateRepositoryContext(ISessionFactory sessionFactory)
        {
            _sessionFactory = Guard.NotNull(sessionFactory, nameof(sessionFactory));

            ConfigureConventions(sessionFactory);
        }

        #endregion

        #region Private Methods

        private ISession GetNewSession()
        {
            return _sessionFactory
                .WithOptions()
                .Interceptor(new LoggerInterceptor(s => Logger?.Debug(s)))
                .OpenSession();
        }

        private void ConfigureConventions(ISessionFactory sessionFactory)
        {
            var helper = new NHibernateConventionsHelper(Guard.NotNull(sessionFactory, nameof(sessionFactory)));

            Conventions = new RepositoryConventions(GetType())
            {
                PrimaryKeysCallback = type => helper.GetPrimaryKeyPropertyInfos(type),
                ColumnNameCallback = pi => helper.GetColumnName(pi),
                ColumnOrderCallback = pi => helper.GetColumnOrder(pi),
                IsColumnMappedCallback = pi => helper.IsColumnMapped(pi)
            };
        }

        #endregion

        #region Implementation of INHibernateRepositoryContext

        /// <summary>
        /// Gets the current session.
        /// </summary>
        public virtual ISession Session
        {
            get { return _session ?? (_session = GetNewSession()); }
        }

        #endregion

        #region Overrides of LinqRepositoryContextBase

        /// <summary>
        /// Returns the entity's query.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <returns>The entity's query.</returns>
        protected override IQueryable<TEntity> AsQueryable<TEntity>()
        {
            return Session.Query<TEntity>();
        }

        /// <summary>
        /// Apply a fetching options to the specified entity's query.
        /// </summary>
        /// <returns>The entity's query with the applied options.</returns>
        protected override IQueryable<TEntity> ApplyFetchingOptions<TEntity>(IQueryable<TEntity> query, IQueryOptions<TEntity> options)
        {
            // TODO: WILL NEED TO COME BACK TO THIS
            // NHibernate seems to auto load entities whenever it wants based on mappings...
            // so maybe we should just let nhibernate do its thing??

            if (options?.FetchStrategy?.PropertyPaths?.Any() == true)
                Logger.Debug("The nhibernate context does not support fetching strategy. Please consider using the nhibernate relationship mappings instead.");

            return query;
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public override IEnumerable<TEntity> ExecuteSqlQuery<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, TEntity> projector)
        {
            Guard.NotEmpty(sql, nameof(sql));
            Guard.NotNull(projector, nameof(projector));

            var connection = Session.Connection;
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

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>The number of rows affected.</returns>
        public override int ExecuteSqlCommand(string sql, CommandType cmdType, Dictionary<string, object> parameters)
        {
            Guard.NotEmpty(sql, nameof(sql));

            var connection = Session.Connection;
            var shouldOpenConnection = connection.State != ConnectionState.Open;

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

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns>The transaction.</returns>
        public override ITransactionManager BeginTransaction()
        {
            CurrentTransaction = new NHibernateTransactionManager(Session.BeginTransaction());

            return CurrentTransaction;
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be inserted into the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Add<TEntity>(TEntity entity)
        {
            Session.Save(Guard.NotNull(entity, nameof(entity)));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be updated in the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Update<TEntity>(TEntity entity)
        {
            Session.Update(Guard.NotNull(entity, nameof(entity)));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be removed from the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Remove<TEntity>(TEntity entity)
        {
            Session.Delete(Guard.NotNull(entity, nameof(entity)));
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public override int SaveChanges()
        {
            Session.Flush();

            return -1;
        }

        #endregion

        #region Overrides of LinqRepositoryContextBaseAsync

        /// <summary>
        /// An overridable method to return the first element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        protected override Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken)
        {
            return source.FirstOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// An overridable method to create a <see cref="T:System.Collections.Generic.List`1" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it asynchronously.
        /// </summary>
        protected override Task<List<TSource>> ToListAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken)
        {
            return source.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// An overridable method to return the number of elements in a sequence.
        /// </summary>
        protected override Task<int> CountAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken)
        {
            return source.CountAsync(cancellationToken);
        }

        /// <summary>
        /// An overridable method to determine whether a sequence contains any elements.
        /// </summary>
        protected override Task<bool> AnyAsync<TSource>(IQueryable<TSource> source, CancellationToken cancellationToken)
        {
            return source.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// An overridable method to create a <see cref="T:System.Collections.Generic.Dictionary`2" /> from an <see cref="T:System.Linq.IQueryable`1" /> by enumerating it asynchronously  according to a specified key selector and an element selector function.
        /// </summary>
        protected override Task<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(IQueryable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, CancellationToken cancellationToken)
        {
            return Task.FromResult<Dictionary<TKey, TElement>>(source.ToDictionary<TSource, TKey, TElement>(keySelector, elementSelector));
        }

        /// <summary>
        /// Asynchronously creates raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a list which each entity has been projected into a new form.</returns> 
        public override async Task<IEnumerable<TEntity>> ExecuteSqlQueryAsync<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, TEntity> projector, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.NotEmpty(sql, nameof(sql));
            Guard.NotNull(projector, nameof(projector));

            var connection = Session.Connection;
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

        /// <summary>
        /// Asynchronously creates raw SQL query that is executed directly in the database.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of rows affected.</returns>
        public override async Task<int> ExecuteSqlCommandAsync(string sql, CommandType cmdType, Dictionary<string, object> parameters, CancellationToken cancellationToken = new CancellationToken())
        {
            Guard.NotEmpty(sql, nameof(sql));

            var connection = Session.Connection;
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

        /// <summary>
        /// Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing the number of state entries written to the database.</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            await Session.FlushAsync(cancellationToken);

            return -1;
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            Session.Close();
            Session.Dispose();

            base.Dispose();
        }

        #endregion
    }
}
