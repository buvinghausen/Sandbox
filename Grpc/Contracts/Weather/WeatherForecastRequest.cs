using System.Runtime.Serialization;

using NodaTime;

namespace Grpc.Contracts.Weather;

[DataContract]
public sealed record WeatherForecastRequest
{
    // All gRPC classes must have a paramterless constructor
    public WeatherForecastRequest()
    {
    }

    public WeatherForecastRequest(LocalDate? date)
    {
        Date = date;
    }

    [DataMember(Order = 1)]
    public LocalDate? Date { get; }
}
