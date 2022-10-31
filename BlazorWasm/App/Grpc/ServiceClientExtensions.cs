using BlazorWasm.App.Grpc;

using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using ProtoBuf.Grpc.Client;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

internal static class ServiceClientExtensions
{
    public static IServiceCollection AddGrpcClient<T>(this IServiceCollection services) where T : class =>
        services.AddTransient(sp =>
            GrpcChannel.ForAddress(sp.GetRequiredService<IWebAssemblyHostEnvironment>().BaseAddress,
                new GrpcChannelOptions
                {
                    HttpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWeb,
                        sp.GetService<HttpClientHandler>() ?? new HttpClientHandler())
                }).Intercept(sp.GetRequiredService<GrpcClientInterceptor>()).CreateGrpcService<T>());
}
