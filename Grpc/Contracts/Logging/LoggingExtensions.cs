using NodaTime;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Logging;

public static partial class LoggingExtensions
{
    [LoggerMessage(0, LogLevel.Information, "Worker running at: {Time}")]
    public static partial void WorkerRunning(this ILogger logger, DateTimeOffset time);

    [LoggerMessage(1, LogLevel.Debug, "Starting call. Request: {Path}")]
    public static partial void StartGrpcCall(this ILogger logger, string path);

    [LoggerMessage(2, LogLevel.Error, "An error occurred when calling {Method}", SkipEnabledCheck = true)]
    public static partial void GrpcError(this ILogger logger, string method, Exception ex);

    [LoggerMessage(3, LogLevel.Information, "Sending greeting to {Name}")]
    public static partial void SendGreeting(this ILogger logger, string name);

    [LoggerMessage(4, LogLevel.Information, "Connection id: {Id}")]
    public static partial void ConnectionId(this ILogger logger, string id);

    [LoggerMessage(5, LogLevel.Information, "Incrementing count by {Count}")]
    public static partial void IncrementCount(this ILogger logger, int count);

    [LoggerMessage(6, LogLevel.Information, "GetForecastAsync {ForecastDate}")]
    public static partial void LoadForecast(this ILogger logger, LocalDate? forecastDate);
}
