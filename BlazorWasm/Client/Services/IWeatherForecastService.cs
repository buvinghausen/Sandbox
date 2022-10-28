using BlazorWasm.Client.Models;

namespace BlazorWasm.Client.Services;

public interface IWeatherForecastService
{
    Task<WeatherForecast[]> GetForecastAsync(DateOnly startDate);
}
