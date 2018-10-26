namespace DotNetToolkit.Repository.Logging
{
    /// <summary>
    /// Represents a logging provider.
    /// </summary>
    public interface ILoggerProvider
    {
        /// <summary>
        /// Creates a new <see cref="ILogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The <see cref="ILogger"/>.</returns>
        ILogger Create(string categoryName);
    }
}
