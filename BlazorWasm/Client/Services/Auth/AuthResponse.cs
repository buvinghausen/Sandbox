using System.Runtime.Serialization;

namespace BlazorWasm.Client.Services.Auth;

[DataContract]
public sealed record AuthResponse
{
    [DataMember(Order = 1)]
    public bool Success { get; init; }
}
