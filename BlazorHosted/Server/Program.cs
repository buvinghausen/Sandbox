using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using BlazorHosted.Server.Data;
using BlazorHosted.Server.Models;

using Duende.IdentityServer;

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Enable OpenTelemetry
_ = builder.Services
    .AddOpenTelemetryTracing(trace => trace
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService("HostedBlazor",
                serviceInstanceId: Guid.NewGuid().ToString()))
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
        .AddHttpClientInstrumentation(o => o.RecordException = true)
        .AddEntityFrameworkCoreInstrumentation(o =>
        {
            o.SetDbStatementForStoredProcedure = true;
            o.SetDbStatementForText = true;
        })
        // all available sources
        .AddSource(IdentityServerConstants.Tracing.Basic)
        .AddSource(IdentityServerConstants.Tracing.Cache)
        .AddSource(IdentityServerConstants.Tracing.Services)
        .AddSource(IdentityServerConstants.Tracing.Stores)
        .AddSource(IdentityServerConstants.Tracing.Validation)
        .AddConsoleExporter());


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services
    .AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

builder.Services
    .AddAuthentication()
    .AddIdentityServerJwt();

builder.Services
    .AddControllersWithViews();
builder.Services
    .AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app
        .UseMigrationsEndPoint()
        .UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection()
    .UseBlazorFrameworkFiles()
    .UseStaticFiles()
    .UseRouting()
    .UseIdentityServer()
    .UseAuthorization();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

await app.RunAsync().ConfigureAwait(false);
