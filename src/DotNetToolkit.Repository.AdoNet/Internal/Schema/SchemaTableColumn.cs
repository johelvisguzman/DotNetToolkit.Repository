namespace DotNetToolkit.Repository.AdoNet.Internal.Schema
{
    using System.ComponentModel.DataAnnotations.Schema;

    // https://www.mssqltips.com/sqlservertutorial/183/informationschemacolumns/

    [Table("[INFORMATION_SCHEMA].[COLUMNS]")]
    internal class SchemaTableColumn
    {
        [Column("TABLE_CATALOG")]
        public string TableCatalog { get; set; }

        [Column("TABLE_SCHEMA")]
        public string TableSchema { get; set; }

        [Column("TABLE_NAME")]
        public string TableName { get; set; }

        [Column("COLUMN_NAME")]
        public string ColumnName { get; set; }

        [Column("ORDINAL_POSITION")]
        public int OrdinalPosition { get; set; }

        [Column("COLUMN_DEFAULT")]
        public string ColumnDefault { get; set; }

        [Column("IS_NULLABLE")]
        public string IsNullable { get; set; }

        [Column("DATA_TYPE")]
        public string DataType { get; set; }

        [Column("CHARACTER_MAXIMUM_LENGTH")]
        public int CharacterMaximunLength { get; set; }

        [Column("CHARACTER_OCTET_LENGTH")]
        public int CharacterOctetLength { get; set; }

        [Column("NUMERIC_PRECISION")]
        public short NumericPrecision { get; set; }

        [Column("NUMERIC_PRECISION_RADIX")]
        public short NumericPrecisionRadix { get; set; }

        [Column("NUMERIC_SCALE")]
        public int? NumericScale { get; set; }

        [Column("DATETIME_PRECISION")]
        public int? DateTimePrecision { get; set; }
    }
}
