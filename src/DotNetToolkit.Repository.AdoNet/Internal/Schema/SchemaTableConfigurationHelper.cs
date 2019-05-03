namespace DotNetToolkit.Repository.AdoNet.Internal.Schema
{
    using Configuration.Conventions;
    using Configuration.Logging;
    using Extensions;
    using Properties;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Utility;

    /// <summary>
    /// Represents an internal schema database helper for executing operations related to editing the database/table schema.
    /// </summary>
    internal class SchemaTableConfigurationHelper
    {
        #region Fields

        private readonly IRepositoryConventions _conventions;
        private readonly DbHelper _dbHelper;
        private readonly Dictionary<Type, bool> _tableCreationMapping = new Dictionary<Type, bool>();
        private readonly Dictionary<Type, Tuple<string, Type>> _foreignTableCreationMapping = new Dictionary<Type, Tuple<string, Type>>();
        private readonly ConcurrentDictionary<Type, bool> _schemaValidationTypeMapping = new ConcurrentDictionary<Type, bool>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaTableConfigurationHelper"/> class.
        /// </summary>
        /// <param name="conventions">The configurable conventions.</param>
        /// <param name="dbHelper">The database helper.</param>
        public SchemaTableConfigurationHelper(IRepositoryConventions conventions, DbHelper dbHelper)
        {
            _conventions = Guard.NotNull(conventions);
            _dbHelper = Guard.NotNull(dbHelper);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Validates the specified type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        public void ExecuteSchemaValidate(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            // Performs some schema validation for this type (either creates the table if does not exist, or validates)
            if (!_schemaValidationTypeMapping.ContainsKey(entityType))
            {
                try
                {
                    if (!ExecuteTableExists(entityType))
                    {
                        ExecuteTableCreate(entityType);
                    }
                    else
                    {
                        ExecuteTableValidate(entityType);
                    }
                }
                finally
                {
                    _schemaValidationTypeMapping[entityType] = true;
                }
            }
        }

        /// <summary>
        /// Checks if table exists.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns><c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        public bool ExecuteTableExists<TEntity>() where TEntity : class
        {
            return ExecuteTableExists(typeof(TEntity));
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        public void ExecuteTableCreate<TEntity>() where TEntity : class
        {
            ExecuteTableCreate(typeof(TEntity));
        }

        /// <summary>
        /// Validates the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        public void ExecuteTableValidate<TEntity>() where TEntity : class
        {
            ExecuteTableValidate(typeof(TEntity));
        }

        /// <summary>
        /// Asynchronously initializes the specified entity type.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public async Task ExecuteSchemaValidateAsync(Type entityType, CancellationToken cancellationToken = new CancellationToken())
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            // Performs some schema validation for this type (either creates the table if does not exist, or validates)
            if (!_schemaValidationTypeMapping.ContainsKey(entityType))
            {
                try
                {
                    if (!await ExecuteTableExistsAsync(entityType, cancellationToken))
                    {
                        await ExecuteTableCreateAsync(entityType, cancellationToken);
                    }
                    else
                    {
                        await ExecuteTableValidateAsync(entityType, cancellationToken);
                    }
                }
                finally
                {
                    _schemaValidationTypeMapping[entityType] = true;
                }
            }
        }

        /// <summary>
        /// Asynchronously checks if table exists.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns><c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation, containing a value indicating <c>true</c> if the table exists; otherwise, <c>false</c>..</returns>
        public Task<bool> ExecuteTableExistsAsync<TEntity>(CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            return ExecuteTableExistsAsync(typeof(TEntity), cancellationToken);
        }

        /// <summary>
        /// Asynchronously creates the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public Task ExecuteTableCreateAsync<TEntity>(CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            return ExecuteTableCreateAsync(typeof(TEntity), cancellationToken);
        }

        /// <summary>
        /// Asynchronously validates the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="cancellationToken">A <see cref="System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="System.Threading.Tasks.Task" /> that represents the asynchronous operation.</returns>
        public Task ExecuteTableValidateAsync<TEntity>(CancellationToken cancellationToken = new CancellationToken()) where TEntity : class
        {
            return ExecuteTableValidateAsync(typeof(TEntity), cancellationToken);
        }

        #endregion

        #region Private Methods

        private bool ExecuteTableExists(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            var tableName = _conventions.GetTableName(entityType);
            var sql = @"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName";
            var parameters = new Dictionary<string, object> { { "@tableName", tableName } };

            using (var reader = _dbHelper.ExecuteReader(sql, parameters))
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

        private void ExecuteTableValidate(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            var tableName = _conventions.GetTableName(entityType);
            var schemaTableColumns = GetSchemaTableColumns(tableName);

            ValidateTable(entityType, schemaTableColumns, tableName);
        }

        private async Task ExecuteTableValidateAsync(Type entityType, CancellationToken cancellationToken)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            var tableName = _conventions.GetTableName(entityType);
            var schemaTableColumns = await GetSchemaTableColumnsAsync(tableName, cancellationToken);

            ValidateTable(entityType, schemaTableColumns, tableName);
        }

        private void ValidateTable(Type entityType, IEnumerable<SchemaTableColumn> schemaTableColumns, string tableName)
        {
            var error = new Dictionary<string, bool>();

            var propertiesMapping = entityType
                .GetRuntimeProperties()
                .Where(x => x.IsPrimitive())
                .OrderBy(_conventions.GetColumnOrderOrDefault)
                .ToDictionary(_conventions.GetColumnName, x => x);

            var columns = propertiesMapping.Keys.ToArray();

            // Ensure we have the same number of columns and they all have the same name
            if (schemaTableColumns.Count() != columns.Length)
                error["ColumnCount_Mismatch"] = true;

            if (!schemaTableColumns.All(x => columns.Contains(x.ColumnName)))
                error["ColumnName_Mismatch"] = true;

            if (error.Any())
                throw new InvalidOperationException(string.Format(Resources.SchemaTableColumnsMismatch, entityType.Name));

            // Gets all the constraints
            var schemaTableColumnConstraintsMapping = GetSchemaTableColumnConstraintsMapping(tableName, columns);
            var primaryKeyPropertiesMapping = _conventions.GetPrimaryKeyPropertyInfos(entityType).ToDictionary(_conventions.GetColumnName, x => x);

            // Gets all the foreign keys
            var foreignKeyPropertyInfosMapping = entityType
                .GetRuntimeProperties()
                .Where(x => x.IsComplex())
                .Select(pi => _conventions.GetForeignKeyPropertyInfos(entityType, pi.PropertyType)
                    .ToDictionary(_conventions.GetColumnName, x => pi))
                .SelectMany(x => x)
                .ToDictionary(x => x.Key, x => x.Value);

            // Validate all the columns
            foreach (var schemaTableColumn in schemaTableColumns)
            {
                var columnName = schemaTableColumn.ColumnName;
                var propertyInfo = propertiesMapping[columnName];

                // Property type
                var columnDataType = DbHelper.MapToType(schemaTableColumn.DataType);
                if (columnDataType != propertyInfo.PropertyType)
                    error["PropertyType_Mismatch"] = true;

                // Property order
                if (!error.Any())
                {
                    var columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
                    if (columnAttribute != null && columnAttribute.Order != -1)
                    {
                        var order = schemaTableColumn.OrdinalPosition;

                        // This is cant be a simple 'schemaTableColumn.OrdinalPosition != columnAttribute.Order' to check for a mismatch...
                        // if the type has foreign composite keys, the foreign keys will have an ordering attribute that matches the one on the
                        // foreign entity primary keys, not necessarily the ones that are in the sql database
                        if (foreignKeyPropertyInfosMapping.ContainsKey(columnName))
                        {
                            var foreignPropertyInfo = foreignKeyPropertyInfosMapping[columnName];

                            if (_conventions.HasCompositePrimaryKey(foreignPropertyInfo.PropertyType))
                            {
                                order = schemaTableColumn.OrdinalPosition - (schemaTableColumn.OrdinalPosition - columnAttribute.Order);
                            }
                        }

                        if (order != columnAttribute.Order)
                            error["OrdinalPosition_Mismatch"] = true;
                    }
                }

                // Property constraints
                if (!error.Any())
                {
                    var requiredAttribute = propertyInfo.GetCustomAttribute<RequiredAttribute>();
                    var canBeNull = !propertyInfo.PropertyType.IsValueType || Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null;
                    if (canBeNull && schemaTableColumn.IsNullable == "NO")
                    {
                        if (requiredAttribute == null && !primaryKeyPropertiesMapping.ContainsKey(columnName))
                            error["IsNullable_Mismatch"] = true;
                    }

                    if (schemaTableColumnConstraintsMapping != null &&
                        schemaTableColumnConstraintsMapping.ContainsKey(columnName))
                    {
                        var constraint = schemaTableColumnConstraintsMapping[columnName];

                        switch (constraint)
                        {
                            case SchemaTableConstraintType.NotNull:
                                {
                                    if (requiredAttribute == null)
                                        error["IsNullable_Mismatch"] = true;

                                    break;
                                }
                            case SchemaTableConstraintType.PrimaryKey:
                                {
                                    if (!primaryKeyPropertiesMapping.ContainsKey(columnName))
                                        error["PrimaryKey_Mismatch"] = true;

                                    break;
                                }
                        }
                    }
                }

                // Property length
                if (!error.Any())
                {
                    var stringLengthAttribute = propertyInfo.GetCustomAttribute<StringLengthAttribute>();
                    if (stringLengthAttribute != null)
                    {
                        if (schemaTableColumn.CharacterMaximunLength != stringLengthAttribute.MaximumLength)
                            error["CharacterMaximunLength_Mismatch"] = true;
                    }
                }

                if (error.Any())
                    throw new InvalidOperationException(string.Format(Resources.SchemaTableColumnsMismatch, entityType.Name));
            }
        }

        private IEnumerable<SchemaTableColumn> GetSchemaTableColumns(string tableName)
        {
            var sql = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName";
            var parameters = new Dictionary<string, object> { { "@tableName", tableName } };

            return _dbHelper.ExecuteList<SchemaTableColumn>(sql, parameters)?.Result;
        }

        private async Task<IEnumerable<SchemaTableColumn>> GetSchemaTableColumnsAsync(string tableName, CancellationToken cancellationToken)
        {
            var sql = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName";
            var parameters = new Dictionary<string, object> { { "@tableName", tableName } };

            return (await _dbHelper.ExecuteListAsync<SchemaTableColumn>(sql, parameters, cancellationToken))?.Result;
        }

        private Dictionary<string, string> GetSchemaTableColumnConstraintsMapping(string tableName, string[] columns)
        {
            // Gets all the constraints
            var sb = new StringBuilder();

            sb.AppendLine("SELECT");
            sb.AppendLine("\tINFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.COLUMN_NAME,");
            sb.AppendLine("\tINFORMATION_SCHEMA.TABLE_CONSTRAINTS.CONSTRAINT_TYPE");
            sb.AppendLine("FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE");
            sb.AppendLine("JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS ON INFORMATION_SCHEMA.TABLE_CONSTRAINTS.CONSTRAINT_NAME = INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.CONSTRAINT_NAME");
            sb.AppendLine("WHERE");
            sb.AppendLine("\tINFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.TABLE_NAME = @tableName AND");
            sb.Append("\tINFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.COLUMN_NAME IN (@column)");

            var sql = sb.ToString();

            var parameters = new Dictionary<string, object>
            {
                {"@tableName", tableName},
                {"@column", columns}
            };

            try
            {
                return _dbHelper.ExecuteDictionary<string, string>(sql, parameters)?.Result;
            }
            catch (Exception ex)
            {
                _dbHelper.Logger.Error(ex);

                return null;
            }
        }

        private void ExecuteTableCreate(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            foreach (var item in GetPreparedCreateTableQueriesMapping(entityType))
            {
                if (!ExecuteTableExists(item.Key))
                {
                    _dbHelper.ExecuteNonQuery(item.Value);
                }
            }
        }

        private async Task ExecuteTableCreateAsync(Type entityType, CancellationToken cancellationToken)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            foreach (var item in GetPreparedCreateTableQueriesMapping(entityType))
            {
                if (!await ExecuteTableExistsAsync(item.Key, cancellationToken: cancellationToken))
                {
                    await _dbHelper.ExecuteNonQueryAsync(item.Value, cancellationToken: cancellationToken);
                }
            }
        }

        private Dictionary<Type, string> GetPreparedCreateTableQueriesMapping(Type entityType)
        {
            _tableCreationMapping[entityType] = true;

            var tableCreationMapping = new Dictionary<Type, string>();
            var sql = PrepareCreateTableQuery(entityType);

            // Determines if we need to create a table for the current type and any navigation properties.
            // If the navigation properties depend on the current type, then current type should be created first;
            // otherwise, create the foreign tables sequentially
            foreach (var item in _foreignTableCreationMapping)
            {
                var requiredType = item.Value.Item2;

                if (requiredType != default(Type))
                {
                    if (_foreignTableCreationMapping.ContainsKey(requiredType))
                    {
                        var m = _foreignTableCreationMapping[requiredType];
                        var foreignRequiredType = m.Item2;
                        var foreignRequiredSql = m.Item1;

                        if (foreignRequiredType == entityType)
                            tableCreationMapping.Add(entityType, sql);

                        tableCreationMapping.Add(requiredType, foreignRequiredSql);
                    }
                    else if (requiredType == entityType && !tableCreationMapping.ContainsKey(entityType))
                    {
                        tableCreationMapping.Add(entityType, sql);
                    }
                }

                if (!tableCreationMapping.ContainsKey(item.Key))
                {
                    tableCreationMapping.Add(item.Key, item.Value.Item1);
                }
            }

            if (!tableCreationMapping.ContainsKey(entityType))
            {
                tableCreationMapping.Add(entityType, sql);
            }

            return tableCreationMapping;
        }

        private string PrepareCreateTableQuery(Type entityType)
        {
            var tableName = _conventions.GetTableName(entityType);

            // Check if has composite primary keys (more than one key), and if so, it needs to defined an ordering
            var primaryKeyPropertyInfos = _conventions.GetPrimaryKeyPropertyInfos(entityType);
            if (primaryKeyPropertyInfos.Count() > 1)
            {
                var keysHaveNoOrdering = primaryKeyPropertyInfos.Any(x => x.GetCustomAttribute<ColumnAttribute>() == null);
                if (keysHaveNoOrdering)
                    throw new InvalidOperationException(string.Format(Resources.UnableToDetermineCompositePrimaryKeyOrdering, "primary", entityType.FullName));
            }

            var properties = entityType
                .GetRuntimeProperties()
                .ToList();

            var keyMappingFromSource = GetForeignKeyPropertyInfosMapping(entityType, false);
            var navigationPropertiesInfo = properties.Where(x => x.IsComplex() || x.PropertyType.IsGenericCollection());

            // Check if has composite foreign keys (more than one key for a given navigation property), and if so, it needs to defined an ordering
            var propertiesMapping = properties
                .Where(x => x.IsPrimitive())
                .OrderBy(_conventions.GetColumnOrderOrDefault)
                .ToDictionary(_conventions.GetColumnName, x => x);

            var primaryKeyPropertyInfosMapping = _conventions.GetPrimaryKeyPropertyInfos(entityType).ToDictionary(_conventions.GetColumnName, x => x);
            var primaryKeyConstraints = new List<string>();
            var foreignKeyConstraints = new Dictionary<string, List<Tuple<string, string>>>();
            var foreignKeyOrder = 0;

            Action<Type, bool> foreignTableCreateAction = (foreignNavigationType, dependentHasPrincipal) =>
            {
                if (!_tableCreationMapping.ContainsKey(foreignNavigationType))
                {
                    _tableCreationMapping[foreignNavigationType] = true;

                    var sql = PrepareCreateTableQuery(foreignNavigationType);

                    if (dependentHasPrincipal == true)
                        _foreignTableCreationMapping[foreignNavigationType] = Tuple.Create(sql, entityType);
                    else
                        _foreignTableCreationMapping[foreignNavigationType] = Tuple.Create(sql, default(Type));
                }
            };

            Action<string, PropertyInfo, bool> foreignKeyConstraintAction = (foreignKeyName, foreignNavigationPropertyInfo, getFromForeign) =>
            {
                //var foreignNavigationPropertyInfo = keyMappingsFromForeign[foreignKeyName];
                var foreignNavigationType = foreignNavigationPropertyInfo.PropertyType;

                // The current type has the principal
                var hasPrincipal = foreignNavigationPropertyInfo.GetCustomAttribute<RequiredAttribute>() != null;

                // The dependent type has the principal
                var dependent = GetForeignKeyPropertyInfosMapping(foreignNavigationType, getFromForeign)
                    .SingleOrDefault(x => x.Key == foreignKeyName && x.Value.PropertyType == entityType)
                    .Value;

                bool? dependentHasPrincipal = null;

                if (dependent != null)
                {
                    dependentHasPrincipal = dependent.GetCustomAttribute<RequiredAttribute>() != null;

                    if (!dependentHasPrincipal == true && !hasPrincipal)
                    {
                        throw new InvalidOperationException(
                            string.Format(Resources.UnableToDeterminePrincipal, foreignNavigationType.FullName, entityType.FullName));
                    }
                }

                // Prepares the sql for the foreign table that needs to be created
                foreignTableCreateAction(foreignNavigationType, dependentHasPrincipal == true);

                if (hasPrincipal)
                {
                    var foreignNavigationTableName = _conventions.GetTableName(foreignNavigationType);
                    var foreignPrimaryKeyColumnNames = _conventions
                        .GetPrimaryKeyPropertyInfos(foreignNavigationType)
                        .Select(_conventions.GetColumnName);
                    var foreignPrimaryKeyColumnName = foreignPrimaryKeyColumnNames.ElementAt(foreignKeyOrder++);

                    if (!foreignKeyConstraints.ContainsKey(foreignNavigationTableName))
                    {
                        foreignKeyConstraints[foreignNavigationTableName] = new List<Tuple<string, string>>();
                    }

                    foreignKeyConstraints[foreignNavigationTableName].Add(Tuple.Create(foreignKeyName, foreignPrimaryKeyColumnName)); /**/
                }
            };

            var sb = new StringBuilder();

            sb.Append($"CREATE TABLE {tableName} (");

            foreach (var item in propertiesMapping)
            {
                sb.Append($"{Environment.NewLine}\t");

                // Name
                sb.Append($"{item.Key} ");

                // Type
                var sqlType = DbHelper.MapToSqlDbType(item.Value.PropertyType).ToString().ToLowerInvariant();

                if (item.Value.PropertyType == typeof(string))
                {
                    var stringLengthAttribute = item.Value.GetCustomAttribute<StringLengthAttribute>();

                    sb.Append($"{sqlType}({stringLengthAttribute?.MaximumLength ?? 4000}) ");
                }
                else
                {
                    sb.Append($"{sqlType} ");
                }

                // Constraints
                if (_conventions.IsColumnIdentity(item.Value))
                {
                    sb.Append("IDENTITY ");
                }

                var requiredAttribute = item.Value.GetCustomAttribute<RequiredAttribute>();
                if (requiredAttribute != null)
                {
                    sb.Append("NOT NULL ");
                }

                // Prepare PRIMARY/FOREIGN KEY Constraints (these constraints are added at the bottom, after all the columns are added)
                if (primaryKeyPropertyInfosMapping.ContainsKey(item.Key))
                {
                    primaryKeyConstraints.Add(item.Key);
                }
                else if (keyMappingFromSource.ContainsKey(item.Key))
                {
                    // by the column name
                    foreignKeyConstraintAction(item.Key, keyMappingFromSource[item.Key], true);
                }

                sb.Length -= 1;
                sb.Append(",");
            }

            if (primaryKeyConstraints.Count > 0)
            {
                sb.Append($"{Environment.NewLine}\tCONSTRAINT PK_{tableName} PRIMARY KEY({string.Join(", ", primaryKeyConstraints)}),");
            }

            if (!foreignKeyConstraints.Any())
            {
                var primaryKeyPropertyInfo = primaryKeyPropertyInfosMapping.First().Value;
                var primaryKeyName = _conventions.GetColumnName(primaryKeyPropertyInfo);

                var keyMappingFromForeign = GetForeignKeyPropertyInfosMapping(entityType, true);

                // check by naming convention if no foreign key column was found
                var key = $"{entityType.Name}{primaryKeyName}";
                if (keyMappingFromForeign.ContainsKey(key))
                    foreignKeyConstraintAction(key, keyMappingFromForeign[key], false);
            }

            if (foreignKeyConstraints.Count > 0)
            {
                foreach (var foreignKeyConstraint in foreignKeyConstraints)
                {
                    var foreignNavigationTableName = foreignKeyConstraint.Key;
                    var foreignNavigationTablePrimaryKeysMapping = foreignKeyConstraint.Value;
                    var foreignNavigationTablePrimaryKeyColumnNames =
                        foreignNavigationTablePrimaryKeysMapping.Select(x => x.Item2);
                    var foreignKeyColumnNames = foreignNavigationTablePrimaryKeysMapping.Select(x => x.Item1);

                    sb.Append($"{Environment.NewLine}\tCONSTRAINT FK_{foreignNavigationTableName} FOREIGN KEY({string.Join(", ", foreignKeyColumnNames)}) REFERENCES {foreignNavigationTableName}({string.Join(", ", foreignNavigationTablePrimaryKeyColumnNames)}) ");
                }
            }

            sb.Length -= 1;
            sb.Append($"{Environment.NewLine});");

            // Checks to see if there are any navigation properties that need to be created that don't have a foreign key on the current entity
            // in that case, we still want to create it them here (this is the behaviour of entity framework... kind of :)
            foreach (var propertyInfo in navigationPropertiesInfo)
            {
                var type = propertyInfo.PropertyType.IsGenericCollection()
                    ? propertyInfo.PropertyType.GetGenericArguments().First()
                    : propertyInfo.PropertyType;

                foreignTableCreateAction(type, false);
            }

            return sb.ToString();
        }

        private Dictionary<string, PropertyInfo> GetForeignKeyPropertyInfosMapping(Type entityType, bool getFromForeign)
        {
            var foreignNavigationPropertyInfos = entityType.GetRuntimeProperties().Where(x => x.IsComplex());
            var foreignKeyPropertyInfosMapping = new Dictionary<string, PropertyInfo>();

            foreach (var pi in foreignNavigationPropertyInfos)
            {
                var foreignKeyPropertyInfos = getFromForeign
                    ? _conventions.GetForeignKeyPropertyInfos(pi.PropertyType, entityType)
                    : _conventions.GetForeignKeyPropertyInfos(entityType, pi.PropertyType);

                if (foreignKeyPropertyInfos.Any())
                {
                    if (foreignKeyPropertyInfos.Count() > 1)
                    {
                        var keysHaveNoOrdering = foreignKeyPropertyInfos.Any(x => x.GetCustomAttribute<ColumnAttribute>() == null);
                        if (keysHaveNoOrdering)
                            throw new InvalidOperationException(string.Format(
                                Resources.UnableToDetermineCompositePrimaryKeyOrdering, "foreign", entityType.FullName));
                    }

                    var dict = foreignKeyPropertyInfos.ToDictionary(_conventions.GetColumnName, x => pi);

                    foreach (var item in dict)
                    {
                        foreignKeyPropertyInfosMapping.Add(item.Key, item.Value);
                    }
                }
            }

            return foreignKeyPropertyInfosMapping;
        }

        private async Task<bool> ExecuteTableExistsAsync(Type entityType, CancellationToken cancellationToken)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            var tableName = _conventions.GetTableName(entityType);
            var sql = @"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName";
            var parameters = new Dictionary<string, object> { { "@tableName", tableName } };

            using (var reader = await _dbHelper.ExecuteReaderAsync(sql, parameters, cancellationToken))
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

        #endregion
    }
}