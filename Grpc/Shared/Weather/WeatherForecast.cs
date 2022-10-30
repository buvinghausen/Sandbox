using System.Runtime.Serialization;

using NodaTime;

namespace Grpc.Shared.Weather;

[DataContract]
public sealed class WeatherForecast
{
    [DataMember(Order = 1)]
    public LocalDate Date { get; set; }

    [DataMember(Order = 2)]
    public int TemperatureC { get; set; }

    [DataMember(Order = 3)]
    public string? Summary { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
