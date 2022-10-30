using System.Security.Cryptography;

using Grpc.Shared.Weather;

using ProtoBuf.Grpc;

namespace Grpc.Server.Services;

internal sealed class WeatherForecastService : IWeatherForecastService
{
    private static readonly string[] Summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastService> _logger;

    public WeatherForecastService(ILogger<WeatherForecastService> logger)
    {
        _logger = logger;
    }

    public Task<WeatherForecast[]> GetForecastsAsync(WeatherForecastRequest request, CallContext context = default)
    {
        _logger.LogInformation("GetForecastAsync");
        return Task.FromResult(Enumerable.Range(0, 5).Select(index => new WeatherForecast
        {
            Date = request.Date.PlusDays(index),
            TemperatureC = RandomNumberGenerator.GetInt32(-20, 55),
            Summary = Summaries[RandomNumberGenerator.GetInt32(Summaries.Length)]
        }).ToArray());
    }
}
