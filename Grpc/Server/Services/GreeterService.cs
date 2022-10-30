using Grpc.Shared.Greeter;

using ProtoBuf.Grpc;

namespace Grpc.Server.Services;

internal sealed class GreeterService : IGreeterService
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public Task<GreeterResponse> GetGreetingAsync(GreeterRequest request, CallContext context = default)
    {
        _logger.LogInformation("Sending hello to {Name}", request.Name);
        return Task.FromResult(new GreeterResponse { Message = $"Hello {request.Name}!" });
    }
}
