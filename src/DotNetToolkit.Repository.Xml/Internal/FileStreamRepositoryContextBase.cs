namespace DotNetToolkit.Repository.Xml.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using Extensions;
    using Properties;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Transactions;
    using Transactions.Internal;
    using Utility;

    /// <summary>
    /// Represents a repository context class which handles reading and writing to a file.
    /// </summary>
    public abstract class FileStreamRepositoryContextBase : LinqRepositoryContextBase
    {
        #region Fields

        private readonly bool _ignoreTransactionWarning;
        private readonly bool _ignoreSqlQueryWarning;
        private readonly string _path;
        private readonly string _extension;
        private readonly BlockingCollection<EntitySet> _items;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileStreamRepositoryContextBase" /> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        /// <param name="extension">The file extension.</param>
        /// <param name="ignoreTransactionWarning">If a transaction operation is requested, ignore any warnings since the context provider does not support transactions.</param>
        /// <param name="ignoreSqlQueryWarning">If a SQL query is executed, ignore any warnings since the in-memory provider does not support SQL query execution.</param>
        protected FileStreamRepositoryContextBase(string path, string extension, bool ignoreTransactionWarning = false, bool ignoreSqlQueryWarning = false)
        {
            Guard.NotEmpty(path, nameof(path));
            Guard.NotEmpty(extension, nameof(extension));

            if (!string.IsNullOrEmpty(Path.GetExtension(path)))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The specified '{0}' path cannot be a file name.", path));

            if (!extension.StartsWith(".") || string.IsNullOrEmpty(Path.GetExtension(extension)))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "The specified '{0}' extension is not valid.", path));

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (!path.EndsWith(@"\"))
                path += @"\";

            Conventions = RepositoryConventions.Default<XmlRepositoryContext>();

            _items = new BlockingCollection<EntitySet>();
            _ignoreTransactionWarning = ignoreTransactionWarning;
            _ignoreSqlQueryWarning = ignoreSqlQueryWarning;
            _path = path;
            _extension = extension;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// A protected overridable method for loading the entities from the specified stream reader.
        /// </summary>
        protected abstract IEnumerable<TEntity> OnLoaded<TEntity>(StreamReader reader);

        /// <summary>
        /// A protected overridable method for saving the entities to the specified stream writer.
        /// </summary>
        protected abstract void OnSaved<TEntity>(StreamWriter writer, IEnumerable<TEntity> entities);

        #endregion

        #region Private Methods

        private string GetFileName(Type type)
        {
            return $"{_path}{Conventions.GetTableName(type)}{_extension}";
        }

        private IEnumerable LoadFile(Type type)
        {
            var fileName = GetFileName(type);

            if (!File.Exists(fileName) || new FileInfo(fileName).Length == 0)
                return Enumerable.Empty<object>();

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                var entities = (IEnumerable)GetType()
                    .GetRuntimeMethods()
                    .Single(x => x.Name == nameof(OnLoaded) &&
                                 x.IsGenericMethodDefinition &&
                                 x.GetGenericArguments().Length == 1 &&
                                 x.GetParameters().Length == 1)
                    .MakeGenericMethod(type)
                    .Invoke(this, new object[] { reader });

                Logger.Debug($"Loaded entities from '{fileName}' file.");

                return entities;
            }
        }

        private void SaveFile(Type type, IEnumerable entities)
        {
            var fileName = GetFileName(type);

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete))
            using (var writer = new StreamWriter(stream))
            {
                GetType()
                    .GetRuntimeMethods()
                    .Single(x => x.Name == nameof(SaveFile) &&
                                 x.IsGenericMethodDefinition &&
                                 x.GetGenericArguments().Length == 1 &&
                                 x.GetParameters().Length == 2)
                    .MakeGenericMethod(type)
                    .Invoke(this, new object[] { writer, entities });

                Logger.Debug($"Saved entities to '{fileName}' file.");
            }
        }

        internal void SaveFile<TEntity>(StreamWriter writer, IEnumerable entities)
            => OnSaved<TEntity>(writer, entities.Cast<TEntity>().ToList());

        private object GetPrimaryKeyValue(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var keyValues = Conventions.GetPrimaryKeyValues(obj);

            return keyValues.Length == 1 ? keyValues[0] : string.Join(":", keyValues);
        }

        private object GeneratePrimaryKey(Type entityType, object lastKeyInFile)
        {
            var propertyInfo = Conventions.GetPrimaryKeyPropertyInfos(entityType).First();
            var propertyType = propertyInfo.PropertyType;

            if (propertyType == typeof(Guid))
                return Guid.NewGuid();

            if (propertyType == typeof(string))
                return Guid.NewGuid().ToString("N");

            if (propertyType == typeof(int))
            {
                return Convert.ToInt32(lastKeyInFile) + 1;
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyValueTypeInvalid, entityType.FullName, propertyType));
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
            return LoadFile(typeof(TEntity)).Cast<TEntity>().AsQueryable();
        }

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns>The transaction.</returns>
        public override ITransactionManager BeginTransaction()
        {
            if (!_ignoreTransactionWarning)
                throw new NotSupportedException(DotNetToolkit.Repository.Properties.Resources.TransactionNotSupported);

            CurrentTransaction = NullTransactionManager.Instance;

            return CurrentTransaction;
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be inserted into the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Add<TEntity>(TEntity entity)
        {
            _items.Add(new EntitySet(Guard.NotNull(entity, nameof(entity)), EntityState.Added));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be updated in the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Update<TEntity>(TEntity entity)
        {
            _items.Add(new EntitySet(Guard.NotNull(entity, nameof(entity)), EntityState.Modified));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be removed from the database when <see cref="SaveChanges" /> is called.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public override void Remove<TEntity>(TEntity entity)
        {
            _items.Add(new EntitySet(Guard.NotNull(entity, nameof(entity)), EntityState.Removed));
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public override int SaveChanges()
        {
            var count = 0;

            try
            {
                if (_items.Count == 0)
                    return count;

                var store = new Dictionary<Type, Dictionary<object, object>>();

                while (_items.TryTake(out var entitySet))
                {
                    var key = GetPrimaryKeyValue(entitySet.Entity);
                    var entityType = entitySet.Entity.GetType();

                    if (!store.ContainsKey(entityType))
                    {
                        store[entityType] = LoadFile(entityType)
                            .Cast<object>()
                            .ToDictionary(GetPrimaryKeyValue);
                    }

                    var context = store[entityType];

                    if (entitySet.State == EntityState.Added)
                    {
                        if (context.ContainsKey(key))
                        {
                            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                Resources.EntityAlreadyBeingTrackedInStore, entityType));
                        }

                        var primaryKeyPropertyInfo = Conventions.GetPrimaryKeyPropertyInfos(entityType).First();

                        if (Conventions.IsColumnIdentity(primaryKeyPropertyInfo))
                        {
                            key = GeneratePrimaryKey(entityType, context.Keys.LastOrDefault());

                            primaryKeyPropertyInfo.SetValue(entitySet.Entity, key);
                        }
                    }
                    else if (!context.ContainsKey(key))
                    {
                        throw new InvalidOperationException(Resources.EntityNotFoundInStore);
                    }

                    if (entitySet.State == EntityState.Removed)
                    {
                        context.Remove(key);
                    }
                    else
                    {
                        context[key] = entitySet.Entity;
                    }

                    count++;
                }

                foreach (var item in store)
                {
                    SaveFile(item.Key, item.Value.Values);
                }
            }
            finally
            {
                // Clears the collection
                while (_items.Count > 0)
                {
                    _items.TryTake(out _);
                }
            }

            return count;
        }

        /// <summary>
        /// Creates a raw SQL query that is executed directly in the database and returns a collection of entities.
        /// </summary>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="cmdType">The command type.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <param name="projector">A function to project each entity into a new form.</param>
        /// <returns>A list which each entity has been projected into a new form.</returns>
        public override IEnumerable<TEntity> ExecuteSqlQuery<TEntity>(string sql, CommandType cmdType, Dictionary<string, object> parameters, Func<IDataReader, IRepositoryConventions, TEntity> projector)
        {
            if (!_ignoreSqlQueryWarning)
                throw new NotSupportedException(Repository.Properties.Resources.QueryExecutionNotSupported);

            Guard.NotEmpty(sql, nameof(sql));
            Guard.NotNull(projector, nameof(projector));

            return Enumerable.Empty<TEntity>();
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
            if (!_ignoreSqlQueryWarning)
                throw new NotSupportedException(Repository.Properties.Resources.QueryExecutionNotSupported);

            Guard.NotEmpty(sql, nameof(sql));

            return 0;
        }

        #endregion

        #region Nested Type: EntitySet

        /// <summary>
        /// Represents an internal entity set, which holds the entity and it's state representing the operation that was performed at the time.
        /// </summary>
        class EntitySet
        {
            public EntitySet(object entity, EntityState state)
            {
                Entity = entity;
                State = state;
            }

            public object Entity { get; }

            public EntityState State { get; }
        }

        #endregion

        #region Nested Type: EntityState

        /// <summary>
        /// Represents an internal state for an entity.
        /// </summary>
        enum EntityState
        {
            Added,
            Removed,
            Modified
        }

        #endregion
    }
}
