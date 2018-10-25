namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Factories;

    public interface ITestCustomerService : IService<Customer> { }

    public class TestCustomerService : ServiceBase<Customer, int>, ITestCustomerService
    {
        public TestCustomerService(IUnitOfWorkFactory unitOfWorkFactory) : base(unitOfWorkFactory) { }
    }
}
