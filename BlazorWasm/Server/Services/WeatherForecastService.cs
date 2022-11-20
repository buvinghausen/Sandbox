using System.Security.Claims;
using System.Security.Cryptography;

using BlazorWasm.Client.Services.Weather;
using BlazorWasm.Client.Shared;

using Grpc.Core;

using Microsoft.AspNetCore.Authorization;

using ProtoBuf.Grpc;

namespace BlazorWasm.Server.Services;

// Only allow logged in users to invoke this service
[Authorize(Policy = Policies.Authorized)]
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

    public ValueTask<WeatherForecastResponse[]> GetForecastsAsync(WeatherForecastRequest request,
        CallContext context = default)
    {
        _logger.LogInformation("GetForecastAsync");
        if (context.ServerCallContext != default)
        {
            var ctx = context.ServerCallContext.GetHttpContext();
            var id = ctx.User.FindFirstValue("id");
            _logger.LogInformation("Id: {Id}", id);
        }
        return new(Enumerable.Range(0, 5).Select(index =>
                new WeatherForecastResponse(request.Date.GetValueOrDefault().PlusDays(index),
                    RandomNumberGenerator.GetInt32(-20, 55),
                    Summaries[RandomNumberGenerator.GetInt32(Summaries.Length)]))
            .ToArray());
    }
}
