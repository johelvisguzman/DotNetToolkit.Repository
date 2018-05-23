namespace DotNetToolkit.Repository.Logging
{
    using System;

    /// <summary>
    /// Represents a logger which logs activities in the repository.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logs the specified message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="objects">The objects to log.</param>
        void Write(string message, params object[] objects);

        /// <summary>
        /// Logs the specified exception.
        /// </summary>
        /// <param name="ex">The exception to log.</param>
        void Write(Exception ex);
    }
}