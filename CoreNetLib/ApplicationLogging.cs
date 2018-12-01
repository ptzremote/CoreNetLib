using Microsoft.Extensions.Logging;

namespace CoreNetLib
{
    public static class CoreNetLogging
    {
        public static ILoggerFactory LoggerFactory { get; } =
          new LoggerFactory();
        public static ILogger CreateLogger<T>() =>
          LoggerFactory.CreateLogger<T>();
    }
}
