using System.ServiceModel;

using ProtoBuf.Grpc;

namespace BlazorWasm.Client.Services.Auth;

[ServiceContract(Name = "grpc.authentication.v1.AuthenticationService")]
public interface IAuthenticationService
{
    [OperationContract]
    Task<ValidateAuthenticationStateResponse> ValidateAuthenticationStateAsync(CallContext context = default);
}
