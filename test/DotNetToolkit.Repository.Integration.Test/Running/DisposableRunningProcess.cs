namespace DotNetToolkit.Repository.Integration.Test.Running
{
    using System;
    using System.Diagnostics;
    using Utility;

    public class DisposableRunningProcess : IDisposable
    {
        private Process _process;

        public DisposableRunningProcess(Process process)
        {
            _process = Guard.NotNull(process, nameof(process));

            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_ProcessExit;
        }

        ~DisposableRunningProcess()
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
