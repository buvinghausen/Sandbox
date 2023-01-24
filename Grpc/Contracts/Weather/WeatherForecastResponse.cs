using NodaTime;

namespace Grpc.Contracts.Weather;

public sealed record WeatherForecastResponse(LocalDate Date, int TemperatureC, string Summary)
{
    /*
     * Thoughts: Read only properties should probably be omitted by default when using constructor initialization?
     * Rationale: The class has everything it needs and both parties client & server can fulfill the contract
     * Client Notes:
     *  This property triggers an InvalidOperationException: No marshaller available for Grpc.Contracts.Weather.WeatherForecastResponse[]
     *  On DefaultClientFactory.CreateClient<TService>(CallInvoker channel)
     * Server Notes:
     *  CodeFirstServiceMethodProvider just ignores the IWeatherForecastService entirely since it only has one method
     */
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
