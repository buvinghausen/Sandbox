using System.Runtime.Serialization;

namespace Grpc.Shared;

[DataContract]
public sealed record HelloResponse
{
    [DataMember(Order = 1)]
    public string? Message { get; set; }
}
