namespace DotNetToolkit.Repository.Performance.Tasks
{
    using System;
    using System.Threading;

    public static class AzureEmulatorTasks
    {
        private static readonly object _syncRoot = new Object();
        private static int _count;
        private static IDisposable _runningTask;

        public static void Run()
        {
            lock (_syncRoot)
            {
                if (Interlocked.Increment(ref _count) == 1)
                {
                    _runningTask = Task.Run(
                        "Tasks\\tools\\azure-storage-emulator",
                        "AzureStorageEmulator.exe",
                        "start -inprocess");
                }
            }
        }

        public static void Cleanup()
        {
            lock (_syncRoot)
            {
                // Only stop when all tests have been completed
                if (Interlocked.Decrement(ref _count) == 0)
                {
                    _runningTask.Dispose();
                }
            }
        }
    }
}
