namespace DotNetToolkit.Repository.Integration.Test
{
    using Configuration.Interceptors;
    using Configuration.Options;
    using Data;
    using InMemory;
    using Moq;
    using Xunit;
    using Xunit.Abstractions;

    [Collection("Sequential")]
    public class RepositoryInterceptorTests : TestBase
    {
        public RepositoryInterceptorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) { }

        [Fact]
        public void Add()
        {
            var entity = new Customer();
            var mock = new Mock<IRepositoryInterceptor>();

            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase()
                .UseInterceptor(mock.Object)
                .Options;

            var repo = new Repository<Customer>(options);

            mock.Verify(x => x.AddExecuting(It.IsAny<Customer>()), Times.Never);

            repo.Add(entity);

            mock.Verify(x => x.AddExecuting(It.Is<Customer>(z => z == entity)), Times.Once);
        }

        [Fact]
        public void Update()
        {
            var entity = new Customer();
            var mock = new Mock<IRepositoryInterceptor>();

            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase()
                .UseInterceptor(mock.Object)
                .Options;

            var repo = new Repository<Customer>(options);

            repo.Add(entity);

            mock.Verify(x => x.UpdateExecuting(It.IsAny<Customer>()), Times.Never);

            repo.Update(entity);

            mock.Verify(x => x.UpdateExecuting(It.Is<Customer>(z => z == entity)), Times.Once);
        }

        [Fact]
        public void Delete()
        {
            var entity = new Customer();
            var mock = new Mock<IRepositoryInterceptor>();

            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase()
                .UseInterceptor(mock.Object)
                .Options;

            var repo = new Repository<Customer>(options);

            repo.Add(entity);

            mock.Verify(x => x.DeleteExecuting(It.IsAny<Customer>()), Times.Never);

            repo.Delete(entity);

            mock.Verify(x => x.DeleteExecuting(It.Is<Customer>(z => z == entity)), Times.Once);
        }

        [Fact]
        public void CanModifyUserOnInterceptions()
        {
            const string user = "Random User";

            var entity = new CustomerWithTimeStamp();
            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase()
                .UseInterceptor(new TestRepositoryTimeStampInterceptor(user))
                .Options;

            var repo = new Repository<CustomerWithTimeStamp>(options);

            Assert.Null(entity.CreateTime);
            Assert.Null(entity.ModTime);
            Assert.Null(entity.CreateUser);
            Assert.Null(entity.ModUser);

            repo.Add(entity);

            Assert.NotNull(entity.CreateTime);
            Assert.NotNull(entity.ModTime);
            Assert.Equal(user, entity.CreateUser);
            Assert.Equal(user, entity.ModUser);
        }
    }
}
