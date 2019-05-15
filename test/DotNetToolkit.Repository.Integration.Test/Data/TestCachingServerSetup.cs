﻿namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public static class TestCachingServerSetup
    {
        private static readonly object _syncRoot = new Object();
        private static int _count;
        private static List<IDisposable> _servers;

        public static void Run()
        {
            lock (_syncRoot)
            {
                if (_servers == null)
                {
                    _servers = new List<IDisposable>
                    {
                        TestMemcachedServer.Run(11211)
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
                    foreach (var server in _servers)
                    {
                        server.Dispose();
                    }

                    _servers = null;
                }
            }
        }
    }
}