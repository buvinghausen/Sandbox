using System.ServiceModel;

using ProtoBuf.Grpc;

namespace Grpc.Shared.Weather;

[ServiceContract]
public interface IWeatherForecastService
{
    [OperationContract]
    Task<WeatherForecastResponse[]> GetForecastsAsync(WeatherForecastRequest request, CallContext context = default);
}
