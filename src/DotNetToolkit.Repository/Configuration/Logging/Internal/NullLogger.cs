namespace DotNetToolkit.Repository.Configuration.Logging.Internal
{
    /// <summary>
    /// Represents an internal logger which doesn't log any information anywhere.
    /// </summary>
    internal sealed class NullLogger : ILogger
    {
        internal static NullLogger Instance { get; } = new NullLogger();

        private NullLogger() { }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log(LogLevel logLevel, string message) { }
    }
}
