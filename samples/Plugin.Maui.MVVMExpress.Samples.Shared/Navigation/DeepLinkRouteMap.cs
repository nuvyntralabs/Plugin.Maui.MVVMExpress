using Plugin.Maui.MVVMExpress.Navigation;
using Result = Plugin.Maui.MVVMExpress.Outcome.Outcome;

namespace Plugin.Maui.MVVMExpress.Samples.Navigation;

/// <summary>
/// Maps an incoming URI to <see cref="INavigator.NavigateToAsync(string, IReadOnlyDictionary{string, object}?, NavOptions?, CancellationToken)"/>.
/// Production apps should compose <c>Plugin.Maui.DeepLinks</c> and call this from the link handler.
/// </summary>
public sealed class DeepLinkRouteMap
{
    public Task<Result> NavigateAsync(Uri uri, INavigator navigator, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(uri);
        ArgumentNullException.ThrowIfNull(navigator);
        var path = uri.AbsolutePath.Trim('/');
        if (string.IsNullOrWhiteSpace(path))
        {
            path = uri.Host;
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            return Task.FromResult(Result.Failure("E_ROUTE", "Deep link has no path."));
        }

        Dictionary<string, object>? query = null;
        if (!string.IsNullOrEmpty(uri.Query))
        {
            query = [];
            var parsed = uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parsed)
            {
                var split = part.Split('=', 2);
                var key = Uri.UnescapeDataString(split[0]);
                var value = split.Length > 1 ? Uri.UnescapeDataString(split[1]) : "";
                query[key] = value;
            }
        }

        return navigator.NavigateToAsync(path, query, options: null, cancellationToken);
    }
}
