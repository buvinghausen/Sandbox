using Grpc.Contracts.Id;
using ProtoBuf.Grpc;

using SequentialGuid;

namespace Grpc.Server.Services;

internal sealed class IdService : IIdService
{
    public ValueTask<IdResponse> GetIdAsync(CallContext context = default) =>
        new(new IdResponse(SequentialGuidGenerator.Instance.NewGuid()));

    public ValueTask<TimestampResponse> GetTimestampAsync(TimestampRequest request, CallContext context = default) =>
        new(new TimestampResponse(request.Id.ToDateTime()));
}
