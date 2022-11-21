using BlazorWasm.Client.Services.Id;

using ProtoBuf.Grpc;

using SequentialGuid;

namespace BlazorWasm.Server.Services;

internal sealed class IdService : IIdService
{
    public ValueTask<IdResponse> GetId(CallContext context = default) =>
        new(new IdResponse(SequentialGuidGenerator.Instance.NewGuid()));
}
