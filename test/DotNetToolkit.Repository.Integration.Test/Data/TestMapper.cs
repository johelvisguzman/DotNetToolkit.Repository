namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Configuration.Mapper;
    using System.Data;

    public class TestCustomerMapper : IMapper<Customer>
    {
        public Customer Map(IDataReader reader)
        {
            return new Customer()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            };
        }
    }
}
