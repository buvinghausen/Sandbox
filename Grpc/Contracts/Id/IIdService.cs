using System.ServiceModel;

using ProtoBuf.Grpc;

namespace Grpc.Contracts.Id;

[ServiceContract(Name = "grpc.id.v1.IdService")]
public interface IIdService
{
    [OperationContract]
    ValueTask<IdResponse> GetIdAsync(CallContext context = default);

    [OperationContract]
    ValueTask<TimestampResponse> GetTimestampAsync(TimestampRequest request, CallContext context = default);
}
