namespace DotNetToolkit.Repository.Test
{
    using Configuration.Logging;
    using Xunit;

    public class ConsoleLoggerTests
    {
        [Fact]
        public void IsEnabled()
        {
            var logger = new ConsoleLogger(LogLevel.Debug, "ConsoleLoggerTests");

            Assert.True(logger.IsEnabled(LogLevel.Debug));
            Assert.True(logger.IsEnabled(LogLevel.Info));
            Assert.True(logger.IsEnabled(LogLevel.Warning));
            Assert.True(logger.IsEnabled(LogLevel.Error));
            Assert.True(logger.IsEnabled(LogLevel.None));

            logger = new ConsoleLogger(LogLevel.Info, "ConsoleLoggerTests");

            Assert.False(logger.IsEnabled(LogLevel.Debug));
            Assert.True(logger.IsEnabled(LogLevel.Info));
            Assert.True(logger.IsEnabled(LogLevel.Warning));
            Assert.True(logger.IsEnabled(LogLevel.Error));
            Assert.True(logger.IsEnabled(LogLevel.None));

            logger = new ConsoleLogger(LogLevel.Warning, "ConsoleLoggerTests");

            Assert.False(logger.IsEnabled(LogLevel.Debug));
            Assert.False(logger.IsEnabled(LogLevel.Info));
            Assert.True(logger.IsEnabled(LogLevel.Warning));
            Assert.True(logger.IsEnabled(LogLevel.Error));
            Assert.True(logger.IsEnabled(LogLevel.None));

            logger = new ConsoleLogger(LogLevel.Error, "ConsoleLoggerTests");

            Assert.False(logger.IsEnabled(LogLevel.Debug));
            Assert.False(logger.IsEnabled(LogLevel.Info));
            Assert.False(logger.IsEnabled(LogLevel.Warning));
            Assert.True(logger.IsEnabled(LogLevel.Error));
            Assert.True(logger.IsEnabled(LogLevel.None));

            logger = new ConsoleLogger(LogLevel.None, "ConsoleLoggerTests");

            Assert.False(logger.IsEnabled(LogLevel.Debug));
            Assert.False(logger.IsEnabled(LogLevel.Info));
            Assert.False(logger.IsEnabled(LogLevel.Warning));
            Assert.False(logger.IsEnabled(LogLevel.Error));
            Assert.True(logger.IsEnabled(LogLevel.None));
        }
    }
}
