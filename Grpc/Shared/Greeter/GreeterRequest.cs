using System.Runtime.Serialization;

namespace Grpc.Shared.Greeter;

[DataContract]
public sealed record GreeterRequest
{
    [DataMember(Order = 1)]
    public string? Name { get; set; }
}