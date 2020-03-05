namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Configuration.Interceptors;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity;

    public class TestRepositoryTimeStampInterceptor : RepositoryInterceptorBase
    {
        private readonly string _user;

        public string User { get { return _user; } }

        [InjectionConstructor]
        public TestRepositoryTimeStampInterceptor()
        {
            _user = "UNKNOWN_USER";
        }

        public TestRepositoryTimeStampInterceptor(string loggedInUser)
        {
            _user = loggedInUser;
        }

        public override void AddExecuting<TEntity>(TEntity entity)
        {
            if (entity is IHaveTimeStamp haveStamp)
            {
                var currentTime = DateTime.UtcNow;

                haveStamp.CreateTime = currentTime;
                haveStamp.CreateUser = _user;
                haveStamp.ModTime = currentTime;
                haveStamp.ModUser = _user;
            }
        }

        public override void UpdateExecuting<TEntity>(TEntity entity)
        {
            if (entity is IHaveTimeStamp haveStamp)
            {
                var currentTime = DateTime.UtcNow;

                haveStamp.ModTime = currentTime;
                haveStamp.ModUser = _user;
            }
        }

        public override Task AddExecutingAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            if (entity is IHaveTimeStamp haveStamp)
            {
                var currentTime = DateTime.UtcNow;

                haveStamp.CreateTime = currentTime;
                haveStamp.CreateUser = _user;
                haveStamp.ModTime = currentTime;
                haveStamp.ModUser = _user;
            }

            return Task.FromResult(0);
        }

        public override Task UpdateExecutingAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = new CancellationToken())
        {
            if (entity is IHaveTimeStamp haveStamp)
            {
                var currentTime = DateTime.UtcNow;

                haveStamp.ModTime = currentTime;
                haveStamp.ModUser = _user;
            }

            return Task.FromResult(0);
        }
    }
}
