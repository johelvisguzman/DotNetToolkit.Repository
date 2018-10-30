namespace DotNetToolkit.Repository.Configuration.Logging
{
    /// <summary>
    /// Represents a logger for logging messages.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Checks if the given <paramref name="logLevel"/> is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns><c>true</c> if enabled.</returns>
        bool IsEnabled(LogLevel logLevel);

        /// <summary>
        /// Logs a message with a specified <paramref name="logLevel"/> severity.
        /// </summary>
        /// <param name="logLevel">The log level severity.</param>
        /// <param name="message">The message to log.</param>
        void Log(LogLevel logLevel, string message);
    }
}
