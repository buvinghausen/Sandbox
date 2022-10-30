using Grpc.Net.Client;
using Grpc.Shared.Greeter;
using Grpc.Shared.Weather;

using ProtoBuf.Grpc.Client;
using ProtoBuf.Meta;

// Add NodaTime support to ProtoBuf
RuntimeTypeModel.Default.AddNodaTime();

Console.WriteLine("What is your name?");
var name = Console.ReadLine();
using var channel = GrpcChannel.ForAddress("https://localhost:5001");
var greeter = channel.CreateGrpcService<IGreeterService>();
var greeting = await greeter.GetGreetingAsync(new GreeterRequest { Name = name });
Console.WriteLine($"Greeting: {greeting.Message}");
Console.WriteLine("Getting weather forecast");
var weather = channel.CreateGrpcService<IWeatherForecastService>();
var forecasts = await weather.GetForecastAsync();
forecasts.ToList().ForEach(f => Console.WriteLine($"Date: {f.Date}\tTemperature (F): {f.TemperatureF}\tSummary: {f.Summary}"));
Console.WriteLine("Shutting down");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
