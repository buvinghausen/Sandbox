using BlazorWasm.App;
using BlazorWasm.App.Grpc;
using BlazorWasm.Client.Services.Auth;
using BlazorWasm.Client.Services.Weather;

using FluentValidation;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using ProtoBuf.Meta;

// Add NodaTime JsonConverters
//JsonSerializerOptions.Default.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
// Add NodaTime support to ProtoBuf
RuntimeTypeModel.Default.AddNodaTime();
// Make fluent validation only return 1x failure per rule (property)
ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

_ = builder.Services
    .AddScoped<AuthenticationStateProvider, ClientAuthenticationStateProvider>() // This is the WASM authentication state provider which talks via gRPC
    .AddValidatorsFromAssemblyContaining<IWeatherForecastService>(includeInternalTypes: true)
    .AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddTransient<GrpcClientInterceptor>()
    .AddOptions()
    .AddAuthorizationCore()
    .AddSingleton<IAuthorizationPolicyProvider, DefaultAuthorizationPolicyProvider>()
    .AddSingleton<IAuthorizationService, DefaultAuthorizationService>()
    .AddGrpcClient<IWeatherForecastService>()
    .AddGrpcClient<IAuthService>();

await builder.Build().RunAsync();
