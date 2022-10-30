using FluentValidation;

using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Grpc.Server.Interceptors;

internal sealed class GrpcExceptionInterceptor : Interceptor
{
    private readonly ILogger<GrpcExceptionInterceptor> _logger;

    public GrpcExceptionInterceptor(ILogger<GrpcExceptionInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
        ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var httpContext = context.GetHttpContext();
        _logger.LogDebug("Starting call. Request: {Path}", httpContext.Request.Path);
        try
        {
            return await continuation(request, context).ConfigureAwait(false);
        }
        catch (ValidationException e)
        {
            // Convert to RpcException and throw
            throw ConvertException(e);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred when calling {Method}", context.Method);
            throw new RpcException(new Status(StatusCode.Internal, e.Message, e));
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request,
        IServerStreamWriter<TResponse> responseStream, ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            await continuation(request, responseStream, context).ConfigureAwait(false);
        }
        catch (ValidationException e)
        {
            // Convert to RpcException and throw
            throw ConvertException(e);
        }
        catch (TaskCanceledException)
        {
            // Ignore this exception
        }
    }

    private static RpcException ConvertException(ValidationException ex)
    {
        var metadata = new Metadata();
        ex.Errors.ToList().ForEach(e => metadata.Add(e.PropertyName, e.ErrorMessage));
        return new RpcException(new Status(StatusCode.InvalidArgument, "Validation failed"), metadata);
    }
}
