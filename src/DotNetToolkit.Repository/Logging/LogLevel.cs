namespace DotNetToolkit.Repository.Logging
{
    /// <summary>
    /// Represents logging levels.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Debug severity.
        /// </summary>
        Debug = 1,
        /// <summary>
        /// Info severity.
        /// </summary>
        Info = 2,
        /// <summary>
        /// Warning severity.
        /// </summary>
        Warning = 3,
        /// <summary>
        /// Error severity.
        /// </summary>
        Error = 4,
        /// <summary>
        /// Not used for writing log messages. Specifies that a logging category should not write any messages.
        /// </summary>
        None = 5
    }
}
