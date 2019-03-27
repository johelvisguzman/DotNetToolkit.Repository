namespace DotNetToolkit.Repository.Wpf.Demo.Infrastructure.Logging
{
    using Configuration.Logging;
    using log4net;
    using log4net.Core;
    using System;

    public class Log4NetLogger : DotNetToolkit.Repository.Configuration.Logging.ILogger
    {
        private readonly log4net.ILog _log;

        public Log4NetLogger(ILog log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            _log = log;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            var level = GetLog4NetLevel(logLevel);

            return _log.Logger.IsEnabledFor(level);
        }

        private Level GetLog4NetLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    return Level.Debug;
                case LogLevel.Info:
                    return Level.Info;
                case LogLevel.Warning:
                    return Level.Warn;
                case LogLevel.Error:
                    return Level.Warn;
                default:
                    return Level.Off;
            }
        }

        public void Log(LogLevel logLevel, string message)
        {
            if (!IsEnabled(logLevel))
                return;

            var level = GetLog4NetLevel(logLevel);

            _log.Logger.Log(GetType(), level, message, null);
        }
    }
}
