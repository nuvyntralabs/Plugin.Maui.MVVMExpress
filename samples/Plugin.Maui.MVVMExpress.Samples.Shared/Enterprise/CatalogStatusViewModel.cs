using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Connectivity;
using Plugin.Maui.MVVMExpress.Flags;

namespace Plugin.Maui.MVVMExpress.Samples.Enterprise;

public sealed class CatalogStatusViewModel : ViewModel
{
    private readonly IConnectivityProbe _connectivity;
    private readonly IFeatureSwitch _flags;

    public CatalogStatusViewModel(IConnectivityProbe connectivity, IFeatureSwitch flags)
    {
        ArgumentNullException.ThrowIfNull(connectivity);
        ArgumentNullException.ThrowIfNull(flags);
        _connectivity = connectivity;
        _flags = flags;
    }

    public bool IsOnline => _connectivity.IsOnline;

    public bool ShowOfflineBanner => _flags.IsEnabled("offline-banner") && !IsOnline;

    public void Refresh()
    {
        Notify(nameof(IsOnline));
        Notify(nameof(ShowOfflineBanner));
    }
}
