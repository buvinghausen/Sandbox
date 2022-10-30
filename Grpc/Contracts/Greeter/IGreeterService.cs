using System.ServiceModel;

using ProtoBuf.Grpc;

namespace Grpc.Contracts.Greeter;

[ServiceContract]
public interface IGreeterService
{
    [OperationContract]
    Task<GreeterResponse> GetGreetingAsync(GreeterRequest request, CallContext context = default);
}
