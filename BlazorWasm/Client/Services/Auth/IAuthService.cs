using System.ServiceModel;

using ProtoBuf.Grpc;

namespace BlazorWasm.Client.Services.Auth;

[ServiceContract(Name = "grpc.auth.v1.AuthService")]
public interface IAuthService
{
    [OperationContract]
    ValueTask<KeyValuePair<string, string>[]> GetClaimsAsync(CallContext context = default);

    [OperationContract]
    Task<BoolResponse> LoginAsync(LoginRequest request, CallContext context = default);

    [OperationContract]
    Task<BoolResponse> LogoutAsync(CallContext context = default);

    [OperationContract]
    Task<BoolResponse> ManageAsync(ManageRequest request, CallContext context = default);

    [OperationContract]
    Task<BoolResponse> RegisterAsync(RegisterRequest request, CallContext context = default);

    [OperationContract]
    Task<BoolResponse> ResetPasswordAsync(ResetPasswordRequest request, CallContext context = default);

    [OperationContract]
    Task<BoolResponse> ValidateAsync(CallContext context = default);

    [OperationContract]
    Task<BoolResponse> VerifyEmailAsync(VerifyEmailRequest request, CallContext context = default);
}
