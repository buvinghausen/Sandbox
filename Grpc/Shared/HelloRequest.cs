using System.Runtime.Serialization;

namespace Grpc.Shared;

[DataContract]
public sealed record HelloRequest
{
    [DataMember(Order = 1)]
    public string? Name { get; set; }
}