using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Shared.Greeter;
using Grpc.Shared.Weather;

using NodaTime;

using ProtoBuf.Grpc.Client;
using ProtoBuf.Meta;

// Add NodaTime support to ProtoBuf
RuntimeTypeModel.Default.AddNodaTime();
using var channel = GrpcChannel.ForAddress("https://localhost:5001");
try
{
    Console.WriteLine("What is your name?");
    var name = Console.ReadLine();
    var service = channel.CreateGrpcService<IGreeterService>();
    var response = await service.GetGreetingAsync(new GreeterRequest { Name = name });
    Console.WriteLine($"Greeting: {response.Message}");
}
catch (RpcException e) when (e.StatusCode == StatusCode.InvalidArgument)
{
    foreach (var error in e.Trailers) Console.WriteLine($"Property: {error.Key}\tMessage: {error.Value}");
}
try
{
    Console.WriteLine("Getting weather forecast");
    var service = channel.CreateGrpcService<IWeatherForecastService>();
    var response = await service.GetForecastsAsync(new WeatherForecastRequest { Date = LocalDate.FromDateTime(DateTime.Now) });
    response.ToList().ForEach(f => Console.WriteLine($"Date: {f.Date}\tTemperature (F): {f.TemperatureF}\tSummary: {f.Summary}"));
}
catch (RpcException e) when (e.StatusCode == StatusCode.InvalidArgument)
{
    foreach (var error in e.Trailers) Console.WriteLine($"Property: {error.Key}\tMessage: {error.Value}");
}
Console.WriteLine("Shutting down");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
