# Plugin.Maui.MVVMExpress

MAUI host for **MVVMExpress** on **Android** and **iOS**: DI, main-thread dispatcher, and page lifecycle.

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.MVVMExpress.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress)

Depends on [Plugin.Maui.MVVMExpress.Core](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Core).

```csharp
builder
    .UseMauiApp<App>()
    .UseMvvmExpress();
```

`UseMvvmExpress` calls `AddMvvmExpress()` and replaces `IMainThread` with `MauiMainThread`.

## Install

```bash
dotnet add package Plugin.Maui.MVVMExpress.Core --prerelease
dotnet add package Plugin.Maui.MVVMExpress --prerelease
```

Target frameworks: `net10.0`, `net10.0-android` (API 21+), `net10.0-ios` (iOS 15+). Version `0.1.0-preview`.

## What this package is

- `UseMvvmExpress` on `MauiAppBuilder`
- `MauiMainThread` (`IMainThread`)
- `ViewModelLifecycleBehavior` for page appear / disappear

It is not a navigator or dialog implementation. Add [Navigation](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Navigation) and [Dialogs](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Dialogs) when you need Shell routes or `DisplayAlert`.

Shared / test code can stay on Core + `AddMvvmExpress()` without this package.

## Related

Product docs: [repository README](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress). License: MIT. Niladri Padhy / MauiEssentials.
