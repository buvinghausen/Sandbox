using ProtoBuf.Grpc;
using System.ServiceModel;

namespace Grpc.Shared;

[ServiceContract]
public interface IGreeterService
{
    [OperationContract]
    Task<HelloResponse> SayHelloAsync(HelloRequest request, CallContext context = default);
}
