using System.Runtime.Serialization;

namespace Grpc.Contracts.Id;

[DataContract]
public sealed record TimestampResponse
{
    public TimestampResponse()
    {
    }

    public TimestampResponse(DateTime? timestamp)
    {
        Timestamp = timestamp;
    }

    [DataMember(Order = 1)]
    public DateTime? Timestamp { get; init; }
}
