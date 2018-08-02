namespace DotNetToolkit.Repository.AdoNet.Internal.Schema
{
    using Configuration.Conventions;
    using Helpers;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an internal schema database helper for executing operations related to editing the database/table schema.
    /// </summary>
    internal class SchemaTableConfigurationHelper
    {
        #region Fields

        private readonly AdoNetRepositoryContext _context;
        private readonly Dictionary<Type, string> _multiplicitiesMapping = new Dictionary<Type, string>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaTableConfigurationHelper"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SchemaTableConfigurationHelper(AdoNetRepositoryContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
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

            if (!ExexuteTableExists(entityType))
            {
                ExexuteTableCreate(entityType);
            }
            else
            {
                ExecuteTableValidate(entityType);
            }
        }

        /// <summary>
        /// Checks if table exists.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns><c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        public bool ExexuteTableExists<TEntity>() where TEntity : class
        {
            return ExexuteTableExists(typeof(TEntity));
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        public void ExexuteTableCreate<TEntity>() where TEntity : class
        {
            ExexuteTableCreate(typeof(TEntity));
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

            if (!await ExecuteTableExistsAsync(entityType, cancellationToken))
            {
                await ExecuteTableCreateAsync(entityType, cancellationToken);
            }
            else
            {
                await ExecuteTableValidateAsync(entityType, cancellationToken);
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

        private void ExecuteTableValidate(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            var tableName = entityType.GetTableName();
            var schemaTableColumns = GetSchemaTableColumns(tableName);

            ValidateTable(entityType, schemaTableColumns, tableName);
        }

        private async Task ExecuteTableValidateAsync(Type entityType, CancellationToken cancellationToken)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            var tableName = entityType.GetTableName();
            var schemaTableColumns = await GetSchemaTableColumnsAsync(tableName, cancellationToken);

            ValidateTable(entityType, schemaTableColumns, tableName);
        }

        private void ValidateTable(Type entityType, IEnumerable<SchemaTableColumn> schemaTableColumns, string tableName)
        {
            var error = new Dictionary<string, bool>();

            var propertiesMapping = entityType
                .GetRuntimeProperties()
                .Where(x => x.IsPrimitive())
                .OrderBy(x => x.GetColumnOrder())
                .ToDictionary(x => x.GetColumnName(), x => x);

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
            var primaryKeyPropertiesMapping = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).ToDictionary(x => x.GetColumnName(), x => x);

            // Gets all the foreign keys
            var foreignKeyPropertyInfosMapping = entityType
                .GetRuntimeProperties()
                .Where(x => x.IsComplex())
                .Select(pi => ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(entityType, pi.PropertyType)
                .ToDictionary(x => x.GetColumnName(), x => pi))
                .SelectMany(x => x)
                .ToDictionary(x => x.Key, x => x.Value);

            // Validate all the columns
            foreach (var schemaTableColumn in schemaTableColumns)
            {
                var columnName = schemaTableColumn.ColumnName;
                var propertyInfo = propertiesMapping[columnName];

                // Property type
                var columnDataType = MapToType(schemaTableColumn.DataType);
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

                            if (PrimaryKeyConventionHelper.HasCompositePrimaryKey(foreignPropertyInfo.PropertyType))
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
                        if (requiredAttribute == null)
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
                                    if (primaryKeyPropertiesMapping.ContainsKey(columnName))
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

            return _context.ExecuteList<SchemaTableColumn>(sql, parameters);
        }

        private Task<IEnumerable<SchemaTableColumn>> GetSchemaTableColumnsAsync(string tableName, CancellationToken cancellationToken)
        {
            var sql = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName";
            var parameters = new Dictionary<string, object> { { "@tableName", tableName } };

            return _context.ExecuteListAsync<SchemaTableColumn>(sql, parameters, cancellationToken);
        }

        private Dictionary<string, string> GetSchemaTableColumnConstraintsMapping(string tableName, string[] columns)
        {
            // Gets all the constraints
            var sql = @"SELECT
                            INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.COLUMN_NAME,
                            INFORMATION_SCHEMA.TABLE_CONSTRAINTS.CONSTRAINT_TYPE
                        FROM INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE
                        JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                          ON INFORMATION_SCHEMA.TABLE_CONSTRAINTS.CONSTRAINT_NAME = INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.CONSTRAINT_NAME
                        WHERE
                            INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.TABLE_NAME = @tableName AND
                            INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE.COLUMN_NAME IN (@column)";

            var parameters = new Dictionary<string, object>
            {
                {"@tableName", tableName},
                {"@column", columns}
            };

            try
            {
                return _context.ExecuteDictionary<string, string>(sql, parameters);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void ExexuteTableCreate(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            var sql = PrepareCreateTableQuery(entityType);

            _context.ExecuteNonQuery(sql);
        }

        private Task ExecuteTableCreateAsync(Type entityType, CancellationToken cancellationToken)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            var sql = PrepareCreateTableQuery(entityType);

            return _context.ExecuteNonQueryAsync(sql, cancellationToken: cancellationToken);
        }

        private string PrepareCreateTableQuery(Type entityType)
        {
            var tableName = entityType.GetTableName();

            // Check if has composite primary keys (more than one key), and if so, it needs to defined an ordering
            var primaryKeyPropertyInfos = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType);
            if (primaryKeyPropertyInfos.Count() > 1)
            {
                var keysHaveNoOrdering = primaryKeyPropertyInfos.Any(x => x.GetCustomAttribute<ColumnAttribute>() == null);
                if (keysHaveNoOrdering)
                    throw new InvalidOperationException(string.Format(Resources.UnableToDetermineCompositePrimaryKeyOrdering, "primary", entityType.FullName));
            }

            // Check if has composite foreign keys (more than one key for a given navigation property), and if so, it needs to defined an ordering
            var foreignNavigationPropertyInfos = entityType.GetRuntimeProperties().Where(x => x.IsComplex());
            var foreignKeyPropertyInfosMapping = new Dictionary<string, PropertyInfo>();

            foreach (var pi in foreignNavigationPropertyInfos)
            {
                var foreignKeyPropertyInfos = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(entityType, pi.PropertyType);
                if (foreignKeyPropertyInfos.Count() > 1)
                {
                    var keysHaveNoOrdering = foreignKeyPropertyInfos.Any(x => x.GetCustomAttribute<ColumnAttribute>() == null);
                    if (keysHaveNoOrdering)
                        throw new InvalidOperationException(string.Format(Resources.UnableToDetermineCompositePrimaryKeyOrdering, "foreign", entityType.FullName));
                }

                var dict = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(entityType, pi.PropertyType).ToDictionary(ModelConventionHelper.GetColumnName, x => pi);

                foreach (var item in dict)
                {
                    foreignKeyPropertyInfosMapping.Add(item.Key, item.Value);
                }
            }

            var propertiesMapping = entityType
                .GetRuntimeProperties()
                .Where(x => x.IsPrimitive())
                .OrderBy(x => x.GetColumnOrder())
                .ToDictionary(x => x.GetColumnName(), x => x);

            var primaryKeyPropertyInfosMapping = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(entityType).ToDictionary(x => x.GetColumnName(), x => x);
            var primaryKeyConstraints = new List<string>();
            var foreignKeyConstraints = new Dictionary<string, List<Tuple<string, string>>>();
            var foreignKeyOrder = 0;

            var sb = new StringBuilder();

            sb.Append($"CREATE TABLE {tableName} (");

            foreach (var item in propertiesMapping)
            {
                sb.Append("\n\t");

                // Name
                sb.Append($"{item.Key} ");

                // Type
                var sqlType = MapToSqlDbType(item.Value.PropertyType).ToString().ToLowerInvariant();

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
                if (item.Value.IsColumnIdentity())
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
                else if (foreignKeyPropertyInfosMapping.ContainsKey(item.Key))
                {
                    var foreignNavigationPropertyInfo = foreignKeyPropertyInfosMapping[item.Key];
                    var foreignNavigationType = foreignNavigationPropertyInfo.PropertyType;

                    if (_multiplicitiesMapping.ContainsKey(foreignNavigationType))
                        throw new InvalidOperationException(string.Format(Resources.ConflictingMultiplicities,
                            _multiplicitiesMapping[foreignNavigationType], foreignNavigationType.FullName));

                    _multiplicitiesMapping[entityType] = foreignNavigationPropertyInfo.Name;

                    // In order to do a foreign key constraint, we need to ensure that the foreign table exists
                    if (!ExexuteTableExists(foreignNavigationType))
                    {
                        ExexuteTableCreate(foreignNavigationType);
                    }

                    var foreignNavigationTableName = foreignNavigationType.GetTableName();
                    var foreignPrimaryKeyColumnNames = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(foreignNavigationType)
                        .Select(x => x.GetColumnName());
                    var foreignPrimaryKeyColumnName = foreignPrimaryKeyColumnNames.ElementAt(foreignKeyOrder++);

                    if (!foreignKeyConstraints.ContainsKey(foreignNavigationTableName))
                    {
                        foreignKeyConstraints[foreignNavigationTableName] = new List<Tuple<string, string>>();
                    }

                    foreignKeyConstraints[foreignNavigationTableName].Add(Tuple.Create(item.Key, foreignPrimaryKeyColumnName));
                }

                sb.Length -= 1;
                sb.Append(",");
            }

            if (primaryKeyConstraints.Count > 0)
            {
                sb.Append($"\n\tCONSTRAINT PK_{tableName} PRIMARY KEY({string.Join(", ", primaryKeyConstraints)}),");
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

                    sb.Append($"\n\tCONSTRAINT FK_{foreignNavigationTableName} FOREIGN KEY({string.Join(", ", foreignKeyColumnNames)}) REFERENCES {foreignNavigationTableName}({string.Join(", ", foreignNavigationTablePrimaryKeyColumnNames)}) ");
                }
            }

            sb.Length -= 1;
            sb.Append("\n)");

            return sb.ToString();
        }

        private bool ExexuteTableExists(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            var tableName = entityType.GetTableName();
            var sql = @"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName";
            var parameters = new Dictionary<string, object> { { "@tableName", tableName } };

            using (var reader = _context.ExecuteReader(sql, parameters))
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

        private async Task<bool> ExecuteTableExistsAsync(Type entityType, CancellationToken cancellationToken)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            var tableName = entityType.GetTableName();
            var sql = @"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName";
            var parameters = new Dictionary<string, object> { { "@tableName", tableName } };

            using (var reader = await _context.ExecuteReaderAsync(sql, parameters, cancellationToken))
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

        private static SqlDbType MapToSqlDbType(string sqlDataType)
        {
            return (SqlDbType)Enum.Parse(typeof(SqlDbType), sqlDataType, true);
        }

        private static Type MapToType(string sqlDataType)
        {
            return MapToType(MapToSqlDbType(sqlDataType));
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

        private static SqlDbType MapToSqlDbType(Type type)
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
    }
}
