using FluentValidation;

using Grpc.Core;
using Grpc.Core.Interceptors;

namespace Grpc.Server.Interceptors;

// This class must handle all 4 cases
internal sealed class GrpcValidationInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request,
        ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
    {
        await ValidateRequestAsync(request, context).ConfigureAwait(false);
        return await continuation(request, context).ConfigureAwait(false);
    }

    // Streaming on the response side only
    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request,
        IServerStreamWriter<TResponse> responseStream, ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        await ValidateRequestAsync(request, context).ConfigureAwait(false);
        await continuation(request, responseStream, context).ConfigureAwait(false);
    }

    // Streaming on the request side only
    public override Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream, ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation) =>
        continuation(GetStreamValidator(requestStream, context), context);

    // Bi-directional streaming
    public override Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream, ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation) =>
        continuation(GetStreamValidator(requestStream, context), responseStream, context);

    // Core validation function
    private static Task ValidateRequestAsync<T>(T request, ServerCallContext context) where T : class
    {
        var validator = context.GetHttpContext().RequestServices.GetService<IValidator<T>>();
        // If no validator present return CompletedTask otherwise ValidateAndThrow
        return validator == default ? Task.CompletedTask : validator.ValidateAndThrowAsync(request, context.CancellationToken);
    }

    private static IAsyncStreamReader<T> GetStreamValidator<T>(IAsyncStreamReader<T> requestStream, ServerCallContext context)
        where T : class =>
        new AsyncStreamReaderValidator<T>(requestStream, r => ValidateRequestAsync(r, context));
}

// Wrapper to allow validation property by property as the stream proceeds forwards
internal sealed class AsyncStreamReaderValidator<T> : IAsyncStreamReader<T>
{
    private readonly IAsyncStreamReader<T> _innerReader;
    private readonly Func<T, Task> _validator;

    // Make this inaccessible to the IoC
    internal AsyncStreamReaderValidator(IAsyncStreamReader<T> innerReader, Func<T, Task> validator)
    {
        _innerReader = innerReader;
        _validator = validator;
    }

    public T Current => _innerReader.Current;

    public async Task<bool> MoveNext(CancellationToken cancellationToken)
    {
        var success = await _innerReader.MoveNext(cancellationToken).ConfigureAwait(false);
        if (success)
        {
            await _validator.Invoke(Current).ConfigureAwait(false);
        }

        return success;
    }
}
