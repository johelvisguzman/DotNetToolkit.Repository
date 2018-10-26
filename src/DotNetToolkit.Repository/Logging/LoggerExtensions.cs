namespace DotNetToolkit.Repository.Logging
{
    using System;

    /// <summary>
    /// Contains various extension methods for <see cref="ILogger" />.
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Logs a message with a <see cref="LogLevel.Debug" /> severity.
        /// </summary>
        /// <param name="source">The logger.</param>
        /// <param name="message">The message to log.</param>
        public static void Debug(this ILogger source, string message)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Log(LogLevel.Debug, message);
        }

        /// <summary>
        /// Logs a composite format message with a <see cref="LogLevel.Debug" /> severity.
        /// </summary>
        /// <param name="source">The logger.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Debug(this ILogger source, string message, params object[] args)
        {
            Debug(source, string.Format(message, args));
        }

        /// <summary>
        /// Logs a message with a <see cref="LogLevel.Info" /> severity.
        /// </summary>
        /// <param name="source">The logger.</param>
        /// <param name="message">The message to log.</param>
        public static void Info(this ILogger source, string message)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Log(LogLevel.Info, message);
        }

        /// <summary>
        /// Logs a composite format message with a <see cref="LogLevel.Info" /> severity.
        /// </summary>
        /// <param name="source">The logger.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Info(this ILogger source, string message, params object[] args)
        {
            Info(source, string.Format(message, args));
        }

        /// <summary>
        /// Logs a message with a <see cref="LogLevel.Warning" /> severity.
        /// </summary>
        /// <param name="source">The logger.</param>
        /// <param name="message">The message to log.</param>
        public static void Warning(this ILogger source, string message)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Log(LogLevel.Warning, message);
        }

        /// <summary>
        /// Logs a composite format message with a <see cref="LogLevel.Warning" /> severity.
        /// </summary>
        /// <param name="source">The logger.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Warning(this ILogger source, string message, params object[] args)
        {
            Warning(source, string.Format(message, args));
        }

        /// <summary>
        /// Logs a message with a <see cref="LogLevel.Error" /> severity.
        /// </summary>
        /// <param name="source">The logger.</param>
        /// <param name="message">The message to log.</param>
        public static void Error(this ILogger source, string message)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            source.Log(LogLevel.Error, message);
        }

        /// <summary>
        /// Logs a composite format message with a <see cref="LogLevel.Error" /> severity.
        /// </summary>
        /// <param name="source">The logger.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Error(this ILogger source, string message, params object[] args)
        {
            Error(source, string.Format(message, args));
        }

        /// <summary>
        /// Logs a message associated with an exception with a <see cref="LogLevel.Error" /> severity.
        /// </summary>
        /// <param name="source">The logger.</param>
        /// <param name="exception">The associated exception to log.</param>
        /// <param name="message">The message to log.</param>
        public static void Error(this ILogger source, Exception exception, string message)
        {
            Error(source, message + "\n" + exception);
        }

        /// <summary>
        /// Logs an exception with a <see cref="LogLevel.Error" /> severity.
        /// </summary>
        /// <param name="source">The logger.</param>
        /// <param name="exception">The associated exception to log.</param>
        public static void Error(this ILogger source, Exception exception)
        {
            Error(source, exception.ToString());
        }
    }
}
