using NodaTime;

namespace BlazorWasm.Client.Services.Weather;

public sealed record WeatherForecastResponse(LocalDate Date, int TemperatureC, string Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
