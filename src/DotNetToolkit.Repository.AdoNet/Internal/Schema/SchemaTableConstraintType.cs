namespace DotNetToolkit.Repository.AdoNet.Internal.Schema
{
    /// <summary>
    /// Represents an schema table constraint type.
    /// </summary>
    internal class SchemaTableConstraintType
    {
        public const string NotNull = "NOT NULL";
        public const string Unique = "UNIQUE";
        public const string PrimaryKey = "PRIMARY KEY";
        public const string ForeignKey = "FOREIGN KEY";
        public const string Check = "CHECK";
        public const string Default = "DEFAULT ";
        public const string Index = "INDEX";
    }
}
