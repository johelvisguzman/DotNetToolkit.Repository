namespace DotNetToolkit.Repository.Performance.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

#if NETFULL
    public class Customer : Microsoft.WindowsAzure.Storage.Table.TableEntity
#else
    public class Customer
#endif
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
#if NETFULL
                RowKey = _id.ToString(); 
#endif
            }
        }
        public string Name { get; set; }

        // for azure table compability (do not map since they are not realted to sql tests)
        [NotMapped]
#if NETFULL
        public new DateTimeOffset Timestamp { get; set; }
#else
        public DateTimeOffset Timestamp { get; set; }
#endif
        [NotMapped]
#if NETFULL
        public new string ETag { get; set; }
#else
        public string ETag { get; set; }
#endif

        public Customer()
        {
#if NETFULL
            PartitionKey = string.Empty; 
#endif
        }
    }
}
