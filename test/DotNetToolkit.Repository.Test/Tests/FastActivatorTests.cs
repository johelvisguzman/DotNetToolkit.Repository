namespace DotNetToolkit.Repository.Test
{
    using System;
    using Utility;
    using Xunit;

    public class FastActivatorTests
    {
        [Fact]
        public void ThrowsIfUnableToFindDefaultCtor()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => FastActivator.CreateInstance<TestClassWithArgs>());

            Assert.Equal(string.Format("No default constructor exists for class {0}", typeof(TestClassWithArgs).FullName), ex.Message);
        }

        [Fact]
        public void ThrowsIfUnableToFindMatchingCtor()
        {
            var ex = Assert.Throws<InvalidOperationException>(
                () => FastActivator.CreateInstance<TestClassWithArgs>(1));

            Assert.Equal(string.Format("No matching constructor found for class {0}", typeof(TestClassWithArgs).FullName), ex.Message);
        }

        [Fact]
        public void CreateInstanceWithDefaultCtor()
        {
            var obj = FastActivator.CreateInstance<TestClass>();
            
            Assert.NotNull(obj);
        }

        [Fact]
        public void CreateInstanceWithMatchingCtor()
        {
            var obj = FastActivator.CreateInstance<TestClassWithArgs>("Hello");
            
            Assert.NotNull(obj);
            Assert.Null(obj.PropB);
            Assert.Equal("Hello", obj.PropA);

            obj = FastActivator.CreateInstance<TestClassWithArgs>("Hello", "World");

            Assert.NotNull(obj);
            Assert.Equal("Hello", obj.PropA);
            Assert.Equal("World", obj.PropB);

            FastActivator.CreateInstance<TestClassWithArgs>("Hello", "World");
        }

        class TestClass { }

        class TestClassWithArgs
        {
            public string PropA { get; set; }
            public string PropB { get; set; }

            public TestClassWithArgs(string a)
            {
                PropA = a;
            }

            public TestClassWithArgs(string a, string b)
            {
                PropA = a;
                PropB = b;
            }
        }
    }
}
