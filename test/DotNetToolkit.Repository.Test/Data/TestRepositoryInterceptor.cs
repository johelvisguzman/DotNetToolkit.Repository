﻿namespace DotNetToolkit.Repository.Test.Data
{
    using Configuration.Interceptors;

    public class TestRepositoryInterceptor : RepositoryInterceptorBase
    {
        public string P1 { get; }
        public bool P2 { get; }

        public TestRepositoryInterceptor(string p1, bool p2)
        {
            P1 = p1;
            P2 = p2;
        }
    }

    public class TestRepositoryInterceptor2 : RepositoryInterceptorBase
    {
        public TestRepositoryInterceptor2() { }
    }
}
