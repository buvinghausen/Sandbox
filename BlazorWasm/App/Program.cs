using BlazorWasm.App.Grpc;
using BlazorWasm.Client.Services.Auth;
using BlazorWasm.Client.Services.Weather;

using FluentValidation;

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
    .AddValidatorsFromAssemblyContaining<IWeatherForecastService>(includeInternalTypes: true)
    .AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
    .AddTransient<GrpcClientInterceptor>()
    .AddGrpcClient<IWeatherForecastService>()
    .AddGrpcClient<IAuthService>();

await builder.Build().RunAsync();
