using FluentValidation;

using Grpc.Server.Interceptors;
using Grpc.Server.Services;

using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using ProtoBuf.Grpc.Server;
using ProtoBuf.Meta;

using SequentialGuid;

// Add NodaTime support to ProtoBuf
RuntimeTypeModel.Default.AddNodaTime();
// Make fluent validation only return 1x failure per rule (property)
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

var builder = WebApplication.CreateBuilder(args);
var isProduction = builder.Environment.IsProduction();
builder.WebHost
    .UseKestrel(kso => kso.ListenAnyIP(5001, lo =>
    {
        lo.Protocols = HttpProtocols.Http1AndHttp2AndHttp3;
        lo.UseHttps();
    }));

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
// 9ntonio uncomment the following 2 lines and try again
//builder.WebHost.UseKestrel(server => server.ConfigureEndpointDefaults(listenOptions =>
//    listenOptions.Use(next => new ClearTextHttpMultiplexingMiddleware(next).OnConnectAsync)));

// Add services to the container.
_ = builder.Services
    .AddHttpContextAccessor()
    .AddValidatorsFromAssemblyContaining<GreeterValidator>(includeInternalTypes: true)
    .AddOpenTelemetry()
    .WithTracing(trace => trace
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(builder.Environment.ApplicationName, serviceInstanceId: SequentialGuidGenerator.Instance.NewGuid().ToString()))
        .AddAspNetCoreInstrumentation(o =>
        {
            o.RecordException = true;
            o.EnableGrpcAspNetCoreSupport = true;
        })
        .AddConsoleExporter());
_ = builder.Services
    .AddCodeFirstGrpc(o =>
    {
        o.EnableDetailedErrors = !isProduction;
        // Wire up validation and exception handlers (note the sequence here is imperative)
        // The interceptors are invoked in the sequence they are added since the exception
        // Interceptor wraps the validation interceptor and maps the ValidationException
        // to an RpcException it must be added first
        o.Interceptors.Add<GrpcExceptionInterceptor>();
        o.Interceptors.Add<GrpcValidationInterceptor>();
    });
// Add real checks here but this at least gives us the infrastructure
_ = builder.Services
    .AddGrpcHealthChecks()
    .AddCheck("Demo", () => HealthCheckResult.Healthy());

if (!isProduction)
    _ = builder.Services.AddCodeFirstGrpcReflection();
var app = builder.Build();

// Configure the HTTP request pipeline.
_ = app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
// Only use GrpcReflection in non-production
if (!isProduction) _ = app.MapCodeFirstGrpcReflectionService();
_ = app.MapGrpcHealthChecksService();
_ = app.MapGrpcService<GreeterService>();
_ = app.MapGrpcService<IdService>();
_ = app.MapGrpcService<WeatherForecastService>();
_ = app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

await app.RunAsync();
