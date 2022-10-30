using System.Runtime.Serialization;

namespace Grpc.Contracts.Greeter;

[DataContract]
public sealed record GreeterRequest
{
    // All gRPC classes must have a paramterless constructor
    public GreeterRequest()
    {
    }

    public GreeterRequest(string? name)
    {
        Name = name;
    }

    [DataMember(Order = 1)]
    public string? Name { get; }
}