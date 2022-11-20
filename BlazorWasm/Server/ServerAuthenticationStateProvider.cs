using BlazorWasm.Client.Services.Auth;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;

namespace BlazorWasm.Server;

internal sealed class ServerAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
{
    private readonly IAuthService _authService;

    public ServerAuthenticationStateProvider(IAuthService authService, ILoggerFactory loggerFactory) : base(loggerFactory)
    {
        _authService = authService;
    }

    protected override Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        // TODO: Wire up with IAuthService and perform the validation there
        return Task.FromResult(true);
    }

    // Check the auth state every 5 minutes
    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(5);
}
