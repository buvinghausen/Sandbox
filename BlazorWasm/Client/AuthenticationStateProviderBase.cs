using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWasm.Client;

/// <summary>
/// This class exists so that the razor components that perform login, logout, & register can cascade the authentication changes
/// </summary>
public abstract class AuthenticationStateProviderBase : AuthenticationStateProvider
{
    // I really wish Microsoft would make this method available on the AuthenticationStateProvider class
    public void NotifyAuthenticationStateChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
