namespace DotNetToolkit.Repository.Configuration.Logging.Internal
{
    /// <summary>
    /// Represents an internal logger which doesn't log any information anywhere.
    /// </summary>
    internal sealed class NullLogger : ILogger
    {
        internal static NullLogger Instance { get; } = new NullLogger();

        private NullLogger() { }

        /// <summary>
        /// Checks if the given <paramref name="logLevel"/> is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns><c>true</c> if enabled.</returns>
        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        /// <summary>
        /// Logs a message with a specified <paramref name="logLevel"/> severity.
        /// </summary>
        /// <param name="logLevel">The log level severity.</param>
        /// <param name="message">The message to log.</param>
        public void Log(LogLevel logLevel, string message) { }
    }
}
