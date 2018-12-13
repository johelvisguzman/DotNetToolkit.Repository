namespace DotNetToolkit.Repository.Test
{
    using Queries.Strategies;
    using Xunit;

    public class SpecificationTests
    {
        class Product
        {
            public string Name { get; set; }
            public double Price { get; set; }
        }

        class ProductOnSaleSpecification : SpecificationQueryStrategy<Product>
        {
            public ProductOnSaleSpecification() : base(p => p.Price < 100) { }
        }

        [Fact]
        public void FindBySpecification()
        {
            var product = new Product { Price = 99 };
            var spec = new SpecificationQueryStrategy<Product>(p => p.Price < 100);

            Assert.True(spec.IsSatisfiedBy(product));

            product.Price = 100;

            Assert.False(spec.IsSatisfiedBy(product));
        }

        [Fact]
        public void FindByAndCompositeSpecification()
        {
            var product = new Product { Price = 99, Name = "Windows XP Professional" };
            var spec = new SpecificationQueryStrategy<Product>(p => p.Price < 100).And(new SpecificationQueryStrategy<Product>(p => p.Name == "Windows XP Professional"));

            Assert.True(spec.IsSatisfiedBy(product));

            product.Name = "Random Name";

            Assert.False(spec.IsSatisfiedBy(product));

            product.Name = "Windows XP Professional";

            Assert.True(spec.IsSatisfiedBy(product));

            product.Price = 100;

            Assert.False(spec.IsSatisfiedBy(product));
        }

        [Fact]
        public void FindByOrCompositeSpecification()
        {
            var product = new Product { Price = 99, Name = "Windows XP Professional" };
            var spec = new SpecificationQueryStrategy<Product>(p => p.Price < 100).Or(new SpecificationQueryStrategy<Product>(p => p.Name == "Windows XP Professional"));

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
        public void FindByNotCompositeSpecification()
        {
            var product = new Product { Price = 99, Name = "Windows XP Professional" };
            var spec = new SpecificationQueryStrategy<Product>(p => p.Price < 100).Not();

            Assert.False(spec.IsSatisfiedBy(product));

            product.Price = 100;

            Assert.True(spec.IsSatisfiedBy(product));
        }

        [Fact]
        public void FindByConcreteSpecification()
        {
            var product = new Product { Price = 99 };
            var spec = new ProductOnSaleSpecification();

            Assert.True(spec.IsSatisfiedBy(product));

            product.Price = 100;

            Assert.False(spec.IsSatisfiedBy(product));
        }

        [Fact]
        public void Spec_ToString()
        {
            var spec = new SpecificationQueryStrategy<Product>(p => p.Price < 100).Or(p => p.Name.Equals("Windows XP Professional"));

            Assert.Equal("SpecificationQueryStrategy<Product>: [ Predicate = p => ((p.Price < 100) OrElse p.Name.Equals(\"Windows XP Professional\")) ]", spec.ToString());
        }
    }
}
