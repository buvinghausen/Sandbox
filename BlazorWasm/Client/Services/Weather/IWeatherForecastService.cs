using System.ServiceModel;

using ProtoBuf.Grpc;

namespace BlazorWasm.Client.Services.Weather;

[ServiceContract(Name = "grpc.weather.v1.WeatherForecastService")]
public interface IWeatherForecastService
{
    [OperationContract]
    ValueTask<WeatherForecastResponse[]> GetForecastsAsync(WeatherForecastRequest request, CallContext context = default);
}
