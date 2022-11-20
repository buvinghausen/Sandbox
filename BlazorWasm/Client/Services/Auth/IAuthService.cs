using System.ServiceModel;

using ProtoBuf.Grpc;

namespace BlazorWasm.Client.Services.Auth;

[ServiceContract(Name = "grpc.auth.v1.AuthService")]
public interface IAuthService
{
    [OperationContract]
    ValueTask<KeyValuePair<string, string>[]> GetClaimsAsync(CallContext context = default);

    [OperationContract]
    Task<AuthResponse> LoginAsync(LoginRequest request, CallContext context = default);

    [OperationContract]
    Task<AuthResponse> LogoutAsync(CallContext context = default);

    [OperationContract]
    Task<AuthResponse> ManageAsync(ManageRequest request, CallContext context = default);

    [OperationContract]
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CallContext context = default);

    [OperationContract]
    Task<AuthResponse> ResetPasswordAsync(ResetPasswordRequest request, CallContext context = default);

    [OperationContract]
    Task<AuthResponse> ValidateAsync(CallContext context = default);

    [OperationContract]
    Task<AuthResponse> VerifyEmailAsync(VerifyEmailRequest request, CallContext context = default);
}
