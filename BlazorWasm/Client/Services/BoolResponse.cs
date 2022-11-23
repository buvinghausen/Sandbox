using System.Runtime.Serialization;

namespace BlazorWasm.Client.Services;

[DataContract]
public sealed record BoolResponse
{
    public BoolResponse()
    {
    }

    public BoolResponse(bool success)
    {
        Success = success;
    }

    [DataMember(Order = 1)]
    public bool Success { get; init; }
}
