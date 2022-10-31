using System.ServiceModel;

using ProtoBuf.Grpc;

namespace BlazorWasm.Client.Services.Weather;

[ServiceContract]
public interface IWeatherForecastService
{
    [OperationContract]
    Task<WeatherForecastResponse[]> GetForecastsAsync(WeatherForecastRequest request, CallContext context = default);
}
