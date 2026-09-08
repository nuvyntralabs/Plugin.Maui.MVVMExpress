namespace Plugin.Maui.MVVMExpress.Hosting;

/// <summary>
/// Documents the official form XAML path. Do not merge FormValidation into this repo.
/// Use <c>FormViewModel.Field</c> + <c>Bind</c> + <c>Validation.For</c> from Plugin.Maui.FormValidation.
/// </summary>
public static class FormValidationAdapter
{
    /// <summary>NuGet package id for the one-line XAML adapter.</summary>
    public const string PackageId = "Plugin.Maui.FormValidation";
}

/// <summary>Documents the official keyboard adapter. Do not build a keyboard engine here.</summary>
public static class KeyboardManagerAdapter
{
    /// <summary>NuGet package id for composer / form keyboard handling.</summary>
    public const string PackageId = "Plugin.Maui.KeyboardManager";
}
