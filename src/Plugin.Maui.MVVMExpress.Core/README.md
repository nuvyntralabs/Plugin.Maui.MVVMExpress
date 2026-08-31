# Plugin.Maui.MVVMExpress.Core

UI-framework-free MVVM primitives for **.NET**. No MAUI reference.

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.MVVMExpress.Core.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Core)

`ObservableModel`, `ViewModel` lifecycle, async commands, `AsyncState<T>`, `Outcome`, messaging, `INavigator`, `ICache`, and `IAuthState`.

```csharp
public sealed class HomeViewModel : ViewModel
{
    public AsyncState<IReadOnlyList<Product>> Products { get; } = new();
    public AsyncModelCommand RefreshCommand { get; }

    public HomeViewModel(ICatalog catalog)
    {
        RefreshCommand = new AsyncModelCommand(
            ct => Products.LoadAsync(token => catalog.ListAsync(token), ct));
    }
}
```

## Install

```bash
dotnet add package Plugin.Maui.MVVMExpress.Core --prerelease
```

Target framework: `net10.0`. Version `0.1.0-preview` — APIs may change.

```csharp
services.AddMvvmExpress(); // tests and shared libraries
```

MAUI apps also add [Plugin.Maui.MVVMExpress](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress) and call `UseMvvmExpress()`.

## What this package is

- `ViewModel` / `PageViewModel` with dispose and `ViewModelCancellationToken`
- `ModelCommand` / `AsyncModelCommand` (prevent, cancel-previous, timeout, retry)
- `AsyncState<T>`, `Outcome`, `BusyGate`, `IMessageHub`
- `ObservableRangeCollection<T>` (`AddRange` → one notify)
- Abstractions: `INavigator`, `IDialogs`, `ICache`, `IConnectivityProbe`, `IAuthState`

Do not call `Page.DisplayAlert` or `Shell.Current` from a ViewModel. Use `IDialogs` / `INavigator`.

## Related

Product docs: [repository README](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress). Changelog: [CHANGELOG.md](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress/blob/main/CHANGELOG.md).

Prefer [CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm) for properties and commands only. Production adapters: [SecureSession](https://www.nuget.org/packages/Plugin.Maui.SecureSession), [ApiCache](https://www.nuget.org/packages/Plugin.Maui.ApiCache), [NetworkMonitor](https://www.nuget.org/packages/Plugin.Maui.NetworkMonitor) (Niladri Padhy / MauiEssentials).

License: MIT.
