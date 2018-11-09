namespace DotNetToolkit.Repository.Configuration.Logging
{
    /// <summary>
    /// Represents an internal logger which doesn't log any information anywhere.
    /// </summary>
    internal sealed class NullLogger : ILogger
    {
        internal static NullLogger Instance { get; } = new NullLogger();

        private NullLogger() { }

        /// <inheritdoc />
        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        /// <inheritdoc />
        public void Log(LogLevel logLevel, string message) { }
    }
}
