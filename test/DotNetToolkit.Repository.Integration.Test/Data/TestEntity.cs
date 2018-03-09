namespace DotNetToolkit.Repository.Integration.Test.Data
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CustomerAddress Address { get; set; }
    }

    public class CustomerAddress
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public Customer Customer { get; set; }
    }
}
