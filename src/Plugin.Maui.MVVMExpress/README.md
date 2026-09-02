# Plugin.Maui.MVVMExpress

MAUI host for **MVVMExpress**. **Supported: Android + iOS.** Mac Catalyst / Windows compile-only. DI, main-thread marshal, and page lifecycle.

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.MVVMExpress.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress)

Depends on [Plugin.Maui.MVVMExpress.Core](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Core).

```csharp
builder
    .UseMauiApp<App>()
    .UseMvvmExpress(o => o
        .UseNavigationPage()
        .UseDialogs()
        .UseAuth<LoginViewModel>());
```

Shell is optional:

```csharp
builder.UseMvvmExpress(o => o.UseShell().UseDialogs().UseAuth<LoginViewModel>());
```

`UseMvvmExpress` calls `AddMvvmExpress()`, replaces `IMainThread` with `MauiMainThread`, and marshals command/property/navigation work. `UseNavigationPage` / `UseShell` / `UseDialogs` live in the Navigation and Dialogs packages. `UseAuth<TChallenge>()` wraps `GuardedNavigator`. Chat-style apps: [chat-host cookbook](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress/blob/main/docs/chat-host.md).

## Install

```bash
dotnet add package Plugin.Maui.MVVMExpress.Core
dotnet add package Plugin.Maui.MVVMExpress
```

Target frameworks: `net10.0`, `net10.0-android` (API 21+), `net10.0-ios` (iOS 15+). Mac Catalyst / Windows compile-only. Version `1.0.0`.

## What this package is

- `UseMvvmExpress` on `MauiAppBuilder`
- `UseAuth<TChallenge>()` — host auth without reconstructing `GuardedNavigator`
- `MauiMainThread` (`IMainThread`)
- `ViewModelLifecycleBehavior` for page appear / disappear

It is not a navigator or dialog implementation. Add [Navigation](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Navigation) and [Dialogs](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Dialogs) when you need page or Shell routes, toasts, or `DisplayAlert`. `MauiWindowContext` and `MauiVisualTree` resolve the current window/page.

Shared / test code can stay on Core + `AddMvvmExpress()` without this package.

## Related

Product docs: [repository README](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress). License: MIT. Niladri Padhy / MauiEssentials.
