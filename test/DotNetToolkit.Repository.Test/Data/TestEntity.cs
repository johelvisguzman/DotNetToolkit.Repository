namespace DotNetToolkit.Repository.Test.Data
{
    using System.ComponentModel.DataAnnotations;

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AddressId { get; set; }
        public int PhoneId { get; set; }
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
}
