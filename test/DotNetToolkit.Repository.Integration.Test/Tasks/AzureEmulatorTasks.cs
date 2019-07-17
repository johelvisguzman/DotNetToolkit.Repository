namespace DotNetToolkit.Repository.Integration.Test.Tasks
{
    using Helpers;
    using System;
    using System.Linq;
    using System.Threading;

    public static class AzureEmulatorTasks
    {
        private static readonly object _syncRoot = new Object();
        private static int _count;
        private static IDisposable _runningTask;

        public static void Run(Type testClassType)
        {
            lock (_syncRoot)
            {
                if (Interlocked.CompareExchange(ref _count, 0, 0) == 0)
                {
                    _runningTask = Task.Run(
                        "Tasks\\tools\\azure-storage-emulator",
                        "AzureStorageEmulator.exe",
                        "start -inprocess");

                    _count = XunitHelper.GetAllTestingMethods(testClassType).Count();
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
