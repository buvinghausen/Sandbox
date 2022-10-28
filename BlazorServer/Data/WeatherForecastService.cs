namespace BlazorServer.Data;

public class WeatherForecastService
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public Task<WeatherForecast[]> GetForecastAsync(DateOnly startDate) =>
        Task.FromResult(Enumerable.Range(0, 5).Select(index => new WeatherForecast(startDate.AddDays(index),
            Random.Shared.Next(-20, 55), Summaries[Random.Shared.Next(Summaries.Length)])).ToArray());
}
