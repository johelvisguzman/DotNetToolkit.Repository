namespace DotNetToolkit.Repository.Test
{
    using Configuration.Options;
    using Data;
    using Factories;
    using Repository;
    using Services;
    using System;
    using Transactions;
    using Xunit;

    public class DependencyResolverTests
    {
        [Fact]
        public void SetCurrentResolver()
        {
            RepositoryDependencyResolver.SetResolver(new RepositoryDependencyResolver.DefaultDependencyResolver());

            Assert.Equal(typeof(RepositoryDependencyResolver.DefaultDependencyResolver), RepositoryDependencyResolver.Current.GetType());
        }

        [Fact]
        public void ResolveWithDefaultResolver()
        {
            RepositoryDependencyResolver.SetResolver(new RepositoryDependencyResolver.DefaultDependencyResolver());

            Assert.NotNull(RepositoryDependencyResolver.Current.Resolve<ClassToResolve>());
            Assert.NotNull((ClassToResolve)RepositoryDependencyResolver.Current.Resolve(typeof(ClassToResolve)));
        }

        [Fact]
        public void ResolveWithDelegateResolver()
        {
            RepositoryDependencyResolver.SetResolver(type => new ClassToResolve());

            Assert.NotNull(RepositoryDependencyResolver.Current.Resolve<ClassToResolve>());
            Assert.NotNull((ClassToResolve)RepositoryDependencyResolver.Current.Resolve(typeof(ClassToResolve)));
        }

        [Fact]
        public void ThrowsIfDefaultResolverResolvesInterfaces()
        {
            RepositoryDependencyResolver.SetResolver(new RepositoryDependencyResolver.DefaultDependencyResolver());

            var ex = Assert.Throws<InvalidOperationException>(() => RepositoryDependencyResolver.Current.Resolve<InterfaceToResolve>());
            Assert.Equal($"Unable to resolve an instance for '{typeof(InterfaceToResolve).FullName}'. Please consider using the RepositoryDependencyResolver to use an IOC container.", ex.Message);
        }

        [Fact]
        public void ThrowsIfDefaultResolverResolvesAbstractClasses()
        {
            RepositoryDependencyResolver.SetResolver(new RepositoryDependencyResolver.DefaultDependencyResolver());

            var ex = Assert.Throws<InvalidOperationException>(() => RepositoryDependencyResolver.Current.Resolve<AbstractClassToResolve>());
            Assert.Equal($"Unable to resolve an instance for '{typeof(AbstractClassToResolve).FullName}'. Please consider using the RepositoryDependencyResolver to use an IOC container.", ex.Message);
        }

        [Fact]
        public void ThrowsIfRepositoryCreateWithDefaultDependencyResolverForResolvingOptions()
        {
            RepositoryDependencyResolver.SetResolver(new RepositoryDependencyResolver.DefaultDependencyResolver());

            var ex = Assert.Throws<InvalidOperationException>(() => new Repository<Customer>());
            Assert.Equal($"Unable to resolve an instance for '{typeof(IRepositoryOptions).FullName}'. Please consider using the RepositoryDependencyResolver to use an IOC container.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => new Repository<Customer, int>());
            Assert.Equal($"Unable to resolve an instance for '{typeof(IRepositoryOptions).FullName}'. Please consider using the RepositoryDependencyResolver to use an IOC container.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => new Repository<Customer, int, int>());
            Assert.Equal($"Unable to resolve an instance for '{typeof(IRepositoryOptions).FullName}'. Please consider using the RepositoryDependencyResolver to use an IOC container.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => new Repository<Customer, int, int, int>());
            Assert.Equal($"Unable to resolve an instance for '{typeof(IRepositoryOptions).FullName}'. Please consider using the RepositoryDependencyResolver to use an IOC container.", ex.Message);
        }

        [Fact]
        public void ThrowsIfServiceCreateWithDefaultDependencyResolverForResolvingOptions()
        {
            RepositoryDependencyResolver.SetResolver(new RepositoryDependencyResolver.DefaultDependencyResolver());

            var ex = Assert.Throws<InvalidOperationException>(() => new Service<Customer>());
            Assert.Equal($"Unable to resolve an instance for '{typeof(IUnitOfWorkFactory).FullName}'. Please consider using the RepositoryDependencyResolver to use an IOC container.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => new Service<Customer, int>());
            Assert.Equal($"Unable to resolve an instance for '{typeof(IUnitOfWorkFactory).FullName}'. Please consider using the RepositoryDependencyResolver to use an IOC container.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => new Service<Customer, int, int>());
            Assert.Equal($"Unable to resolve an instance for '{typeof(IUnitOfWorkFactory).FullName}'. Please consider using the RepositoryDependencyResolver to use an IOC container.", ex.Message);

            ex = Assert.Throws<InvalidOperationException>(() => new Service<Customer, int, int, int>());
            Assert.Equal($"Unable to resolve an instance for '{typeof(IUnitOfWorkFactory).FullName}'. Please consider using the RepositoryDependencyResolver to use an IOC container.", ex.Message);
        }

        class ClassToResolve
        {
            public ClassToResolve() { }
        }

        abstract class AbstractClassToResolve { }

        interface InterfaceToResolve { }
    }
}
