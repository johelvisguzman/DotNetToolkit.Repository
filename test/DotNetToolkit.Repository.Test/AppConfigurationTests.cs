namespace DotNetToolkit.Repository.Test
{
    using Data;
    using Internal.ConfigFile;
    using System;
    using System.Linq;
    using Xunit;

    public class AppConfigurationTests
    {
        [Fact]
        public void CanCreateInterceptorsWithParameters()
        {
            const string type = "DotNetToolkit.Repository.Test.Data.TestRepositoryInterceptor, DotNetToolkit.Repository.Test";
            const string paramName = "random param";

            var interceptorElementCollection = new RepositoryInterceptorElementCollection();
            var interceptorElement = interceptorElementCollection.AddInterceptor(type);

            Assert.Equal(type, interceptorElement.Type);

            interceptorElement.Parameters = new ParameterCollection();

            var p1 = interceptorElement.Parameters.NewElement();
            p1.ValueString = paramName;
            p1.TypeName = "System.String";

            var p2 = interceptorElement.Parameters.NewElement();
            p2.ValueString = "True";
            p2.TypeName = "System.Boolean";

            var parameters = interceptorElement.Parameters.GetTypedParameterValues();

            Assert.Equal(2, parameters.Length);
            Assert.Equal(paramName, parameters.ElementAt(0));
            Assert.True((bool)parameters.ElementAt(1));

            var interceptors = interceptorElementCollection.GetTypedValues().ToList();
            var interceptor = (TestRepositoryInterceptor)interceptors[0].Value();

            Assert.Single(interceptors);
            Assert.Equal(paramName, interceptor.P1);
        }

        [Fact]
        public void CanCreateInterceptorsWithDefaultProviderFactory()
        {
            const string type = "DotNetToolkit.Repository.Test.Data.TestRepositoryInterceptor, DotNetToolkit.Repository.Test";
            const string paramName = "not so random param";

            var interceptorElementCollection = new RepositoryInterceptorElementCollection();
            var interceptorElement = interceptorElementCollection.AddInterceptor(type);

            Assert.Equal(type, interceptorElement.Type);

            interceptorElement.Parameters = new ParameterCollection();

            RepositoryInterceptorProvider.SetDefaultFactory(t => Activator.CreateInstance(t, paramName, true));

            var interceptors = interceptorElementCollection.GetTypedValues().ToList();
            var interceptor = (TestRepositoryInterceptor)interceptors[0].Value();

            Assert.Single(interceptors);
            Assert.Equal(paramName, interceptor.P1);
            Assert.True(interceptor.P2);
        }
    }
}
