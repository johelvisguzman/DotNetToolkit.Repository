namespace DotNetToolkit.Repository.Integration.Test.Tasks
{
    using System;
    using System.IO;
    using Utility;
    using System.Diagnostics;

    public class Task
    {
       public static IDisposable Run(string basePath, string exe, string args = null)
        {
            var workingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, basePath);
            var fileName = Path.Combine(workingDirectory, exe);

            var process = System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                Arguments = args,
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

                if (_process != null && !_process.HasExited)
                {
                    _process.Kill();
                    _process.Dispose();
                    _process = null;
                }
            }
        }
    }
}
