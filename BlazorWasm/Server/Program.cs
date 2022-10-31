using BlazorWasm.Client.Services.Weather;
using BlazorWasm.Server.Interceptors;
using BlazorWasm.Server.Middleware;
using BlazorWasm.Server.Services;

using FluentValidation;

using ProtoBuf.Grpc.Server;
using ProtoBuf.Meta;

// It's 2022 and System.Text.Json is still broken :(
//JsonSerializerOptions.Default.Converters.Add(new JsonStringEnumConverter());
// Add NodaTime JsonConverters
//JsonSerializerOptions.Default.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
// Add NodaTime support to ProtoBuf
RuntimeTypeModel.Default.AddNodaTime();
// Make fluent validation only return 1x failure per rule (property)
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

var builder = WebApplication.CreateBuilder(args);
var isProduction = builder.Environment.IsProduction();
// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
// 9ntonio uncomment the following 2 lines and try again
//builder.WebHost.UseKestrel(server => server.ConfigureEndpointDefaults(listenOptions =>
//    listenOptions.Use(next => new ClearTextHttpMultiplexingMiddleware(next).OnConnectAsync)));

// Add services to the container.
// Only add GrpcReflection for non-production
if (!isProduction)
{
    _ = builder.Services
        .AddGrpcReflection();
}

_ = builder.Services
    .AddTransient<IWeatherForecastService, WeatherForecastService>() // gRPC services should be wired up as transient without the gRPC ceremony for pre-rendering
    .AddValidatorsFromAssemblyContaining<IWeatherForecastService>(includeInternalTypes: true)
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

// Need to add RazorPages so that _Host.cshtml & Error.cshtml can execute
_ = builder.Services.AddRazorPages();

var app = builder.Build();
// Only use GrpcReflection in non-production
if (!isProduction)
{
    _ = app
        .MapGrpcReflectionService();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app
        .UseExceptionHandler("/Error")
        .UseHsts(); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
}

app
    .UseHttpsRedirection()
    .UseBlazorFrameworkFiles()
    .UseStaticFiles()
    .UseRouting()
    .UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
_ = app.MapRazorPages();
//app.MapControllers();
_ = app.MapGrpcService<WeatherForecastService>();
_ = app.MapFallbackToPage("/_Host");

await app.RunAsync();
