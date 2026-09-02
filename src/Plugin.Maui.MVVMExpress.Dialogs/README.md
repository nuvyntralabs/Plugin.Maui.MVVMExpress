# Plugin.Maui.MVVMExpress.Dialogs

MAUI `DisplayAlert` and toast adapter for **MVVMExpress**. ViewModels depend on `IDialogs` / `INotifier`, not `Page`. **Supported: Android + iOS.** Mac Catalyst / Windows compile-only. Alerts hop to `IMainThread`.

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.MVVMExpress.Dialogs.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Dialogs)

```csharp
public sealed class EditViewModel : ViewModel
{
  private readonly IDialogs _dialogs;
  private readonly INotifier _notifier;
  public EditViewModel(IDialogs dialogs, INotifier notifier)
  {
    _dialogs = dialogs;
    _notifier = notifier;
  }

  public async Task ConfirmDeleteAsync()
  {
    if (await _dialogs.ConfirmAsync("Delete", "Remove this item?"))
      await DeleteAsync();
    await _notifier.ToastAsync("Deleted");
  }
}
```

Register `MauiDialogs` as `IDialogs` and `MauiNotifier` as `INotifier` in the MAUI host. Tests use `NullDialogs` (Core) or `FakeDialogs` ([Testing](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Testing)). Inject `IToastPresenter` to record toasts without a window.

## Install

```bash
dotnet add package Plugin.Maui.MVVMExpress.Dialogs
```

Target frameworks: `net10.0`, `net10.0-android` (API 21+), `net10.0-ios` (iOS 15+). Mac Catalyst / Windows compile TFMs are present and compile-only. Requires the [host](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress) package. Version `1.0.0`.

`MauiNotifier` toasts use `Window.AddOverlay` and never wrap `Page.Content`.

`MauiDialogs` and `MauiNotifier` resolve the current page from `IWindowContext` (Shell first, then the window's page).

## Related

Product docs: [repository README](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress). Alternatives: CommunityToolkit.Maui popups, Prism dialogs. License: MIT.
