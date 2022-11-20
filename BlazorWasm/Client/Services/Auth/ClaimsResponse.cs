using System.Runtime.Serialization;
using System.Security.Claims;

namespace BlazorWasm.Client.Services.Auth;

[DataContract]
// All his class does is gRPC up the claims payload from the cookie to use in WASM mode
public sealed record ClaimsResponse
{
    public ClaimsResponse()
    {
    }

    public ClaimsResponse(ClaimsPrincipal principal)
    {
        // Convert the claims to a wire friendly format
        Claims = principal.Claims.ToDictionary(k => k.Type, v => v.Value);
    }

    // Note: proto-buf will not deserialize IReadOnlyDictionary<string, string> at present
    [DataMember(Order = 1)]
    public IDictionary<string, string> Claims { get; init; }
}
