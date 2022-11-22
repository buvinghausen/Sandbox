using BlazorWasm.Client.Services.Id;

using Microsoft.AspNetCore.Authorization;

using ProtoBuf.Grpc;

using SequentialGuid;

namespace BlazorWasm.Server.Services;

[Authorize]
internal sealed class IdService : IIdService
{
    public ValueTask<IdResponse> GetIdAsync(CallContext context = default) =>
        new(new IdResponse(SequentialGuidGenerator.Instance.NewGuid()));

    public ValueTask<TimestampResponse> GetTimestampAsync(TimestampRequest request, CallContext context = default) =>
        new(new TimestampResponse(request.Id.ToDateTime()));
}
