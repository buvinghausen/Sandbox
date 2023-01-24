namespace Grpc.Contracts.Greeter;

public sealed record GreeterResponse(string Message)
{
    public GreeterResponse(GreeterRequest request) : this($"Hello {request.Name}!")
    {
    }
}
