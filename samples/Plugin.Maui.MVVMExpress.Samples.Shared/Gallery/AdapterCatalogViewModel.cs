using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Hosting;

namespace Plugin.Maui.MVVMExpress.Samples.Gallery;

/// <summary>
/// Phase 9 adapter catalog. MVVMExpress does not replace these packages;
/// the sample documents the official composition points.
/// </summary>
[RegisterViewModel]
[Route("adapters")]
public sealed class AdapterCatalogViewModel : ViewModel
{
    /// <summary>Official form XAML adapter package.</summary>
    public string FormValidationPackage { get; } = "Plugin.Maui.FormValidation";

    /// <summary>Official keyboard adapter package.</summary>
    public string KeyboardPackage { get; } = "Plugin.Maui.KeyboardManager";

    /// <summary>Official deep-link adapter package.</summary>
    public string DeepLinksPackage { get; } = "Plugin.Maui.DeepLinks";

    /// <summary>Official session adapter package.</summary>
    public string SecureSessionPackage { get; } = "Plugin.Maui.SecureSession";
}
