using System.Collections.Specialized;
using System.Web;

using Microsoft.AspNetCore.Components;

namespace BlazorWasm.Client.Components;

/// <summary>
/// This component should be inherited from when parsing a querystring is required
/// </summary>
public abstract class QueryStringComponentBase : ComponentBase
{
    [Inject] protected NavigationManager Navigation { get; set; }

    private NameValueCollection QueryString { get; set; }

    // Use the more terse interface by wrapping the key indexer in a try get value
    protected string this[string key] => QueryString[key];

    protected override void OnInitialized()
    {
        base.OnInitialized();
        QueryString = HttpUtility.ParseQueryString(new Uri(Navigation.Uri).Query);
    }
}
