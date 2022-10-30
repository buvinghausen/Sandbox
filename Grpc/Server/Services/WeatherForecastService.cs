using System.Security.Cryptography;

using FluentValidation;

using Grpc.Contracts.Weather;

using NodaTime;
using NodaTime.Extensions;

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

    public Task<WeatherForecastResponse[]> GetForecastsAsync(WeatherForecastRequest request, CallContext context = default)
    {
        _logger.LogInformation("GetForecastAsync");
        return Task.FromResult(Enumerable.Range(0, 5).Select(index => new WeatherForecastResponse
        {
            Date = request.Date.GetValueOrDefault().PlusDays(index),
            TemperatureC = RandomNumberGenerator.GetInt32(-20, 55),
            Summary = Summaries[RandomNumberGenerator.GetInt32(Summaries.Length)]
        }).ToArray());
    }
}

internal sealed class WeatherForecastValidator : AbstractValidator<WeatherForecastRequest>
{
    public WeatherForecastValidator()
    {
        // Note: you could create a ZonedClock and inject it into the constructor
        var today = SystemClock.Instance.InTzdbSystemDefaultZone().GetCurrentDate();
        RuleFor(w => w.Date)
            .NotEmpty()
            .GreaterThanOrEqualTo(today)
            .LessThanOrEqualTo(today.PlusDays(10));
    }
}
