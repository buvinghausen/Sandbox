using BlazorWasm.App.Auth;
using BlazorWasm.Client.Services.Auth;
using BlazorWasm.Client.Services.Weather;
using BlazorWasm.Server;
using BlazorWasm.Server.Extensions;
using BlazorWasm.Server.Interceptors;
using BlazorWasm.Server.Middleware;
using BlazorWasm.Server.Services;

using FluentValidation;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

using ProtoBuf.Grpc.Server;
using ProtoBuf.Meta;

using SequentialGuid;

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
// Add services to the container.
// Enable OpenTelemetry
_ = builder.Services
    .AddOpenTelemetryTracing(trace => trace
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("BlazorWasm",
                serviceInstanceId: SequentialGuidGenerator.Instance.NewGuid().ToString()))
        .AddAspNetCoreInstrumentation(o =>
        {
            o.RecordException = true;
            o.EnableGrpcAspNetCoreSupport = true;
            // OpenTelemetry's Filter is inverted you return true to record and false to filter out
            o.Filter = ctx =>
            {
                var path = ctx.Request.Path.ToUriComponent();
                // Ignore telemetry for Blazor infrastructure items and static files
                return !path.StartsWith("/_", StringComparison.OrdinalIgnoreCase) && !Path.HasExtension(path);
            };
        })
        .AddConsoleExporter());
// Configure Authentication & Authorization
_ = builder.Services
    .AddAuthorizationPolicyHandlers()
    .AddAuthorization(o => o.AddAuthorizationPolicies()) // Note: policies must be available to both server & wasm
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        // Beef up cookie security
        // __Host prefix requires same site & secure
        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Set-Cookie#cookie_prefixes
        o.Cookie.Name = "__Host-BlazorWasm";
        o.Cookie.HttpOnly = true;
        o.Cookie.IsEssential = true;
        o.Cookie.SameSite = SameSiteMode.Strict;
        o.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        o.ExpireTimeSpan = TimeSpan.FromDays(1);
        o.SlidingExpiration = true;
    });

// For server rendering or webassembly pre-rendering wire up the services directly on the server and skip the gRPC serialization and deserialization
_ = builder.Services
    .AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>() // This is the revalidating state provider which will operate for the blazor server stuff
    .AddScoped<IWeatherForecastService, WeatherForecastService>() 
    .AddScoped<IAuthService, AuthService>()
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


// Only add GrpcReflection for non-production
if (!isProduction)
{
    _ = builder.Services
        .AddCodeFirstGrpcReflection();
}
// Need to add RazorPages so that _Host.cshtml & Error.cshtml can execute
_ = builder.Services.AddRazorPages();

var app = builder.Build();
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
    .UseAuthentication()
    .UseAuthorization()
    .UseMiddleware<UserIdMiddleware>() // This will generate an anonymous cookie with a user id
    .UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
// Only use GrpcReflection in non-production
if (!isProduction)
{
    _ = app
        .MapCodeFirstGrpcReflectionService();
}
_ = app.MapRazorPages();
//app.MapControllers();
_ = app.MapGrpcService<AuthService>();
_ = app.MapGrpcService<WeatherForecastService>();
// The fallback path needs to exclude certain path prefixes so we respond correctly with a 404 rather than the UI
_ = app.MapFallbackToPage("{*path:regex(^(?!" + string.Join('|', Extensions.Prefixes) + ").*$)}", "/_Host"); 

await app.RunAsync();
