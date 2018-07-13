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

    internal class SchemaTableHelper
    {
        #region Fields

        private readonly AdoNetRepositoryContext _context;

        #endregion

        #region Constructors

        public SchemaTableHelper(AdoNetRepositoryContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        #endregion

        #region Public Methods

        public void Validate<TEntity>() where TEntity : class
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
                throw new InvalidOperationException(string.Format(Resources.SchemaTableColumnsMismatch, _context.GetType().Name));

            // Gets all the constraints
            var schemaTableColumnConstraintsMapping = GetSchemaTableColumnConstraintsMapping(tableName, columns);

            var primaryKeyPropertiesMapping = ConventionHelper.GetPrimaryKeyPropertyInfos(entityType).ToDictionary(ConventionHelper.GetColumnName, x => x);

            // Validate all the columns
            foreach (var schemaTableColumn in schemaTableColumns)
            {
                var columnName = schemaTableColumn.ColumnName;
                var propertyInfo = propertiesMapping[columnName];
                var propertyTypeInfo = propertyInfo.PropertyType.GetTypeInfo();

                // Property type
                var columnDataType = MapToType(schemaTableColumn.DataType);
                if (columnDataType != propertyInfo.PropertyType)
                    error["PropertyType_Mismatch"] = true;

                // Property order
                if (!error.Any())
                {
                    var columnAttribute = propertyTypeInfo.GetCustomAttribute<ColumnAttribute>();
                    if (columnAttribute != null && columnAttribute.Order != 0)
                    {
                        if (schemaTableColumn.OrdinalPosition != columnAttribute.Order)
                            error["OrdinalPosition_Mismatch"] = true;
                    }
                }

                // Property constraints
                if (!error.Any())
                {
                    var canBeNull = !propertyInfo.PropertyType.IsValueType || Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null;
                    if (canBeNull && schemaTableColumn.IsNullable == "NO")
                        error["IsNullable_Mismatch"] = true;

                    if (schemaTableColumnConstraintsMapping != null && schemaTableColumnConstraintsMapping.ContainsKey(columnName))
                    {
                        var constraint = schemaTableColumnConstraintsMapping[columnName];

                        switch (constraint)
                        {
                            case SchemaTableConstraintType.NotNull:
                                {
                                    var requiredAttribute = propertyTypeInfo.GetCustomAttribute<RequiredAttribute>();
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
                    var stringLengthAttribute = propertyTypeInfo.GetCustomAttribute<StringLengthAttribute>();
                    if (stringLengthAttribute != null)
                    {
                        if (schemaTableColumn.CharacterMaximunLength != stringLengthAttribute.MaximumLength)
                            error["CharacterMaximunLength_Mismatch"] = true;
                    }
                }

                if (error.Any())
                    throw new InvalidOperationException(string.Format(Resources.SchemaTableColumnsMismatch, _context.GetType().Name));
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

        #endregion
    }
}
