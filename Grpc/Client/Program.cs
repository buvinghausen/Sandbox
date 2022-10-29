using Grpc.Net.Client;
using Grpc.Shared;

using ProtoBuf.Grpc.Client;

Console.WriteLine("What is your name?");
var name = Console.ReadLine();
using var channel = GrpcChannel.ForAddress("https://localhost:5001");
var client = channel.CreateGrpcService<IGreeterService>();
var reply = await client.SayHelloAsync(new HelloRequest { Name = name });
Console.WriteLine($"Greeting: {reply.Message}");
Console.WriteLine("Shutting down");
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
