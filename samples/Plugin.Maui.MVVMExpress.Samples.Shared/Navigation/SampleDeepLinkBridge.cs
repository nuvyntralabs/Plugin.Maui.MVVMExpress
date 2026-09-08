using Plugin.Maui.MVVMExpress.Navigation;
using Result = Plugin.Maui.MVVMExpress.Outcome.Outcome;

namespace Plugin.Maui.MVVMExpress.Samples.Navigation;

/// <summary>
/// Phase 9 in-memory <see cref="IDeepLinkBridge"/>. Production apps wrap Plugin.Maui.DeepLinks.
/// </summary>
public sealed class SampleDeepLinkBridge : IDeepLinkBridge
{
    private readonly DeepLinkRouteMap _map = new();

    /// <inheritdoc />
    public Task<Result> NavigateAsync(string uri, INavigator navigator, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);
        ArgumentNullException.ThrowIfNull(navigator);
        if (!Uri.TryCreate(uri, UriKind.Absolute, out var parsed))
        {
            parsed = new Uri("app://local/" + uri.TrimStart('/'), UriKind.Absolute);
        }

        return _map.NavigateAsync(parsed, navigator, cancellationToken);
    }
}
