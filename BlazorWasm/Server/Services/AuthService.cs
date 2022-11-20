using System.Security.Claims;

using BlazorWasm.Client.Services.Auth;

using FluentValidation;

using Grpc.Core;

using IdentityModel;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

using ProtoBuf.Grpc;

using SequentialGuid;

namespace BlazorWasm.Server.Services;

internal sealed class AuthService : IAuthService
{
    [Authorize]
    public Task<ClaimsResponse> GetClaimsAsync(CallContext context = default) =>
        Task.FromResult(new ClaimsResponse(context.ServerCallContext!.GetHttpContext().User));

    [Authorize(Roles = "Anonymous")]
    public async Task<AuthResponse> LoginAsync(LoginRequest request, CallContext context = default)
    {
        // Check username & password
        var response = new AuthResponse { Success = request.Email == "demo@demo.com" && request.Password == "demo" };
        // If invalid throw ValidationException
        if (!response.Success)
            throw new ValidationException("Invalid email or password");
        // If valid get UserId from anonymous cookie
        var httpCtx = context.ServerCallContext!.GetHttpContext();
        var id = httpCtx.User.FindFirstValue(JwtClaimTypes.Id)!;
        // Now SignOut anonymous cookie
        await httpCtx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        // Now SignIn with user based cookie
        await httpCtx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(new ClaimsIdentity(
                new[]
                {
                    new Claim(JwtClaimTypes.Email, request.Email!),
                    new Claim(JwtClaimTypes.Name, request.Email!.Split('@')[0]),
                    new Claim(JwtClaimTypes.Id, id),
                    new Claim(JwtClaimTypes.Nonce, SequentialGuidGenerator.Instance.NewGuid().ToString())
                }, CookieAuthenticationDefaults.AuthenticationScheme)), // <- Claims identity class is a doofus you must pass the authentication scheme to it or else .NET acts like it's not authenticated
            new AuthenticationProperties
            {
                ExpiresUtc = request.RememberMe ? DateTimeOffset.MaxValue : DateTimeOffset.Now.AddDays(1), IsPersistent = request.RememberMe, IssuedUtc = DateTimeOffset.UtcNow
            }).ConfigureAwait(false);
        // Return the response
        return response;
    }

    // This will log the user out and restore their anonymous userid cookie
    public Task<AuthResponse> LogoutAsync(CallContext context = default)
    {
        throw new NotImplementedException();
    }

    // This will verify the nonce claim is still valid (i.e. someone hasn't logged out on a different circuit/session)
    public Task<AuthResponse> ValidateAsync(CallContext context = default)
    {
        throw new NotImplementedException();
    }
}
