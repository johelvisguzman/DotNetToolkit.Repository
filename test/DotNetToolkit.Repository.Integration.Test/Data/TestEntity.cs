namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using System;

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AddressId { get; set; }
        public CustomerAddress Address { get; set; }
    }

    public class CustomerAddress
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }

    public class CustomerWithGuidId
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
    }
    
    public class CustomerWithStringId
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
    }

    public class CustomerWithNoId
    {
        public string Name { get; set; }
    }
}
