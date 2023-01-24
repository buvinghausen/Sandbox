using NodaTime;

namespace Grpc.Contracts.Weather;

public sealed record WeatherForecastRequest(LocalDate Date)
{
    // Helper method to convert from System.DateTime
    public WeatherForecastRequest(DateTime date) : this(LocalDate.FromDateTime(date))
    {
    }
}