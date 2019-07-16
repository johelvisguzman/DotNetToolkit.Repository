namespace DotNetToolkit.Repository.Configuration.Logging
{
    /// <summary>
    /// An implementation of <see cref="ILoggerProvider" />.
    /// </summary>
    public class NullLoggerProvider : ILoggerProvider
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static NullLoggerProvider Instance { get; } = new NullLoggerProvider();

        /// <summary>
        /// Initializes a new instance of the <see cref="NullLoggerProvider"/> class.
        /// </summary>
        private NullLoggerProvider() { }

        /// <summary>
        /// Creates a new <see cref="ILogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The <see cref="ILogger"/>.</returns>
        public ILogger Create(string categoryName)
        {
            return Internal.NullLogger.Instance;
        }
    }
}
