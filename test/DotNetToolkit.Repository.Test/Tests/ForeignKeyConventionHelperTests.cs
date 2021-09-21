namespace DotNetToolkit.Repository.Test.Tests
{
    using Configuration.Conventions.Internal;
    using Configuration.Conventions;
    using Test.Data;
    using Xunit;

    public class ForeignKeyConventionHelperTests
    {
        [Fact]
        public void FindForeignKeys()
        {
            var conventions = RepositoryConventions.Default();
            var keyInfos = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(conventions, typeof(CustomerAddress), typeof(Customer));

            Assert.Single(keyInfos);
            Assert.Equal(nameof(CustomerAddress.CustomerId), keyInfos[0].Name);

            keyInfos = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(conventions, typeof(CustomerPhone), typeof(Customer));

            Assert.Single(keyInfos);
            Assert.Equal(nameof(CustomerPhone.CustomerId), keyInfos[0].Name);
        }

        [Fact]
        public void CannotFindForeignKeysWhenEntityHasNoNavigationProperties()
        {
            var conventions = RepositoryConventions.Default();
            var keyInfos = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(conventions, typeof(CustomerAddress), typeof(CustomerWithNavigationProperties));

            Assert.Empty(keyInfos);

            keyInfos = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(conventions, typeof(CustomerPhone), typeof(CustomerWithNavigationProperties));

            Assert.Empty(keyInfos);
        }
    }
}
