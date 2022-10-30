using ProtoBuf.Grpc;

using System.ServiceModel;

namespace Grpc.Shared.Greeter;

[ServiceContract]
public interface IGreeterService
{
    [OperationContract]
    Task<GreeterResponse> GetGreetingAsync(GreeterRequest request, CallContext context = default);
}
