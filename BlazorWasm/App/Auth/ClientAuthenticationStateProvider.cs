using System.Security.Claims;

using BlazorWasm.Client;
using BlazorWasm.Client.Services.Auth;

using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWasm.App.Auth;

internal sealed class ClientAuthenticationStateProvider : AuthenticationStateProviderBase
{
    private readonly IAuthService _authService;

    public ClientAuthenticationStateProvider(IAuthService authService)
    {
        _authService = authService;
    }

    // Get the claims array from the server via gRPC and hand the browser the synthesized AuthenticationState
    public override async Task<AuthenticationState> GetAuthenticationStateAsync() =>
        new(new ClaimsPrincipal(new ClaimsIdentity(
            (await _authService.GetClaimsAsync()).Select(kvp => new Claim(kvp.Key, kvp.Value)),
            "Cookies", // CookieAuthenticationDefaults.AuthenticationScheme
            "name", // JwtClaimTypes.Name
            "role"))); // JwtClaimTypes.Role
}
