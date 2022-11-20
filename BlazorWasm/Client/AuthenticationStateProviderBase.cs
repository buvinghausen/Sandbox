using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWasm.Client;

public abstract class AuthenticationStateProviderBase : AuthenticationStateProvider
{
    // I really wish Microsoft would make this method available on the AuthenticationStateProvider class
    public void NotifyAuthenticationStateChanged() =>
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
}
