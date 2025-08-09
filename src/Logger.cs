using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.SystemConsole.Themes;

namespace ModeratorBot
{
    public static class Logger
    {
        private static readonly ILogger logger;

        static Logger()
        {
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(theme: AnsiConsoleTheme.Code)
                .WriteTo.File($"logs/{timestamp}_human_readable.txt")
                .WriteTo.File(new CompactJsonFormatter(), $"logs/{timestamp}_machine_readable.json")
                .CreateLogger();
        }


        /// <summary>
        /// Log debug message.
        /// </summary>
        /// <param name="message">Message log.</param>
        /// <param name="propertyValues">Optional. Serilog's way of doing $"{exampleVariable}", but colorful.</param>
        public static void Debug(string message, params object?[] propertyValues)
        {
            logger.Debug(message, propertyValues);
        }

        /// <summary>
        /// Log information message.
        /// </summary>
        /// <param name="message">Message log.</param>
        /// <param name="propertyValues">Optional. Serilog's way of doing $"{exampleVariable}", but colorful.</param>
        public static void Info(string message, params object[] propertyValues)
        {
            logger.Information(message, propertyValues);
        }


        /// <summary>
        /// Log warning message.
        /// </summary>
        /// <param name="message">Message log.</param>
        /// <param name="propertyValues">Optional. Serilog's way of doing $"{exampleVariable}", but colorful.</param>
        public static void Warn(string message, params object[] propertyValues)
        {
            logger.Warning(message, propertyValues);
        }

        /// <summary>
        /// Log error.
        /// </summary>
        /// <param name="message">Message log.</param>
        /// <param name="ex">Optional. Include the whole exception.</param>
        /// <param name="propertyValues">Optional. Serilog's way of doing $"{exampleVariable}", but colorful.</param>
        public static void Error(string message, Exception? ex = null, params object?[] propertyValues)
        {
            if (ex != null)
                logger.Error(ex, message, propertyValues);
            else
                logger.Error(message, propertyValues);
        }

        public static void Error(string message, params object?[] propertyValues)
        {
            logger.Error(message, propertyValues);
        }

        /// <summary>
        /// Log fatal error.
        /// </summary>
        /// <param name="message">Message log</param>
        /// <param name="ex">Optional. Include the whole exception.</param>
        public static void Fatal(string message, Exception? ex = null)
        {
            if (ex != null)
                logger.Fatal(ex, message);
            else
                logger.Fatal(message);
        }
    }
}
