using System.Net.Http.Json;

using BlazorWasm.Client.Models;

namespace BlazorWasm.Client.Services;

internal sealed class WeatherForecastService : IWeatherForecastService
{
    private readonly HttpClient _http;

    public WeatherForecastService(HttpClient http)
    {
        _http = http;
    }
    
    public Task<IEnumerable<WeatherForecast>?> GetForecastAsync(DateOnly startDate) =>
        _http.GetFromJsonAsync<IEnumerable<WeatherForecast>?>("WeatherForecast");
}
