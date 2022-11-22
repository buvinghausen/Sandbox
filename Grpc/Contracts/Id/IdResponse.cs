using System.Runtime.Serialization;

namespace Grpc.Contracts.Id;

[DataContract]
public sealed record IdResponse
{
    public IdResponse()
    {
    }

    public IdResponse(Guid id)
    {
        Id = id;
    }

    [DataMember(Order = 1)]
    public Guid Id { get; init; }
}