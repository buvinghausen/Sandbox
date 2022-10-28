using BlazorWasm.Client.Models;

namespace BlazorWasm.Client.Services;

public interface IWeatherForecastService
{
    Task<IEnumerable<WeatherForecast>?> GetForecastAsync(DateOnly startDate);
}
