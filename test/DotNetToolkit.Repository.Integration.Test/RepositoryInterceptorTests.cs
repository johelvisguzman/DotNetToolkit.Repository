namespace DotNetToolkit.Repository.Integration.Test
{
    using Data;
    using InMemory;
    using Interceptors;
    using Moq;
    using System.Collections.Generic;
    using Xunit;

    public class RepositoryInterceptorTests
    {
        [Fact]
        public void Add()
        {
            var entity = new Customer();
            var mock = new Mock<IRepositoryInterceptor>();
            var interceptors = new List<IRepositoryInterceptor> { mock.Object };

            var repo = new InMemoryRepository<Customer>(interceptors);

            mock.Verify(x => x.AddExecuting(It.IsAny<Customer>()), Times.Never);
            mock.Verify(x => x.AddExecuted(It.IsAny<Customer>()), Times.Never);

            repo.Add(entity);

            mock.Verify(x => x.AddExecuting(It.Is<Customer>(z => z == entity)), Times.Once);
            mock.Verify(x => x.AddExecuted(It.Is<Customer>(z => z == entity)), Times.Once);
        }

        [Fact]
        public void Update()
        {
            var entity = new Customer();
            var mock = new Mock<IRepositoryInterceptor>();
            var interceptors = new List<IRepositoryInterceptor> { mock.Object };

            var repo = new InMemoryRepository<Customer>(interceptors);

            repo.Add(entity);

            mock.Verify(x => x.UpdateExecuting(It.IsAny<Customer>()), Times.Never);
            mock.Verify(x => x.UpdateExecuted(It.IsAny<Customer>()), Times.Never);

            repo.Update(entity);

            mock.Verify(x => x.UpdateExecuting(It.Is<Customer>(z => z == entity)), Times.Once);
            mock.Verify(x => x.UpdateExecuted(It.Is<Customer>(z => z == entity)), Times.Once);
        }

        [Fact]
        public void Delete()
        {
            var entity = new Customer();
            var mock = new Mock<IRepositoryInterceptor>();
            var interceptors = new List<IRepositoryInterceptor> { mock.Object };

            var repo = new InMemoryRepository<Customer>(interceptors);

            repo.Add(entity);

            mock.Verify(x => x.DeleteExecuting(It.IsAny<Customer>()), Times.Never);
            mock.Verify(x => x.DeleteExecuted(It.IsAny<Customer>()), Times.Never);

            repo.Delete(entity);

            mock.Verify(x => x.DeleteExecuting(It.Is<Customer>(z => z == entity)), Times.Once);
            mock.Verify(x => x.DeleteExecuted(It.Is<Customer>(z => z == entity)), Times.Once);
        }

        [Fact]
        public void CanModifyUserOnInterceptions()
        {
            const string user = "Random User";

            var interceptors = new List<IRepositoryInterceptor> { new TestRepositoryTimeStampInterceptor(user) };
            var repo = new InMemoryRepository<CustomerWithTimeStamp>(interceptors);
            var entity = new CustomerWithTimeStamp();

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
