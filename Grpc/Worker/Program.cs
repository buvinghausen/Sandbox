using Grpc.Contracts.Counter;
using Grpc.Worker;

using ProtoBuf.Grpc.ClientFactory;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
            .AddHostedService<Worker>()
            .AddCodeFirstGrpcClient<ICounterService>(o => o.Address = new Uri("https://localhost:5001"));
    })
    .Build();

await host.RunAsync();
