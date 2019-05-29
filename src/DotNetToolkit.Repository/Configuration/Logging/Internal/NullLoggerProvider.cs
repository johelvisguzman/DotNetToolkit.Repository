namespace DotNetToolkit.Repository.Configuration.Logging.Internal
{
    /// <summary>
    /// An implementation of <see cref="ILoggerProvider" />.
    /// </summary>
    internal class NullLoggerProvider : ILoggerProvider
    {
        internal static NullLoggerProvider Instance { get; } = new NullLoggerProvider();

        private NullLoggerProvider() { }

        public ILogger Create(string categoryName)
        {
            return NullLogger.Instance;
        }
    }
}
