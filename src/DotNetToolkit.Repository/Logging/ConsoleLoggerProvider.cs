namespace DotNetToolkit.Repository.Logging
{
    /// <summary>
    /// An implementation of <see cref="ILoggerProvider" />.
    /// </summary>
    public class ConsoleLoggerProvider : ILoggerProvider
    {
        private readonly LogLevel _minLogLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLoggerProvider"/> class.
        /// </summary>
        /// <param name="minLogLevel">Indicates that only message of this level or higher will be logged.</param>
        public ConsoleLoggerProvider(LogLevel minLogLevel)
        {
            _minLogLevel = minLogLevel;
        }

        /// <summary>
        /// Creates a new <see cref="ILogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The <see cref="ILogger"/>.</returns>
        public ILogger Create(string categoryName)
        {
            return new ConsoleLogger(_minLogLevel, categoryName);
        }
    }
}
