using System.Runtime.Serialization;

namespace BlazorWasm.Client.Services.Auth;

[DataContract]
public sealed record ValidateAuthenticationStateResponse
{
    [DataMember(Order = 1)]
    public bool IsValid { get; init; }
}
