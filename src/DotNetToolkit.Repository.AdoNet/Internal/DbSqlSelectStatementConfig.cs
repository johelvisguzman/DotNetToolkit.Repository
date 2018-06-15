namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using Helpers;
    using Properties;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    internal class DbSqlSelectStatementConfig
    {
        #region Fields

        private int _tableAliasCount = 1;

        private readonly Dictionary<string, string> _tableAliasMapping = new Dictionary<string, string>();
        private readonly Dictionary<string, Type> _tableNameAndTypeMapping = new Dictionary<string, Type>();
        private readonly Dictionary<Type, string> _tableTypeAndNameMapping = new Dictionary<Type, string>();
        private readonly Dictionary<string, Dictionary<string, string>> _tableColumnAliasMapping = new Dictionary<string, Dictionary<string, string>>();
        private readonly Dictionary<string, string> _columnAliasTableNameMapping = new Dictionary<string, string>();
        private readonly Dictionary<string, int> _columnAliasMappingCount = new Dictionary<string, int>();

        #endregion

        #region Constructors

        public DbSqlSelectStatementConfig()
        {
            JoinTablePropertiesMapping = new Dictionary<Type, Dictionary<string, PropertyInfo>>();
        }

        #endregion

        #region Properties

        public string Sql { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public Dictionary<Type, Dictionary<string, PropertyInfo>> JoinTablePropertiesMapping { get; set; }

        #endregion

        #region Public Methods

        public string GenerateTableAlias(Type tableType)
        {
            var tableAlias = $"Extent{_tableAliasCount++}";
            var tableName = ConventionHelper.GetTableName(tableType);

            _tableAliasMapping.Add(tableName, tableAlias);
            _tableNameAndTypeMapping.Add(tableName, tableType);
            _tableTypeAndNameMapping.Add(tableType, tableName);
            _tableColumnAliasMapping[tableName] = new Dictionary<string, string>();

            return tableAlias;
        }

        public string GenerateColumnAlias(PropertyInfo pi)
        {
            var columnName = ConventionHelper.GetColumnName(pi);
            var columnAlias = columnName;
            var tableName = ConventionHelper.GetTableName(pi.DeclaringType);

            if (_columnAliasMappingCount.TryGetValue(columnName, out int columnAliasCount))
            {
                ++columnAliasCount;
                columnAlias = $"{columnAlias}{columnAliasCount}";

                _columnAliasMappingCount[columnName] = columnAliasCount;
            }
            else
            {
                _columnAliasMappingCount.Add(columnName, 0);
            }

            _tableColumnAliasMapping[tableName][columnName] = columnAlias;

            _columnAliasTableNameMapping[columnAlias] = tableName;

            return columnAlias;
        }

        public string GetTableAlias(string tableName)
        {
            return _tableAliasMapping[tableName];
        }

        public string GetTableName(string tableAlias)
        {
            return !_tableAliasMapping.ContainsValue(tableAlias)
                ? null
                : _tableAliasMapping.SingleOrDefault(x => x.Value == tableAlias).Key;
        }

        public string GetTableName(Type tableType)
        {
            return _tableTypeAndNameMapping[tableType];
        }

        public string GetColumnAlias(PropertyInfo pi)
        {
            var columnName = ConventionHelper.GetColumnName(pi);
            var tableName = ConventionHelper.GetTableName(pi.DeclaringType);
            var columnMapping = _tableColumnAliasMapping[tableName];

            if (!columnMapping.ContainsKey(columnName))
                throw new InvalidOperationException(string.Format(Resources.InvalidColumnName, columnName));

            return columnMapping[columnName];
        }

        public string GetColumnName(string columnAlias)
        {
            return Regex.Replace(columnAlias, @"[\d-]", string.Empty);
        }

        public Type GetTableTypeByColumnAlias(string columnAlias)
        {
            var tableName = _columnAliasTableNameMapping[columnAlias];

            return _tableNameAndTypeMapping[tableName];
        }

        #endregion
    }
}
