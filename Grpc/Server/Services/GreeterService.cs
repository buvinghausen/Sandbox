using Grpc.Shared;

using ProtoBuf.Grpc;

namespace Grpc.Server.Services;

public sealed class GreeterService : IGreeterService
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public Task<HelloResponse> SayHelloAsync(HelloRequest request, CallContext context = default)
    {
        _logger.LogInformation("Sending hello to {Name}", request.Name);
        return Task.FromResult(new HelloResponse { Message = $"Hello {request.Name}!" });
    }
}
