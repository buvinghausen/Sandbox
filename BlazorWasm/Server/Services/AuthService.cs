using System.Security.Claims;

using BlazorWasm.Client.Services.Auth;
using BlazorWasm.Client.Shared;

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
    [Authorize] // We only want the generic authorize attribute here because we need to get the claims for all user types
    public ValueTask<KeyValuePair<string, string>[]> GetClaimsAsync(CallContext context = default) =>
        new(context.ServerCallContext!.GetHttpContext().User.Claims
            .Select(c => new KeyValuePair<string, string>(c.Type, c.Value)).ToArray());

    // We only want to allow someone to invoke the login function if they have an anonymous cookie so deny the request even if they are logged in
    [Authorize(Policy = Policies.Anonymous)]
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
                }, CookieAuthenticationDefaults.AuthenticationScheme, JwtClaimTypes.Name, JwtClaimTypes.Role)), // <- Claims identity class is a doofus you must pass the authentication scheme to it or else .NET acts like it's not authenticated
            new AuthenticationProperties
            {
                ExpiresUtc = request.RememberMe ? DateTimeOffset.MaxValue : DateTimeOffset.Now.AddDays(1), IsPersistent = request.RememberMe, IssuedUtc = DateTimeOffset.UtcNow
            }).ConfigureAwait(false);
        // Return the response
        return response;
    }

    // We only want to allow someone to invoke the logout function if they have an authorized cookie
    [Authorize(Policy = Policies.Authorized)]
    public Task<AuthResponse> LogoutAsync(CallContext context = default)
    {
        throw new NotImplementedException();
    }

    [Authorize(Policy = Policies.Authorized)]
    public Task<AuthResponse> ManageAsync(ManageRequest request, CallContext context = default)
    {
        throw new NotImplementedException();
    }

    // We only want to allow someone to invoke the register function if they have an anonymous cookie so deny the request even if they are logged in
    [Authorize(Policy = Policies.Anonymous)]
    public Task<AuthResponse> RegisterAsync(RegisterRequest request, CallContext context = default)
    {
        throw new NotImplementedException();
    }

    // We only want someone to be able to reset the password if they are not logged in
    // If they are logged in they should just update their profile which skips the email verification process
    [Authorize(Policy = Policies.Anonymous)]
    public Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request, CallContext context = default)
    {
        throw new NotImplementedException();
    }

    // This will verify the logged in user is still valid on the server
    // It should be enabled for all authentication types but only perform
    // the nonce claim => security stamp verification for authorized users
    // Anonymous should always return true
    [Authorize]
    public Task<AuthResponse> ValidateAsync(CallContext context = default)
    {
        throw new NotImplementedException();
    }

    // We only want someone who has logged in to be able to verify their email
    [Authorize(Policy = Policies.Authorized)]
    public Task<AuthResponse> VerifyEmailAsync(VerifyEmailRequest request, CallContext context = default)
    {
        throw new NotImplementedException();
    }
}
