using System.Runtime.Serialization;

using NodaTime;

namespace BlazorWasm.Client.Services.Weather;

[DataContract]
public sealed record WeatherForecastResponse
{
    // All gRPC classes must have a paramterless constructor
    public WeatherForecastResponse()
    {
    }

    public WeatherForecastResponse(LocalDate date, int temperatureC, string summary)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
    }

    [DataMember(Order = 1)]
    public LocalDate Date { get; init; }

    [DataMember(Order = 2)]
    public int TemperatureC { get; init; }

    [DataMember(Order = 3)]
    public string Summary { get; init; } = string.Empty;

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}