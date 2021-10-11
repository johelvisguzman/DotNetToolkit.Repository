namespace DotNetToolkit.Repository.Performance.Running
{
    using System.Diagnostics;
    using System.Linq;

    public static class AzureStorageEmulatorManager
    {
        private const string _azureStorageEmulatorPath = @"C:\Program Files (x86)\Microsoft SDKs\Azure\Storage Emulator\AzureStorageEmulator.exe";
        private const string _win7ProcessName = "WAStorageEmulator";
        private const string _win8ProcessName = "WASTOR~1";
        private const string _win10ProcessName = "AzureStorageEmulator";

        static Process GetProcess()
        {
            return Process.GetProcessesByName(_win10ProcessName).FirstOrDefault()
                   ?? Process.GetProcessesByName(_win8ProcessName).FirstOrDefault()
                   ?? Process.GetProcessesByName(_win7ProcessName).FirstOrDefault();
        }

        static readonly ProcessStartInfo _startStorageEmulator = new ProcessStartInfo
        {
            FileName = _azureStorageEmulatorPath,
            Arguments = "start",
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        static readonly ProcessStartInfo _clearStorageEmulator = new ProcessStartInfo
        {
            FileName = _azureStorageEmulatorPath,
            Arguments = "clear all",
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        static readonly ProcessStartInfo _stopStorageEmulator = new ProcessStartInfo
        {
            FileName = _azureStorageEmulatorPath,
            Arguments = "stop",
            WindowStyle = ProcessWindowStyle.Hidden
        };

        static bool IsProcessStarted()
        {
            return GetProcess() != null;
        }

        public static void Start()
        {
            if (IsProcessStarted())
            {
                return;
            }
            using (var process = Process.Start(_startStorageEmulator))
            {
                process.WaitForExit();
            }
        }

        public static void Clear()
        {
            using (var process = Process.Start(_clearStorageEmulator))
            {
                process.WaitForExit();
            }
        }

        public static void Stop()
        {
            using (var process = Process.Start(_stopStorageEmulator))
            {
                process.WaitForExit();
            }
        }
    }
}
