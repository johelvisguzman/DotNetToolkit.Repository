namespace DotNetToolkit.Repository.Integration.Test.CachingServers
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Utility;

    public class ServerProcess
    {
        public static IDisposable Run<TConfig>() where TConfig : IServerProcessConfig, new()
        {
            return Run(new TConfig());
        }

        public static IDisposable Run(IServerProcessConfig config)
        {
            Guard.NotNull(config, nameof(config));

            var workingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, config.BasePath);
            var fileName = Path.Combine(workingDirectory, config.Exe);

            var process = Process.Start(new ProcessStartInfo
            {
                Arguments = config.Args,
                FileName = fileName,
                WorkingDirectory = workingDirectory,
                WindowStyle = ProcessWindowStyle.Hidden
            });

            return new KillProcess(process);
        }

        private class KillProcess : IDisposable
        {
            private Process _process;

            public KillProcess(Process process)
            {
                _process = Guard.NotNull(process, nameof(process));

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

                if (_process != null)
                {
                    _process.Kill();
                    _process.Dispose();
                    _process = null;
                }
            }
        }
    }
}
