namespace DotNetToolkit.Repository.Configuration.Logging
{
    /// <inheritdoc />
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

        /// <inheritdoc />
        public ILogger Create(string categoryName)
        {
            return new ConsoleLogger(_minLogLevel, categoryName);
        }
    }
}
