namespace DotNetToolkit.Repository.Configuration.Logging
{
    using JetBrains.Annotations;
    using System;
    using Utility;

    /// <summary>
    /// Represents a logger for that output to the console.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private readonly LogLevel _minLogLevel;
        private readonly string _categoryName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogger"/> class.
        /// </summary>
        /// <param name="minLogLevel">Indicates that only message of this level or higher will be logged.</param>
        /// <param name="categoryName">The name of the category.</param>
        public ConsoleLogger(LogLevel minLogLevel, [NotNull] string categoryName)
        {
            Guard.NotEmpty(categoryName);

            _minLogLevel = minLogLevel;
            _categoryName = categoryName;
        }

        /// <summary>
        /// Checks if the given <paramref name="logLevel"/> is enabled.
        /// </summary>
        /// <param name="logLevel">level to be checked.</param>
        /// <returns><c>true</c> if enabled.</returns>
        public virtual bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _minLogLevel;
        }

        /// <summary>
        /// Logs a message with a specified <paramref name="logLevel"/> severity.
        /// </summary>
        /// <param name="logLevel">The log level severity.</param>
        /// <param name="message">The message to log.</param>
        public virtual void Log(LogLevel logLevel, [CanBeNull] string message)
        {
            if (!IsEnabled(logLevel))
                return;

            Console.Error.WriteLine("[{0} {1}] [{2}]   {3}",
                DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt"),
                logLevel.ToString(),
                _categoryName,
                message);
        }
    }
}
