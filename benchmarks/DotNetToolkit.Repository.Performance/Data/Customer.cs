namespace DotNetToolkit.Repository.Performance.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Customer : Microsoft.WindowsAzure.Storage.Table.TableEntity
    {
        private int _id;

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                RowKey = _id.ToString();
            }
        }
        public string Name { get; set; }

        // for azure table compability (do not map since they are not realted to sql tests)
        [NotMapped]
        public new DateTimeOffset Timestamp { get; set; }
        [NotMapped]
        public new string ETag { get; set; }

        public Customer()
        {
            PartitionKey = string.Empty;
        }
    }
}
