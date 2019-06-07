namespace DotNetToolkit.Repository.Integration.Test.CachingServers
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class ServerProcess
    {
        public static IDisposable Run<TConfig>() where TConfig : IServerProcessConfig, new()
        {
            return Run(new TConfig());
        }

        public static IDisposable Run(IServerProcessConfig config)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                Arguments = config.Args,
                FileName = Path.Combine(config.BasePath, config.Exe),
                WorkingDirectory = config.BasePath,
                WindowStyle = config.IsHidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal
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
