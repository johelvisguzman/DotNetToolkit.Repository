namespace DotNetToolkit.Repository.Test.Data
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CustomerAddress Address { get; set; }
        public CustomerPhone Phone { get; set; }
    }

    public class CustomerPhone
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public int CustomerId { get; set; }
        [Required]
        public Customer Customer { get; set; }
    }

    public class CustomerAddress
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int CustomerId { get; set; }
        [Required]
        public Customer Customer { get; set; }
    }

    public class CustomerAddressWithWithThreeCompositePrimaryKey
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int CustomerId1 { get; set; }
        public string CustomerId2 { get; set; }
        public int CustomerId3 { get; set; }
        [Required]
        public CustomerWithThreeCompositePrimaryKey Customer { get; set; }
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
        public CustomerAddressWithWithThreeCompositePrimaryKey Address { get; set; }
    }

    public class CustomerWithNoId
    {
        public string Name { get; set; }
    }

    public class CustomerWithNavigationProperties
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
