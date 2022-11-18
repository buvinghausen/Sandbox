using BlazorWasm.Client.Services.Auth;

using ProtoBuf.Grpc;

namespace BlazorWasm.Server.Services;

internal sealed class AuthService : IAuthService
{
    public Task<AuthResponse> LoginAsync(LoginRequest request, CallContext context = default)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponse> LogoutAsync(CallContext context = default)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResponse> ValidateAsync(CallContext context = default)
    {
        throw new NotImplementedException();
    }
}
