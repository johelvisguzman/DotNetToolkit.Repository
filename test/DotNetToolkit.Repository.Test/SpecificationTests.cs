namespace DotNetToolkit.Repository.Test
{
    using Specifications;
    using Xunit;

    public class SpecificationTests
    {
        class Product
        {
            public string Name { get; set; }
            public double Price { get; set; }
        }

        class ProductOnSaleSpecification : Specification<Product>
        {
            public ProductOnSaleSpecification() : base(p => p.Price < 100) { }
        }

        [Fact]
        public void Find_By_Specification()
        {
            var product = new Product { Price = 99 };
            var spec = new Specification<Product>(p => p.Price < 100);

            Assert.True(spec.IsSatisfiedBy(product));

            product.Price = 100;

            Assert.False(spec.IsSatisfiedBy(product));
        }

        [Fact]
        public void Find_By_And_Composite_Specification()
        {
            var product = new Product { Price = 99, Name = "Windows XP Professional" };
            var spec = new Specification<Product>(p => p.Price < 100).And(new Specification<Product>(p => p.Name == "Windows XP Professional"));

            Assert.True(spec.IsSatisfiedBy(product));

            product.Name = "Random Name";

            Assert.False(spec.IsSatisfiedBy(product));

            product.Name = "Windows XP Professional";

            Assert.True(spec.IsSatisfiedBy(product));

            product.Price = 100;

            Assert.False(spec.IsSatisfiedBy(product));
        }

        [Fact]
        public void Find_By_Or_Composite_Specification()
        {
            var product = new Product { Price = 99, Name = "Windows XP Professional" };
            var spec = new Specification<Product>(p => p.Price < 100).Or(new Specification<Product>(p => p.Name == "Windows XP Professional"));

            Assert.True(spec.IsSatisfiedBy(product));

            product.Name = "Random Name";

            Assert.True(spec.IsSatisfiedBy(product));

            product.Name = "Windows XP Professional";
            product.Price = 100;

            Assert.True(spec.IsSatisfiedBy(product));

            product.Name = "Random Name";

            Assert.False(spec.IsSatisfiedBy(product));
        }

        [Fact]
        public void Find_By_Not_Composite_Specification()
        {
            var product = new Product { Price = 99, Name = "Windows XP Professional" };
            var spec = new Specification<Product>(p => p.Price < 100).Not();

            Assert.False(spec.IsSatisfiedBy(product));

            product.Price = 100;

            Assert.True(spec.IsSatisfiedBy(product));
        }

        [Fact]
        public void Find_By_Concret_Specification()
        {
            var product = new Product { Price = 99 };
            var spec = new ProductOnSaleSpecification();

            Assert.True(spec.IsSatisfiedBy(product));

            product.Price = 100;

            Assert.False(spec.IsSatisfiedBy(product));
        }
    }
}
