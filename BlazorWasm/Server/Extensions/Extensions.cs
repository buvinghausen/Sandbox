namespace BlazorWasm.Server.Extensions;

internal static class Extensions
{
    internal static string[] Prefixes = { "api", "grpc", "healthz", "_" };

    public static bool IsAppRoute(this HttpContext ctx)
    {
        var path = ctx.Request.Path.ToUriComponent();
        return Prefixes.All(p => !path.StartsWith($"/{p}", StringComparison.OrdinalIgnoreCase)) &&
               !Path.HasExtension(path);
    }
}
