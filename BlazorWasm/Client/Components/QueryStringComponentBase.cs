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

    // Lock the horrors of NameValueCollection away by wrapping the property in a simple indexer get
    protected string this[string key] => QueryString == null
        ? throw new NullReferenceException("Have you forgotten to call base.OnInitialized() in your component?")
        : QueryString[key];

    protected override void OnInitialized()
    {
        base.OnInitialized();
        QueryString = HttpUtility.ParseQueryString(new Uri(Navigation.Uri).Query);
    }
}
