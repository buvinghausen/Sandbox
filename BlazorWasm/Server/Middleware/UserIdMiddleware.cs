using System.Security.Claims;

using BlazorWasm.Server.Extensions;

using IdentityModel;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

using SequentialGuid;

namespace BlazorWasm.Server.Middleware;

internal sealed class UserIdMiddleware
{
    private readonly RequestDelegate _next;

    public UserIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Only write the auth cookie when we serve the main app and if not logged in or previously generated
        if (!(context.User.Identity?.IsAuthenticated ?? false) && context.IsAppRoute())
        {
            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(
                    new[]
                    {
                        new Claim(JwtClaimTypes.AuthenticationMethod, "anon"),
                        new Claim(JwtClaimTypes.Id, $"{SequentialGuidGenerator.Instance.NewGuid()}"),
                        new Claim(JwtClaimTypes.Role, "Anonymous")
                    }, CookieAuthenticationDefaults.AuthenticationScheme, JwtClaimTypes.Name, JwtClaimTypes.Role)), // <- Claims identity class is a doofus you must pass the authentication scheme to it or else .NET acts like it's not authenticated
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.MaxValue,
                    IsPersistent = true,
                    IssuedUtc = DateTimeOffset.UtcNow
                }).ConfigureAwait(false);
        }

        // Call the next delegate/middleware in the pipeline
        await _next(context)
            .ConfigureAwait(false);
    }
}
