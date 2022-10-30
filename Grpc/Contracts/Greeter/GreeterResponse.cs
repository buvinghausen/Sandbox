using System.Runtime.Serialization;

namespace Grpc.Contracts.Greeter;

[DataContract]
public sealed record GreeterResponse
{
    [DataMember(Order = 1)]
    public string? Message { get; set; }
}
