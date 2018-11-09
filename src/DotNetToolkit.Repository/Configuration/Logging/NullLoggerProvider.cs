namespace DotNetToolkit.Repository.Configuration.Logging
{
    /// <summary>
    /// An implementation of <see cref="ILoggerProvider" />.
    /// </summary>
    internal class NullLoggerProvider : ILoggerProvider
    {
        internal static NullLoggerProvider Instance { get; } = new NullLoggerProvider();

        private NullLoggerProvider() { }

        /// <summary>
        /// Creates a new <see cref="ILogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The <see cref="ILogger"/>.</returns>
        public ILogger Create(string categoryName)
        {
            return NullLogger.Instance;
        }
    }
}
