using System.Runtime.Serialization;

using FluentValidation;

using NodaTime;
using NodaTime.Extensions;

namespace BlazorWasm.Client.Services.Weather;

[DataContract]
public sealed record WeatherForecastRequest
{
    // All gRPC classes must have a paramterless constructor
    public WeatherForecastRequest()
    {
    }

    public WeatherForecastRequest(DateTime date) : this(LocalDate.FromDateTime(date))
    {
    }

    public WeatherForecastRequest(LocalDate date)
    {
        Date = date;
    }

    [DataMember(Order = 1)]
    public LocalDate? Date { get; }
}

internal sealed class WeatherForecastValidator : AbstractValidator<WeatherForecastRequest>
{
    public WeatherForecastValidator()
    {
        // Note: you could create a ZonedClock and inject it into the constructor
        var today = SystemClock.Instance.InTzdbSystemDefaultZone().GetCurrentDate();
        RuleFor(w => w.Date)
            .NotEmpty()
            .GreaterThanOrEqualTo(today)
            .LessThanOrEqualTo(today.PlusDays(10));
    }
}
