using BlazorWasm.Client.Models;
using BlazorWasm.Client.Services;

namespace BlazorWasm.Server.Services;

internal sealed class WeatherForecastService : IWeatherForecastService
{
    private static readonly string[] Summaries = {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

#pragma warning disable CA1822 // Mark members as static
    public Task<IEnumerable<WeatherForecast>?> GetForecastAsync(DateOnly startDate) =>
        Task.FromResult(Enumerable
            .Range(0, 5)
            .Select(index => new WeatherForecast(startDate.AddDays(index), Random.Shared.Next(-20, 55), Summaries[Random.Shared.Next(Summaries.Length)])))!;
#pragma warning restore CA1822 // Mark members as static
}
