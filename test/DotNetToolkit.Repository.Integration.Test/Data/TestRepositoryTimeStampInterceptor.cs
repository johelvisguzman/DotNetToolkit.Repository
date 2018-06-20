﻿namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using Interceptors;
    using System;

    public class TestRepositoryTimeStampInterceptor : RepositoryInterceptor
    {
        private readonly string _user;

        public TestRepositoryTimeStampInterceptor(string loggedInUser)
        {
            _user = loggedInUser;
        }

        public override void AddExecuting<TEntity>(TEntity entity)
        {
            base.AddExecuting(entity);

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
    }
}