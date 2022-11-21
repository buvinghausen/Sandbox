using System.ServiceModel;

using ProtoBuf.Grpc;

namespace BlazorWasm.Client.Services.Id;

[ServiceContract(Name = "grpc.id.v1.IdService")]
public interface IIdService
{
    [OperationContract]
    ValueTask<IdResponse> GetId(CallContext context = default);
}
