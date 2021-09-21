namespace DotNetToolkit.Repository.Test
{
    using Configuration.Conventions;
    using Configuration.Conventions.Internal;
    using Test.Data;
    using Test.Helpers;
    using Xunit;

    public class ForeignKeyConventionHelperTests
    {
        [Fact]
        public void FindForeignKeys()
        {
            var conventions = RepositoryConventions.Default();
            var keyInfos = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(conventions,
                ExpressionHelper.GetPropertyInfo<Customer>(x => x.Address));

            Assert.Single(keyInfos);
            Assert.Equal(nameof(CustomerAddress.CustomerId), keyInfos[0].Name);

            keyInfos = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(conventions,
                ExpressionHelper.GetPropertyInfo<Customer>(x => x.Phone));

            Assert.Single(keyInfos);
            Assert.Equal(nameof(CustomerPhone.CustomerId), keyInfos[0].Name);
        }

        [Fact]
        public void FindForeignKeysWithParentHavingCompositeKeys()
        {
            var conventions = RepositoryConventions.Default();
            var keyInfos = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(conventions,
                ExpressionHelper.GetPropertyInfo<CustomerWithThreeCompositePrimaryKey>(x => x.Address));

            Assert.Equal(3, keyInfos.Length);
            Assert.Equal(nameof(CustomerAddressWithWithThreeCompositePrimaryKey.CustomerId1), keyInfos[0].Name);
            Assert.Equal(nameof(CustomerAddressWithWithThreeCompositePrimaryKey.CustomerId2), keyInfos[1].Name);
            Assert.Equal(nameof(CustomerAddressWithWithThreeCompositePrimaryKey.CustomerId3), keyInfos[2].Name);
        }
    }
}
