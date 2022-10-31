using System.Security.Cryptography;

using BlazorWasm.Client.Services.Weather;

using ProtoBuf.Grpc;

namespace BlazorWasm.Server.Services;

internal sealed class WeatherForecastService : IWeatherForecastService
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastService> _logger;

    public WeatherForecastService(ILogger<WeatherForecastService> logger)
    {
        _logger = logger;
    }

    public Task<WeatherForecastResponse[]> GetForecastsAsync(WeatherForecastRequest request,
        CallContext context = default)
    {
        _logger.LogInformation("GetForecastAsync");
        return Task.FromResult(Enumerable.Range(0, 5).Select(index =>
                new WeatherForecastResponse(request.Date.GetValueOrDefault().PlusDays(index),
                    RandomNumberGenerator.GetInt32(-20, 55),
                    Summaries[RandomNumberGenerator.GetInt32(Summaries.Length)]))
            .ToArray());
    }
}
