using Microsoft.Extensions.Logging;

namespace CoreNetLib
{
    public static class CoreNetLogging
    {
        public static ILoggerFactory LoggerFactory { get; } =
          new LoggerFactory();
        internal static ILogger CreateLogger<T>() =>
          LoggerFactory.CreateLogger<T>();
    }
}
