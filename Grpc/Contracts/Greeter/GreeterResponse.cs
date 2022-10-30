using System.Runtime.Serialization;

namespace Grpc.Contracts.Greeter;

[DataContract]
public sealed record GreeterResponse
{
    // All gRPC classes must have a paramterless constructor
    public GreeterResponse()
    {
    }

    public GreeterResponse(GreeterRequest request)
    {
        Message = $"Hello {request.Name}!";
    }

    [DataMember(Order = 1)]
    public string? Message { get; }
}
