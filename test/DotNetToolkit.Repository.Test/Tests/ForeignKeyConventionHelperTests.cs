namespace DotNetToolkit.Repository.Test
{
    using Configuration.Conventions.Internal;
    using Test.Data;
    using Test.Helpers;
    using Xunit;

    public class ForeignKeyConventionHelperTests
    {
        [Fact]
        public void FindForeignKeysFromNavigationProperty()
        {
            var rightPi = ExpressionHelper.GetPropertyInfo<Customer>(x => x.Address1);
            var leftPi = ExpressionHelper.GetPropertyInfo<CustomerAddress>(x => x.Customer);

            var result = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(rightPi);

            Assert.NotNull(result);

            Assert.Equal(leftPi, result.LeftNavPi);
            Assert.Equal(nameof(Customer.Id), result.LeftKeysToJoinOn[0].Name);

            Assert.Equal(rightPi, result.RightNavPi);
            Assert.Equal(nameof(CustomerAddress.CustomerId), result.RightKeysToJoinOn[0].Name);
        }

        [Fact]
        public void FindForeignKeysFromSource()
        {
            var rightPi = ExpressionHelper.GetPropertyInfo<CustomerPhone>(x => x.Customer);
            var leftPi = ExpressionHelper.GetPropertyInfo<Customer>(x => x.Phone);

            var result = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(rightPi);

            Assert.NotNull(result);

            Assert.Equal(leftPi, result.LeftNavPi);
            Assert.Equal(nameof(CustomerPhone.CustomerId), result.LeftKeysToJoinOn[0].Name);

            Assert.Equal(rightPi, result.RightNavPi);
            Assert.Equal(nameof(CustomerPhone.Id), result.RightKeysToJoinOn[0].Name);
        }

        [Fact]
        public void FindForeignKeysWithNavigationPropertyHavingCompositeKey()
        {
            var rightPi = ExpressionHelper.GetPropertyInfo<CustomerWithThreeCompositePrimaryKey>(x => x.Address);

            var result = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(rightPi);

            var rightKeys = result.RightKeysToJoinOn;

            Assert.Equal(3, rightKeys.Length);
            Assert.Equal(nameof(CustomerAddressWithWithThreeCompositePrimaryKey.CustomerId1), rightKeys[0].Name);
            Assert.Equal(nameof(CustomerAddressWithWithThreeCompositePrimaryKey.CustomerId2), rightKeys[1].Name);
            Assert.Equal(nameof(CustomerAddressWithWithThreeCompositePrimaryKey.CustomerId3), rightKeys[2].Name);
        }

        [Fact]
        public void FindForeignKeysFromSourceWithForeignKeyAttributeOnNavigationProperty()
        {
            var rightPi = ExpressionHelper.GetPropertyInfo<CustomerWithForeignKeyAttribute>(x => x.Address);
            var leftPi = ExpressionHelper.GetPropertyInfo<CustomerAddressWithCustomerWithForeignKeyAttribute>(x => x.Customer);

            var result = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(rightPi);

            Assert.NotNull(result);

            Assert.Equal(leftPi, result.LeftNavPi);
            Assert.Equal(nameof(CustomerWithForeignKeyAttribute.AddressRefId), result.LeftKeysToJoinOn[0].Name);

            Assert.Equal(rightPi, result.RightNavPi);
            Assert.Equal(nameof(CustomerAddressWithCustomerWithForeignKeyAttribute.Id), result.RightKeysToJoinOn[0].Name);
        }

        [Fact]
        public void CannotFindForeignKeysWithoutIds()
        {
            var rightPi = ExpressionHelper.GetPropertyInfo<CustomerWithNoId>(x => x.Address);

            var result = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(rightPi);

            Assert.Null(result);
        }

        [Fact]
        public void FindForeignKeysFromSourceOneDirection()
        {
            var rightPi = ExpressionHelper.GetPropertyInfo<TableI>(x => x.TableJ);
            var result = ForeignKeyConventionHelper.GetForeignKeyPropertyInfos(rightPi);

            Assert.NotNull(result);

            Assert.Null(result.LeftNavPi);
            Assert.Null(result.LeftKeysToJoinOn);

            Assert.Equal(rightPi, result.RightNavPi);
            Assert.Equal(nameof(TableJ.Id), result.RightKeysToJoinOn[0].Name);
        }
    }
}