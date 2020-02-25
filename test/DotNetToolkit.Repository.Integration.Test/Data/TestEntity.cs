namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using System;
    using System.Collections.Generic;
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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public CustomerAddress Address { get; set; }

#if NETFULL
        // for azure table compability (do not map since DateTimeOffset is not supported for sql)
        [NotMapped]
        public new DateTimeOffset Timestamp { get; set; }

        public Customer()
        {
            PartitionKey = string.Empty;
        }
#endif
    }

    [Table("CustomersWithNoIdentity")]
    public class CustomerWithNoIdentity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CustomerAddress
    {
        [Key]
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int CustomerId { get; set; }
        [Required]
        public Customer Customer { get; set; }
    }

    public class CustomerWithThreeCompositePrimaryKeyAndNoOrder
    {
        [Key]
        public int Id1 { get; set; }
        [Key]
        public string Id2 { get; set; }
        [Key]
        public int Id3 { get; set; }
        public string Name { get; set; }
    }

#if NETFULL
    public class CustomerWithTwoCompositePrimaryKey : Microsoft.WindowsAzure.Storage.Table.TableEntity
#else
    public class CustomerWithTwoCompositePrimaryKey
#endif
    {
        private int _id1;
        private string _id2;

        [Key]
        [Column(Order = 1)]
        public int Id1
        {
            get { return _id1; }
            set
            {
                _id1 = value;
#if NETFULL
                PartitionKey = _id1.ToString();
#endif
            }
        }
        [Key]
        [Column(Order = 2)]
        public string Id2
        {
            get { return _id2; }
            set
            {
                _id2 = value;
#if NETFULL
                RowKey = _id2;
#endif
            }
        }
        public string Name { get; set; }

#if NETFULL
        // for azure table compability (do not map since DateTimeOffset is not supported for sql)
        [NotMapped]
        public new DateTimeOffset Timestamp { get; set; }
#endif

        public override int GetHashCode()
        {
            int hashCode = 0;

            hashCode = hashCode ^ Id1.GetHashCode() ^ Id2.GetHashCode();

            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var toCompare = obj as CustomerWithTwoCompositePrimaryKey;
            if (toCompare == null)
            {
                return false;
            }
            return (this.GetHashCode() != toCompare.GetHashCode());
        }
    }

    public class CustomerWithThreeCompositePrimaryKey
    {
        [Key]
        [Column(Order = 1)]
        public int Id1 { get; set; }
        [Key]
        [Column(Order = 2)]
        public string Id2 { get; set; }
        [Key]
        [Column(Order = 3)]
        public int Id3 { get; set; }
        public string Name { get; set; }

        public override int GetHashCode()
        {
            int hashCode = 0;

            hashCode = hashCode ^ Id1.GetHashCode() ^ Id2.GetHashCode() ^ Id3.GetHashCode();

            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var toCompare = obj as CustomerWithThreeCompositePrimaryKey;
            if (toCompare == null)
            {
                return false;
            }
            return (this.GetHashCode() != toCompare.GetHashCode());
        }
    }

    public class CustomerWithTimeStamp : IHaveTimeStamp
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModTime { get; set; }
        public string CreateUser { get; set; }
        public string ModUser { get; set; }
    }

    public class CustomerWithNoId
    {
        public string Name { get; set; }
    }

    [Table("Customers")]
    public class CustomerWithCompositeAddress
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CustomerCompositeAddress Address { get; set; }
    }

    [Table("CustomerCompositeAddresses")]
    public class CustomerCompositeAddress
    {
        [Key]
        [Column(Order = 1)]
        public int Id { get; set; }
        [Key]
        [Column(Order = 2)]
        public int CustomerId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        [Required]
        public CustomerWithCompositeAddress Customer { get; set; }
    }

    public class CustomerWithMultipleAddresses
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<CustomerAddressWithMultipleAddresses> Addresses { get; set; }
    }

    public class CustomerAddressWithMultipleAddresses
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int CustomerId { get; set; }
        public CustomerWithMultipleAddresses Customer { get; set; }
    }

    public interface IHaveTimeStamp
    {
        DateTime? CreateTime { get; set; }
        DateTime? ModTime { get; set; }
        string CreateUser { get; set; }
        string ModUser { get; set; }
    }
}