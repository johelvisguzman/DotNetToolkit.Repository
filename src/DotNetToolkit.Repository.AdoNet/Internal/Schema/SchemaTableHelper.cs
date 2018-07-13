namespace DotNetToolkit.Repository.AdoNet.Internal.Schema
{
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

    /// <summary>
    /// Represents an internal schema database helper for executing operations related to editing the database/table schema.
    /// </summary>
    internal class SchemaTableHelper
    {
        #region Fields

        private readonly AdoNetRepositoryContext _context;
        private Dictionary<Type, string> _multiplicitiesMapping;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaTableHelper"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public SchemaTableHelper(AdoNetRepositoryContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        public void Initialize<TEntity>() where TEntity : class
        {
            if (!CheckIfTableExists<TEntity>())
            {
                CreateTable<TEntity>();
            }
            else
            {
                ValidateTable<TEntity>();
            }
        }

        /// <summary>
        /// Checks if table exists.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns><c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
        public bool CheckIfTableExists<TEntity>() where TEntity : class
        {
            return CheckIfTableExists(typeof(TEntity));
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        public void CreateTable<TEntity>() where TEntity : class
        {
            _multiplicitiesMapping = new Dictionary<Type, string>();

            CreateTable(typeof(TEntity));

            _multiplicitiesMapping.Clear();
        }

        /// <summary>
        /// Validates the table.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        public void ValidateTable<TEntity>() where TEntity : class
        {
            var error = new Dictionary<string, bool>();
            var entityType = typeof(TEntity);
            var tableName = ConventionHelper.GetTableName(entityType);

            var schemaTableColumns = GetSchemaTableColumns(tableName);

            var propertiesMapping = entityType
                .GetRuntimeProperties()
                .Where(ConventionHelper.IsPrimitive)
                .OrderBy(ConventionHelper.GetColumnOrder)
                .ToDictionary(ConventionHelper.GetColumnName, x => x);

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

            var primaryKeyPropertiesMapping = ConventionHelper.GetPrimaryKeyPropertyInfos(entityType).ToDictionary(ConventionHelper.GetColumnName, x => x);

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
                        if (schemaTableColumn.OrdinalPosition != columnAttribute.Order)
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

                    if (schemaTableColumnConstraintsMapping != null && schemaTableColumnConstraintsMapping.ContainsKey(columnName))
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

        #endregion

        #region Private Methods

        private IEnumerable<SchemaTableColumn> GetSchemaTableColumns(string tableName)
        {
            var sql = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName";
            var parameters = new Dictionary<string, object> { { "@tableName", tableName } };

            return _context.ExecuteList<SchemaTableColumn>(sql, parameters);
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

        private void CreateTable(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            var tableName = ConventionHelper.GetTableName(entityType);

            var propertiesMapping = entityType
                .GetRuntimeProperties()
                .Where(ConventionHelper.IsPrimitive)
                .OrderBy(ConventionHelper.GetColumnOrder)
                .ToDictionary(ConventionHelper.GetColumnName, x => x);

            var primaryKeyPropertyInfosMapping = ConventionHelper.GetPrimaryKeyPropertyInfos(entityType)
                .ToDictionary(ConventionHelper.GetColumnName, x => x);
            var foreignKeyPropertynfosMapping = ConventionHelper.GetForeignKeyPropertiesMapping(entityType);
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
                if (ConventionHelper.IsIdentity(item.Value))
                {
                    sb.Append("IDENTITY ");
                }

                var requiredAttribute = item.Value.GetCustomAttribute<RequiredAttribute>();
                if (requiredAttribute != null)
                {
                    sb.Append("NOT NULL ");
                }

                // Prepare PRIMARY/FOREIGN KEY Constraints
                if (primaryKeyPropertyInfosMapping.ContainsKey(item.Key))
                {
                    primaryKeyConstraints.Add(item.Key);
                }
                else if (foreignKeyPropertynfosMapping.ContainsKey(item.Key))
                {
                    var foreignNavigationPropertyInfo = foreignKeyPropertynfosMapping[item.Key];
                    var foreignNavigationType = foreignNavigationPropertyInfo.PropertyType;

                    if (_multiplicitiesMapping.ContainsKey(foreignNavigationType))
                        throw new InvalidOperationException(string.Format(Resources.ConflictingMultiplicities, _multiplicitiesMapping[foreignNavigationType], foreignNavigationType.FullName));

                    _multiplicitiesMapping[entityType] = foreignNavigationPropertyInfo.Name;

                    // In order to do a foreign key constraint, we need to ensure that the foreign table exists
                    if (!CheckIfTableExists(foreignNavigationType))
                    {
                        CreateTable(foreignNavigationType);
                    }

                    var foreignNavigationTableName = ConventionHelper.GetTableName(foreignNavigationType);
                    var foreignPrimaryKeyColumnNames = ConventionHelper.GetPrimaryKeyPropertyInfos(foreignNavigationType).Select(ConventionHelper.GetColumnName);
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
                    var foreignNavigationTablePrimaryKeyColumnNames = foreignNavigationTablePrimaryKeysMapping.Select(x => x.Item2);
                    var foreignKeyColumnNames = foreignNavigationTablePrimaryKeysMapping.Select(x => x.Item1);

                    sb.Append($"\n\tCONSTRAINT FK_{foreignNavigationTableName} FOREIGN KEY({string.Join(", ", foreignKeyColumnNames)}) REFERENCES {foreignNavigationTableName}({string.Join(", ", foreignNavigationTablePrimaryKeyColumnNames)}) ");
                }
            }

            sb.Length -= 1;
            sb.Append("\n)");

            _context.ExecuteNonQuery(sb.ToString());
        }

        private bool CheckIfTableExists(Type entityType)
        {
            if (entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            var tableName = ConventionHelper.GetTableName(entityType);
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
