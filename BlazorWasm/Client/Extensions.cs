using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace BlazorWasm.Client;

public static class Extensions
{
    public static IReadOnlyDictionary<string, string> ToDictionary(this NameValueCollection value)
    {
        var dict = new Dictionary<string, string>();

        if (value == null)
        {
            return dict;
        }

        foreach (var key in value.AllKeys) dict.Add(key!, value[key]);

        return new ReadOnlyDictionary<string, string>(dict);
    }
}
