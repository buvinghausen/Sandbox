using System.ServiceModel;

using ProtoBuf.Grpc;

namespace Grpc.Shared.Weather;

[ServiceContract]
public interface IWeatherForecastService
{
    [OperationContract]
    Task<WeatherForecast[]> GetForecastAsync(CallContext context = default);
}
