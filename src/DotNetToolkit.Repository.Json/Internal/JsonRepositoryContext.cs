namespace DotNetToolkit.Repository.Json.Internal
{
    using Configuration;
    using Configuration.Conventions;
    using FetchStrategies;
    using Helpers;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Properties;
    using Queries;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Transactions;

    /// <summary>
    /// Represents an internal json repository context.
    /// </summary>
    /// <seealso cref="IRepositoryContext" />
    internal class JsonRepositoryContext : IRepositoryContext
    {
        #region Fields

        private readonly string _path;

        private readonly ConcurrentDictionary<Type, BlockingCollection<EntitySet>> _store = new ConcurrentDictionary<Type, BlockingCollection<EntitySet>>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        protected string FileExtension { get; } = ".json";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonRepositoryContext" /> class.
        /// </summary>
        /// <param name="path">The database directory to create.</param>
        public JsonRepositoryContext(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));

            if (!string.IsNullOrEmpty(Path.GetExtension(path)))
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.CannotBeFileName, path));

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (!path.EndsWith(@"\"))
                path += @"\";

            _path = path;
        }

        #endregion

        #region Private Methods

        private void ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(object[] keyValues) where TEntity : class
        {
            if (keyValues.Length != PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos<TEntity>().Count())
                throw new ArgumentException(DotNetToolkit.Repository.Properties.Resources.EntityPrimaryKeyValuesLengthMismatch, nameof(keyValues));
        }

        private IQueryable<TEntity> GetQuery<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            return options != null ? options.Apply(AsQueryable<TEntity>(options.FetchStrategy)) : AsQueryable<TEntity>();
        }

        private void InvokeOnFileSaved(Type type, IEnumerable entities)
        {
            GetType()
                .GetRuntimeMethods()
                .Single(x => x.Name == nameof(OnFileSaved) &&
                             x.IsGenericMethodDefinition &&
                             x.GetGenericArguments().Length == 1 &&
                             x.GetParameters().Length == 1)
                .MakeGenericMethod(type)
                .Invoke(this, new[] { entities });
        }

        private IEnumerable<object> InvokeOnFileLoaded(Type type)
        {
            return (IEnumerable<object>)GetType()
                .GetRuntimeMethods()
                .Single(x => x.Name == nameof(OnFileLoaded) &&
                             x.IsGenericMethodDefinition &&
                             x.GetGenericArguments().Length == 1 &&
                             x.GetParameters().Length == 0)
                .MakeGenericMethod(type)
                .Invoke(this, null);
        }

        private IEnumerable<TEntity> OnFileLoaded<TEntity>()
        {
            var fileName = $"{_path}{typeof(TEntity).GetTableName()}{FileExtension}";

            if (!File.Exists(fileName) || new FileInfo(fileName).Length == 0)
                return Enumerable.Empty<TEntity>();

            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                var serializer = new JsonSerializer();
                var entities = (List<TEntity>)serializer.Deserialize(reader, typeof(List<TEntity>));

                return entities.ToList();
            }
        }

        private void OnFileSaved<TEntity>(IEnumerable entities)
        {
            var fileName = $"{_path}{typeof(TEntity).GetTableName()}{FileExtension}";

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Delete))
            using (var writer = new StreamWriter(stream))
            {
                var serializer = new JsonSerializer
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                serializer.Serialize(writer, entities.Cast<TEntity>().ToList<TEntity>());
            }
        }

        private object GeneratePrimaryKey(Type entityType, IEnumerable<object> entitiesInFile)
        {
            if (entitiesInFile == null)
                throw new ArgumentNullException(nameof(entitiesInFile));

            var propertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();
            var propertyType = propertyInfo.PropertyType;

            if (propertyType == typeof(Guid))
                return Guid.NewGuid();

            if (propertyType == typeof(string))
                return Guid.NewGuid().ToString("N");

            if (propertyType == typeof(int))
            {
                var lastKeyInTemp = _store[entityType]
                    .Where(x => x.State == EntityState.Unchanged)
                    .Select(x => PrimaryKeyConventionHelper.GetPrimaryKeyValue(x.Entity))
                    .OrderByDescending(x => x)
                    .FirstOrDefault();

                var lastKeyInFile = entitiesInFile
                    .Select(PrimaryKeyConventionHelper.GetPrimaryKeyValue)
                    .OrderByDescending(x => x)
                    .FirstOrDefault();

                var key = Math.Max(Convert.ToInt32(lastKeyInTemp), Convert.ToInt32(lastKeyInFile));

                return key + 1;
            }

            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.EntityKeyValueTypeInvalid, entityType.FullName, propertyType));
        }

        #endregion

        #region Implementation of IRepositoryContext

        /// <summary>
        /// Begins the transaction.
        /// </summary>
        /// <returns>The transaction.</returns>
        public ITransactionManager BeginTransaction()
        {
            throw new NotSupportedException(Resources.TransactionNotSupported);
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be inserted into the database when <see cref="SaveChanges" /> is called..
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            var entityType = typeof(TEntity);

            if (!_store.ContainsKey(entityType))
                _store[entityType] = new BlockingCollection<EntitySet>();

            _store[entityType].Add(new EntitySet(entity, EntityState.Added));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be updated in the database when <see cref="SaveChanges" /> is called..
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            var entityType = typeof(TEntity);

            if (!_store.ContainsKey(entityType))
                _store[entityType] = new BlockingCollection<EntitySet>();

            _store[entityType].Add(new EntitySet(entity, EntityState.Modified));
        }

        /// <summary>
        /// Tracks the specified entity in memory and will be removed from the database when <see cref="SaveChanges" /> is called..
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            var entityType = typeof(TEntity);

            if (!_store.ContainsKey(entityType))
                _store[entityType] = new BlockingCollection<EntitySet>();

            _store[entityType].Add(new EntitySet(entity, EntityState.Removed));
        }

        /// <summary>
        /// Saves all changes made in this context to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        public int SaveChanges()
        {
            var store = new ConcurrentDictionary<Type, ConcurrentDictionary<object, object>>();
            var count = 0;

            try
            {
                // Validates all the entities before saving to the file
                foreach (var item in _store)
                {
                    var entityType = item.Key;
                    var entitiesInFile = InvokeOnFileLoaded(entityType);

                    foreach (var entitySet in item.Value)
                    {
                        var key = PrimaryKeyConventionHelper.GetPrimaryKeyValue(entitySet.Entity);

                        if (!store.ContainsKey(entityType))
                            store[entityType] = new ConcurrentDictionary<object, object>(entitiesInFile.ToDictionary(PrimaryKeyConventionHelper.GetPrimaryKeyValue));

                        var context = store[entityType];

                        if (entitySet.State == EntityState.Added)
                        {
                            if (context.ContainsKey(key))
                            {
                                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture,
                                    Resources.EntityAlreadyBeingTrackedInStore, entitySet.Entity.GetType()));
                            }

                            var primeryKeyPropertyInfo = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).First();

                            if (primeryKeyPropertyInfo.IsColumnIdentity())
                            {
                                key = GeneratePrimaryKey(entityType, entitiesInFile);

                                primeryKeyPropertyInfo.SetValue(entitySet.Entity, key);
                            }
                        }
                        else if (!context.ContainsKey(key))
                        {
                            throw new InvalidOperationException(Resources.EntityNotFoundInStore);
                        }

                        if (entitySet.State == EntityState.Removed)
                        {
                            context.TryRemove(key, out _);
                        }
                        else
                        {
                            entitySet.State = EntityState.Unchanged;

                            context[key] = entitySet.Entity;
                        }

                        count++;
                    }
                }


                // Saves the entities to the file
                foreach (var item in store)
                {
                    InvokeOnFileSaved(item.Key, item.Value.Values);
                }
            }
            finally
            {
                _store.Clear();
                store.Clear();
            }

            return count;
        }

        /// <summary>
        /// Returns the entity <see cref="System.Linq.IQueryable{TEntity}" />.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <returns>The entity <see cref="System.Linq.IQueryable{TEntity}" />.</returns>
        public IQueryable<TEntity> AsQueryable<TEntity>() where TEntity : class
        {
            return AsQueryable<TEntity>((IFetchStrategy<TEntity>)null);
        }

        /// <summary>
        /// Returns the entity <see cref="System.Linq.IQueryable{TEntity}" /> using the specified fetching strategy.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <returns>The entity <see cref="System.Linq.IQueryable{TEntity}" />.</returns>
        public IQueryable<TEntity> AsQueryable<TEntity>(IFetchStrategy<TEntity> fetchStrategy) where TEntity : class
        {
            var entities = OnFileLoaded<TEntity>();

            return entities.AsQueryable();
        }

        /// <summary>
        /// Finds an entity with the given primary key values in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="fetchStrategy">Defines the child objects that should be retrieved when loading the entity</param>
        /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
        /// <returns>The entity found in the repository.</returns>
        public virtual TEntity Find<TEntity>(IFetchStrategy<TEntity> fetchStrategy, params object[] keyValues) where TEntity : class
        {
            if (keyValues == null)
                throw new ArgumentNullException(nameof(keyValues));

            ThrowsIfEntityPrimaryKeyValuesLengthMismatch<TEntity>(keyValues);

            var key = PrimaryKeyConventionHelper.MergePrimaryKeyValues(keyValues);
            var entityType = typeof(TEntity);
            var context = InvokeOnFileLoaded(entityType).ToDictionary(PrimaryKeyConventionHelper.GetPrimaryKeyValue);

            if (!context.ContainsKey(key))
                return default(TEntity);

            var entity = context[key];

            return (TEntity)Convert.ChangeType(entity, entityType);
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

            return GetQuery(options).Select(selector).FirstOrDefault();
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

            return GetQuery(options).Select(selector).ToList();
        }

        /// <summary>
        /// Finds the collection of entities in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <returns>The collection of entities in the repository.</returns>
        public IEnumerable<TEntity> FindAll<TEntity>() where TEntity : class
        {
            return FindAll<TEntity, TEntity>((IQueryOptions<TEntity>)null, IdentityExpression<TEntity>.Instance);
        }

        /// <summary>
        /// Returns the number of entities that satisfies the criteria specified by the <paramref name="options" /> in the repository.
        /// </summary>
        /// <typeparam name="TEntity">The type of the of the entity.</typeparam>
        /// <param name="options">The options to apply to the query.</param>
        /// <returns>The number of entities that satisfied the criteria specified by the <paramref name="options" /> in the repository.</returns>
        public int Count<TEntity>(IQueryOptions<TEntity> options) where TEntity : class
        {
            return GetQuery(options).Count();
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

            return GetQuery(options).Any();
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

            return GetQuery(options).ToDictionary(keySelectFunc, elementSelectorFunc);
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

            return GetQuery(options).GroupBy(keySelectFunc, resultSelectorFunc).ToList();
        }

        #endregion

        #region Implementation of IDisposable

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            _store.Clear();
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
            /// Gets or sets the state.
            /// </summary>
            public EntityState State { get; set; }

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
            Modified,
            Unchanged
        }

        #endregion
    }
}
