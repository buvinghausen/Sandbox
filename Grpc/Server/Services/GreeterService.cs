using FluentValidation;

using Grpc.Contracts.Greeter;

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
        _logger.SendGreeting(request.Name!);
        return Task.FromResult(new GreeterResponse(request));
    }
}

internal sealed class GreeterValidator : AbstractValidator<GreeterRequest>
{
    public GreeterValidator() =>
        RuleFor(g => g.Name)
            .NotEmpty();
}
