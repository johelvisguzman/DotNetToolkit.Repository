namespace DotNetToolkit.Repository.Integration.Test.Data
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public static class TestMemcachedServer
    {
        private static readonly string _basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tools");
        private static readonly string _exePath = Path.Combine(_basePath, "memcached.exe");

        public static IDisposable Run(int port = 11211, bool verbose = false, int maxMem = 512, bool hidden = true)
        {
            var args = $"-E default_engine.so -p {port} -m {maxMem}";
            if (verbose) args += " -vv";

            var process = Process.Start(new ProcessStartInfo
            {
                Arguments = args,
                FileName = _exePath,
                WorkingDirectory = _basePath,
                WindowStyle = hidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal
            });

            return new KillProcess(process);
        }

        private class KillProcess : IDisposable
        {
            private Process process;

            public KillProcess(Process process)
            {
                this.process = process;

                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
                AppDomain.CurrentDomain.DomainUnload += CurrentDomain_ProcessExit;
            }

            ~KillProcess()
            {
                GC.WaitForPendingFinalizers();

                Dispose();
            }

            private void CurrentDomain_ProcessExit(object sender, EventArgs e)
            {
                Dispose();
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);

                if (process != null)
                {
                    using (process)
                        process.Kill();

                    process = null;
                }
            }
        }
    }
}
