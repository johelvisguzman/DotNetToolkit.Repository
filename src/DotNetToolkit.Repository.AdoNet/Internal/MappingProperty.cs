namespace DotNetToolkit.Repository.AdoNet.Internal
{
    using System.Reflection;
    using Utility;

    internal class MappingProperty
    {
        public string TableName { get; }
        public string TableAlias { get; }
        public string ColumnName { get; }
        public string ColumnAlias { get; }
        public PropertyInfo PropertyInfo { get; }
        public PropertyInfo JoinTablePropertyInfo { get; set; }

        public MappingProperty(string tableName, string tableAlias, string columnName, string columnAlias, PropertyInfo pi)
        {
            TableName = Guard.NotEmpty(tableName, nameof(tableName));
            TableAlias = Guard.NotEmpty(tableAlias, nameof(tableAlias));
            ColumnName = Guard.NotEmpty(columnName, nameof(columnName));
            ColumnAlias = Guard.NotEmpty(columnAlias, nameof(columnAlias));
            PropertyInfo = Guard.NotNull(pi, nameof(pi));
        }

        public override string ToString()
        {
            return $"[{TableAlias}].[{ColumnName}] AS [{ColumnAlias}]";
        }
    }
}
