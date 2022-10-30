using System.Runtime.Serialization;

namespace Grpc.Contracts.Greeter;

[DataContract]
public sealed record GreeterRequest
{
    [DataMember(Order = 1)]
    public string? Name { get; set; }
}