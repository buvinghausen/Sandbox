using System.ServiceModel;

using ProtoBuf.Grpc;

namespace Grpc.Contracts.Greeter;

[ServiceContract(Name = "grpc.greeter.v1.GreeterService")]
public interface IGreeterService
{
    [OperationContract]
    Task<GreeterResponse> GetGreetingAsync(GreeterRequest request, CallContext context = default);
}
