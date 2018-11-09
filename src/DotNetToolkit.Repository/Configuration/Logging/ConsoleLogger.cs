namespace DotNetToolkit.Repository.Configuration.Logging
{
    using System;

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
        public ConsoleLogger(LogLevel minLogLevel, string categoryName)
        {
            if (categoryName == null)
                throw new ArgumentNullException(nameof(categoryName));

            _minLogLevel = minLogLevel;
            _categoryName = categoryName;
        }

        /// <inheritdoc />
        public virtual bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= _minLogLevel;
        }

        /// <inheritdoc />
        public virtual void Log(LogLevel logLevel, string message)
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
