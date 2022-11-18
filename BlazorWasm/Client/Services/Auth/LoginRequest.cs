using System.Runtime.Serialization;

namespace BlazorWasm.Client.Services.Auth;

[DataContract]
public sealed class LoginRequest
{
    [DataMember(Order = 1)]
    public string? Username { get; set; }

    [DataMember(Order = 2)]
    public string? Password { get; set; }
}
