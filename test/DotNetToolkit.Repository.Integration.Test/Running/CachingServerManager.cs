namespace DotNetToolkit.Repository.Integration.Test.Running
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public static class CachingServerManager
    {
        private static IDisposable _memcachedRunningTask;
        private static IDisposable _redisRunningTask;

        private static readonly ProcessStartInfo _startRedis = new ProcessStartInfo
        {
            FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Running\\tools\\redis-server\\redis-server.exe"),
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        private static readonly ProcessStartInfo _startMemcached = new ProcessStartInfo
        {
            FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Running\\tools\\memcached-server\\memcached.exe"),
            Arguments = "-E default_engine.so -p 11211 -m 512",
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        public static void StartMemcached()
        {
            if (_memcachedRunningTask == null)
                _memcachedRunningTask = new DisposableRunningProcess(Process.Start(_startMemcached));
        }

        public static void StartRedis()
        {
            if (_redisRunningTask == null)
                _redisRunningTask = new DisposableRunningProcess(Process.Start(_startRedis));
        }

        public static void StopMemcached()
        {
            if (_memcachedRunningTask != null)
                _memcachedRunningTask.Dispose();
        }

        public static void StopRedis()
        {
            if (_redisRunningTask != null)
                _redisRunningTask.Dispose();
        }
    }
}
