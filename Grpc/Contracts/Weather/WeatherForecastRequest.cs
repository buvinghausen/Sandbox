using System.Runtime.Serialization;

using NodaTime;

namespace Grpc.Contracts.Weather;

[DataContract]
public sealed class WeatherForecastRequest
{
    [DataMember(Order = 1)]
    public LocalDate? Date { get; set; }
}
