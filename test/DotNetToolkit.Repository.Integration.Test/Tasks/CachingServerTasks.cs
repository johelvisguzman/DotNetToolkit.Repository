namespace DotNetToolkit.Repository.Integration.Test.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public static class CachingServerTasks
    {
        private static readonly object _syncRoot = new Object();
        private static int _count;
        private static List<IDisposable> _runningTasks;

        public static void Run()
        {
            lock (_syncRoot)
            {
                if (_runningTasks == null)
                {
                    _runningTasks = new List<IDisposable>
                    {
                        Task.Run(
                            "Tasks\\tools\\memcached-server",
                            "memcached.exe",
                            "-E default_engine.so -p 11211 -m 512"),

                        Task.Run(
                            "Tasks\\tools\\redis-server",
                            "redis-server.exe"),
                    };
                }

                Interlocked.Increment(ref _count);
            }
        }

        public static void Cleanup()
        {
            lock (_syncRoot)
            {
                if (Interlocked.Decrement(ref _count) == 0)
                {
                    foreach (var task in _runningTasks)
                    {
                        task.Dispose();
                    }

                    _runningTasks = null;
                }
            }
        }
    }
}
