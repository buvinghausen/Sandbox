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

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // Get the claims array from the server via gRPC
        var claims = await _authService.GetClaimsAsync();
        // Now just return back the new authentication state
        return new AuthenticationState(
            new ClaimsPrincipal(new ClaimsIdentity(claims.Claims.Select(kvp => new Claim(kvp.Key, kvp.Value)), "Cookies", "name", "role")));
    }
}
