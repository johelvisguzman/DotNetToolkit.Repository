namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Configuration.Interceptors;
    using Unity;

    public class TestRepositoryInterceptor : RepositoryInterceptorBase
    {
        public string P1 { get; }
        public bool P2 { get; }

        [InjectionConstructor]
        public TestRepositoryInterceptor() { }

        public TestRepositoryInterceptor(string p1, bool p2)
        {
            P1 = p1;
            P2 = p2;
        }
    }
}
