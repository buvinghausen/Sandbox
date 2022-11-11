using System.ServiceModel;

using ProtoBuf.Grpc;

namespace Grpc.Contracts.Weather;

[ServiceContract(Name = "grpc.weather.v1.WeatherForecastService")]
public interface IWeatherForecastService
{
    [OperationContract]
    Task<WeatherForecastResponse[]> GetForecastsAsync(WeatherForecastRequest request, CallContext context = default);
}
