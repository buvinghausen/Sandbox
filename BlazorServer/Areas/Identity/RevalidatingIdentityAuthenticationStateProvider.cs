using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BlazorServer.Areas.Identity;

public class RevalidatingIdentityAuthenticationStateProvider<TUser>
    : RevalidatingServerAuthenticationStateProvider where TUser : class
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IdentityOptions _options;

    public RevalidatingIdentityAuthenticationStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory scopeFactory,
        IOptions<IdentityOptions> optionsAccessor)
        : base(loggerFactory)
    {
        _scopeFactory = scopeFactory;
        _options = optionsAccessor.Value;
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        // Get the user manager from a new scope to ensure it fetches fresh data
        var scope = _scopeFactory.CreateScope();
        try
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
            var user = await userManager.GetUserAsync(authenticationState.User).ConfigureAwait(false);
            return user != null && (!userManager.SupportsUserSecurityStamp ||
                                    authenticationState.User.FindFirstValue(_options.ClaimsIdentity
                                        .SecurityStampClaimType) ==
                                    await userManager.GetSecurityStampAsync(user));
        }
        finally
        {
            if (scope is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else
            {
                scope.Dispose();
            }
        }
    }
}
