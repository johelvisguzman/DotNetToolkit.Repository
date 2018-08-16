namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int AddressId { get; set; }
        public CustomerAddress Address { get; set; }
    }

    [Table("CustomersWithNoIdentity")]
    public class CustomerWithNoIdentity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public int AddressId { get; set; }
    }

    public class CustomerAddress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }

    public class CustomerWithTwoCompositePrimaryKey
    {
        [Key]
        public int Id1 { get; set; }
        [Key]
        public int Id2 { get; set; }
        public string Name { get; set; }
    }

    public class CustomerWithThreeCompositePrimaryKey
    {
        [Key]
        public int Id1 { get; set; }
        [Key]
        public int Id2 { get; set; }
        [Key]
        public int Id3 { get; set; }
        public string Name { get; set; }
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

    public interface IHaveTimeStamp
    {
        DateTime? CreateTime { get; set; }
        DateTime? ModTime { get; set; }
        string CreateUser { get; set; }
        string ModUser { get; set; }
    }
}
