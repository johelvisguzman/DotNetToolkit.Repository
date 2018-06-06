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

    public class CustomerWithTimeStamp : IHaveTimeStamp
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModTime { get; set; }
        public string CreateUser { get; set; }
        public string ModUser { get; set; }
    }

    public interface IHaveTimeStamp
    {
        DateTime? CreateTime { get; set; }
        DateTime? ModTime { get; set; }
        string CreateUser { get; set; }
        string ModUser { get; set; }
    }
}
