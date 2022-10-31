using FluentValidation;

using Grpc.Server.Interceptors;
using Grpc.Server.Services;

using ProtoBuf.Grpc.Server;
using ProtoBuf.Meta;

// Add NodaTime support to ProtoBuf
RuntimeTypeModel.Default.AddNodaTime();
// Make fluent validation only return 1x failure per rule (property)
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

var builder = WebApplication.CreateBuilder(args);
var isProduction = builder.Environment.IsProduction();
// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
_ = builder.Services
    .AddValidatorsFromAssemblyContaining<GreeterValidator>(includeInternalTypes: true)
    .AddGrpc(o =>
    {
        o.EnableDetailedErrors = !isProduction;
        // Wire up validation and exception handlers (note the sequence here is imperative)
        // The interceptors are invoked in the sequence they are added since the exception
        // Interceptor wraps the validation interceptor and maps the ValidationException
        // to an RpcException it must be added first
        o.Interceptors.Add<GrpcExceptionInterceptor>();
        o.Interceptors.Add<GrpcValidationInterceptor>();
    });
_ = builder.Services
    .AddCodeFirstGrpc(o => o.EnableDetailedErrors = !isProduction);

var app = builder.Build();
// Configure the HTTP request pipeline.
_ = app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
_ = app.MapGrpcService<GreeterService>();
_ = app.MapGrpcService<WeatherForecastService>();
_ = app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

await app.RunAsync();
