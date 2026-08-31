# Plugin.Maui.MVVMExpress.Dialogs

MAUI `DisplayAlert` adapter for **MVVMExpress**. ViewModels depend on `IDialogs`, not `Page`.

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.MVVMExpress.Dialogs.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Dialogs)

```csharp
public sealed class EditViewModel : ViewModel
{
    private readonly IDialogs _dialogs;
    public EditViewModel(IDialogs dialogs) => _dialogs = dialogs;

    public async Task ConfirmDeleteAsync()
    {
        if (await _dialogs.ConfirmAsync("Delete", "Remove this item?"))
            await DeleteAsync();
    }
}
```

Register `MauiDialogs` as `IDialogs` in the MAUI host. Tests use `NullDialogs` (Core) or `FakeDialogs` ([Testing](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Testing)).

## Install

```bash
dotnet add package Plugin.Maui.MVVMExpress.Dialogs --prerelease
```

Target frameworks: `net10.0`, `net10.0-android` (API 21+), `net10.0-ios` (iOS 15+). Requires the [host](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress) package. Version `0.1.0-preview`.

`MauiDialogs` prefers `Shell.Current.CurrentPage`.

## Related

Product docs: [repository README](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress). Alternatives: CommunityToolkit.Maui popups, Prism dialogs. License: MIT.
