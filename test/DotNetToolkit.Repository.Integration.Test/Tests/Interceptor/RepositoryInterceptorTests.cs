namespace DotNetToolkit.Repository.Integration.Test.Interceptor
{
    using Configuration.Interceptors;
    using Configuration.Options;
    using Data;
    using InMemory;
    using Moq;
    using System.Threading;
    using System.Threading.Tasks;
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

        [Fact]
        public void CantModifyUserOnInterceptionsWhenDisabled()
        {
            const string user = "Random User";

            var entity = new CustomerWithTimeStamp();
            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase()
                .UseInterceptor(new TestRepositoryTimeStampInterceptor(user))
                .Options;

            var repo = new Repository<CustomerWithTimeStamp>(options)
            {
                InterceptorsEnabled = false
            };

            Assert.Null(entity.CreateTime);
            Assert.Null(entity.ModTime);
            Assert.Null(entity.CreateUser);
            Assert.Null(entity.ModUser);

            repo.Add(entity);

            Assert.Null(entity.CreateTime);
            Assert.Null(entity.ModTime);
            Assert.Null(entity.CreateUser);
            Assert.Null(entity.ModUser);
        }

        [Fact]
        public void CantModifyUserOnInterceptionsWhenDisabledByType()
        {
            const string user = "Random User";

            var entity = new CustomerWithTimeStamp();
            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase()
                .UseInterceptor(new TestRepositoryTimeStampInterceptor(user))
                .Options;

            var repo = new Repository<CustomerWithTimeStamp>(options)
            {
                InterceptorsEnabled = true
            };

            repo.InterceptorTypesDisabled.Add(typeof(TestRepositoryTimeStampInterceptor), true);

            Assert.Null(entity.CreateTime);
            Assert.Null(entity.ModTime);
            Assert.Null(entity.CreateUser);
            Assert.Null(entity.ModUser);

            repo.Add(entity);

            Assert.Null(entity.CreateTime);
            Assert.Null(entity.ModTime);
            Assert.Null(entity.CreateUser);
            Assert.Null(entity.ModUser);
        }

        [Fact]
        public async Task AddAsync()
        {
            var entity = new Customer();
            var mock = new Mock<IRepositoryInterceptor>();

            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase()
                .UseInterceptor(mock.Object)
                .Options;

            var repo = new Repository<Customer>(options);

            mock.Verify(x => x.AddExecutingAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);

            await repo.AddAsync(entity);

            mock.Verify(x => x.AddExecutingAsync(It.Is<Customer>(z => z == entity), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync()
        {
            var entity = new Customer();
            var mock = new Mock<IRepositoryInterceptor>();

            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase()
                .UseInterceptor(mock.Object)
                .Options;

            var repo = new Repository<Customer>(options);

            await repo.AddAsync(entity);

            mock.Verify(x => x.UpdateExecutingAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);

            await repo.UpdateAsync(entity);

            mock.Verify(x => x.UpdateExecutingAsync(It.Is<Customer>(z => z == entity), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync()
        {
            var entity = new Customer();
            var mock = new Mock<IRepositoryInterceptor>();

            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase()
                .UseInterceptor(mock.Object)
                .Options;

            var repo = new Repository<Customer>(options);

            await repo.AddAsync(entity);

            mock.Verify(x => x.DeleteExecutingAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);

            await repo.DeleteAsync(entity);

            mock.Verify(x => x.DeleteExecutingAsync(It.Is<Customer>(z => z == entity), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CanModifyUserOnInterceptionsAsync()
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

            await repo.AddAsync(entity);

            Assert.NotNull(entity.CreateTime);
            Assert.NotNull(entity.ModTime);
            Assert.Equal(user, entity.CreateUser);
            Assert.Equal(user, entity.ModUser);
        }

        [Fact]
        public async Task CantModifyUserOnInterceptionsWhenDisabledAsync()
        {
            const string user = "Random User";

            var entity = new CustomerWithTimeStamp();
            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase()
                .UseInterceptor(new TestRepositoryTimeStampInterceptor(user))
                .Options;

            var repo = new Repository<CustomerWithTimeStamp>(options)
            {
                InterceptorsEnabled = false
            };

            Assert.Null(entity.CreateTime);
            Assert.Null(entity.ModTime);
            Assert.Null(entity.CreateUser);
            Assert.Null(entity.ModUser);

            await repo.AddAsync(entity);

            Assert.Null(entity.CreateTime);
            Assert.Null(entity.ModTime);
            Assert.Null(entity.CreateUser);
            Assert.Null(entity.ModUser);
        }

        [Fact]
        public async Task CantModifyUserOnInterceptionsWhenDisabledByTypeAsync()
        {
            const string user = "Random User";

            var entity = new CustomerWithTimeStamp();
            var options = new RepositoryOptionsBuilder()
                .UseInMemoryDatabase()
                .UseInterceptor(new TestRepositoryTimeStampInterceptor(user))
                .Options;

            var repo = new Repository<CustomerWithTimeStamp>(options)
            {
                InterceptorsEnabled = true
            };

            repo.InterceptorTypesDisabled.Add(typeof(TestRepositoryTimeStampInterceptor), true);

            Assert.Null(entity.CreateTime);
            Assert.Null(entity.ModTime);
            Assert.Null(entity.CreateUser);
            Assert.Null(entity.ModUser);

            await repo.AddAsync(entity);

            Assert.Null(entity.CreateTime);
            Assert.Null(entity.ModTime);
            Assert.Null(entity.CreateUser);
            Assert.Null(entity.ModUser);
        }
    }
}