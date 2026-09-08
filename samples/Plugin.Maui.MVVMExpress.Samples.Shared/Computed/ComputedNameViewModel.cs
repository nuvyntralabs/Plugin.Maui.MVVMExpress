using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Hosting;

namespace Plugin.Maui.MVVMExpress.Samples.Computed;

/// <summary>Phase 8: <c>[NotifyDependsOn]</c> without the Reactive package.</summary>
[RegisterViewModel]
[Route("computed")]
public partial class ComputedNameViewModel : ViewModel
{
    [Notify]
    private string _first = "Ada";

    [Notify]
    private string _last = "Lovelace";

    /// <summary>Computed display name.</summary>
    [NotifyDependsOn(nameof(First), nameof(Last))]
    public string FullName => $"{First} {Last}".Trim();
}
