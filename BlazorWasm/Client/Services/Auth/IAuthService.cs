using System.ServiceModel;

using ProtoBuf.Grpc;

namespace BlazorWasm.Client.Services.Auth;

[ServiceContract(Name = "grpc.auth.v1.AuthService")]
public interface IAuthService
{
    [OperationContract]
    Task<AuthResponse> LoginAsync(LoginRequest request, CallContext context = default);

    [OperationContract]
    Task<AuthResponse> LogoutAsync(CallContext context = default);

    [OperationContract]
    Task<AuthResponse> ValidateAsync(CallContext context = default);
}
