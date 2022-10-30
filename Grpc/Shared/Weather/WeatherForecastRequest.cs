using System.Runtime.Serialization;

using NodaTime;

namespace Grpc.Shared.Weather;

[DataContract]
public sealed class WeatherForecastRequest
{
    [DataMember(Order = 1)]
    public LocalDate Date { get; set; }
}
