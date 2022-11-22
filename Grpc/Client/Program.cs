using Grpc.Contracts.Greeter;
using Grpc.Contracts.Id;
using Grpc.Contracts.Weather;
using Grpc.Core;
using Grpc.Health.V1;
using Grpc.Net.Client;

using OpenTelemetry;
using OpenTelemetry.Trace;

using ProtoBuf.Grpc.Client;
using ProtoBuf.Meta;

using StatusCode = Grpc.Core.StatusCode;

// Add NodaTime support to ProtoBuf
RuntimeTypeModel.Default.AddNodaTime();

// Wireup OpenTelemetry
using var tracerProvider = Sdk.CreateTracerProviderBuilder()
    .AddGrpcClientInstrumentation(
        opt => opt.SuppressDownstreamInstrumentation = true)
    .AddHttpClientInstrumentation()
    .AddConsoleExporter()
    .Build();

Console.WriteLine("Press any key to start reading from the server");
Console.Read();

using var channel = GrpcChannel.ForAddress("https://localhost:5001");
// Health Checks
{
    Console.WriteLine("Fetching gRPC service status");
    var service = new Health.HealthClient(channel);
    var response = await service.CheckAsync(new HealthCheckRequest());
    Console.WriteLine($"Status: {response.Status}");
}

// Id service checks to make sure Guids & Time values serialize correctly
// See: gRPC compatibility level: https://protobuf-net.github.io/protobuf-net/compatibilitylevel.html
{
    var service = channel.CreateGrpcService<IIdService>();
    var id = (await service.GetIdAsync()).Id;
    Console.WriteLine($"Id: {id}");
    var time = await service.GetTimestampAsync(new TimestampRequest(id));
    Console.WriteLine($"Timestamp: {time.Timestamp}");
}

try
{
    Console.WriteLine("What is your name?");
    var name = Console.ReadLine();
    var service = channel.CreateGrpcService<IGreeterService>();
    var response = await service.GetGreetingAsync(new GreeterRequest(name));
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
    var response = await service.GetForecastsAsync(new WeatherForecastRequest(DateTime.Now));
    response.ToList().ForEach(f => Console.WriteLine($"Date: {f.Date}\tTemperature (F): {f.TemperatureF}\tSummary: {f.Summary}"));
}
catch (RpcException e) when (e.StatusCode == StatusCode.InvalidArgument)
{
    foreach (var error in e.Trailers) Console.WriteLine($"Property: {error.Key}\tMessage: {error.Value}");
}

Console.WriteLine("Shutting down");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
