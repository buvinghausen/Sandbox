using System.Security.Claims;

using BlazorWasm.Client.Shared;

using Microsoft.AspNetCore.Authorization;

namespace BlazorWasm.App.Auth;

public static class PolicyExtensions
{
    public static IServiceCollection AddAuthorizationPolicyHandlers(this IServiceCollection services) => services
        .AddSingleton<IAuthorizationHandler, AnonymousHandler>()
        .AddSingleton<IAuthorizationHandler, AuthorizedHandler>()
        .AddSingleton<IAuthorizationHandler, VerifiedHandler>()
        .AddSingleton<IAuthorizationHandler, EmployeeHandler>()
        .AddSingleton<IAuthorizationHandler, AdminHandler>();

    // Function to enable both WASM & server to have the exact same policies
    public static void AddAuthorizationPolicies(this AuthorizationOptions options)
    {
        options.AddPolicy(Policies.Anonymous, policy => policy.Requirements.Add(new AnonymousRequirement()));
        options.AddPolicy(Policies.Authorized, policy => policy.Requirements.Add(new AuthorizedRequirement()));
        options.AddPolicy(Policies.Verified, policy => policy.Requirements.Add(new VerifiedRequirement()));
        options.AddPolicy(Policies.Employee, policy => policy.Requirements.Add(new EmployeeRequirement()));
        options.AddPolicy(Policies.Admin, policy => policy.Requirements.Add(new AdminRequirement()));
        // He we can create composable policies using multiple requirements
        options.AddPolicy(Policies.EmployeeAdmin, policy =>
        {
            policy.Requirements.Add(new AdminRequirement());
            policy.Requirements.Add(new EmployeeRequirement());
        });
    }
}

// All the requirements & handlers can be internal
internal abstract class AuthorizationRequirementBase : IAuthorizationRequirement
{
    private readonly Func<ClaimsPrincipal, bool> _predicate;

    protected AuthorizationRequirementBase(Func<ClaimsPrincipal, bool> predicate)
    {
        _predicate = predicate;
    }

    public Task SetResult(AuthorizationHandlerContext context, IAuthorizationRequirement requirement)
    {
        if (_predicate.Invoke(context.User))
            context.Succeed(requirement);
        else
            context.Fail();
        return Task.CompletedTask;
    }
}

internal sealed class AnonymousRequirement : AuthorizationRequirementBase
{
    public AnonymousRequirement() : base(u => u.IsInRole("Anonymous"))
    {
    }
}

internal sealed class AuthorizedRequirement : AuthorizationRequirementBase
{
    public AuthorizedRequirement() : base(u => !u.IsInRole("Anonymous"))
    {
    }
}

internal sealed class VerifiedRequirement : AuthorizationRequirementBase
{
    public VerifiedRequirement() : base(u => u.HasClaim("email_verified", "true"))
    {
    }
}

internal sealed class EmployeeRequirement : AuthorizationRequirementBase
{
    public EmployeeRequirement() : base(u => u.IsInRole("Employee"))
    {
    }
}

internal sealed class AdminRequirement : AuthorizationRequirementBase
{
    public AdminRequirement() : base(u => u.IsInRole("Admin"))
    {
    }
}

// Handler base marshals the result from the requirement
internal abstract class AuthorizationHandlerBase<T> : AuthorizationHandler<T> where T : AuthorizationRequirementBase
{
    protected sealed override Task HandleRequirementAsync(AuthorizationHandlerContext context, T requirement) =>
        requirement.SetResult(context, requirement);
}

// Handlers become pretty basic at this point they just need to implement the base class
internal sealed class AnonymousHandler : AuthorizationHandlerBase<AnonymousRequirement>
{
}

internal sealed class AuthorizedHandler : AuthorizationHandlerBase<AuthorizedRequirement>
{
}

internal sealed class VerifiedHandler : AuthorizationHandlerBase<VerifiedRequirement>
{
}

internal sealed class EmployeeHandler : AuthorizationHandlerBase<EmployeeRequirement>
{
}

internal sealed class AdminHandler : AuthorizationHandlerBase<AdminRequirement>
{
}