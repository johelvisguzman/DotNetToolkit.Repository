namespace DotNetToolkit.Repository.Test.Tests
{
    using Configuration.Conventions.Internal;
    using Test.Data;
    using Xunit;

    public class PrimaryKeyConventionHelperTests
    {
        [Fact]
        public void FindWithCompositeKeys()
        {
            var keyInfos = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(typeof(CustomerWithThreeCompositePrimaryKey));

            Assert.Equal(3, keyInfos.Length);
            Assert.Equal(nameof(CustomerWithThreeCompositePrimaryKey.Id1), keyInfos[0].Name);
            Assert.Equal(nameof(CustomerWithThreeCompositePrimaryKey.Id2), keyInfos[1].Name);
            Assert.Equal(nameof(CustomerWithThreeCompositePrimaryKey.Id3), keyInfos[2].Name);
        }

        [Fact]
        public void CannotFindWhenEntityHasNoKeys()
        {
            var keyInfos = PrimaryKeyConventionHelper.GetPrimaryKeyPropertyInfos(typeof(CustomerWithNoId));

            Assert.Empty(keyInfos);
        }
    }
}
