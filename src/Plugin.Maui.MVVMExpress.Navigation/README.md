# Plugin.Maui.MVVMExpress.Navigation

Shell and page navigation hosts for **MVVMExpress**: map a ViewModel type to a Shell route or a `Page`. **Supported:** Android, iOS, Mac Catalyst, and Windows (single-window).

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.MVVMExpress.Navigation.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Navigation)

`INavigator`, `IPageNavigator`, `GuardedNavigator`, and the URI stack live in Core. This package is `MauiShellNavigator` and `MauiPageNavigator`. **Pages are constructed on `IMainThread`.**

```csharp
builder.UseMvvmExpress(o => o.UseNavigationPage((nav, _) => nav
  .Map<LoginViewModel, LoginPage>("login")
  .Map<HomeViewModel, HomePage>("home")).UseDialogs());

await Navigator.ResetAsync<HomeViewModel>(); // replace-root after login
await Navigator.NavigateToAsync<DetailsViewModel, DetailsArgs>(new(42));
```

Shell is optional (`UseShell()`). `ResetAsync` / `ReplaceRootAsync` replace `window.Page` with a `NavigationPage`.

Typed args become a query string via `NavigationRouteTable.FormatQuery`. `INavigator.Stack` / `CanGoBack` / `ReplaceAsync` / `ResetAsync` track the URI stack. Register one navigator per `IWindowContext` with `WindowNavigatorRegistry`.

## Install

```bash
dotnet add package Plugin.Maui.MVVMExpress.Navigation
```

Target frameworks: `net10.0`, `net10.0-android` (API 21+), `net10.0-ios` (iOS 15+), `net10.0-maccatalyst` (15+), and `net10.0-windows10.0.19041.0` (packed on Windows; otherwise `net10.0`). Requires the [host](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress) package. Version `1.0.0`.

Prefer `UseNavigationPage()` for login → host → push. Wrap with `GuardedNavigator` when routes need auth. See the [chat-host cookbook](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress/blob/main/docs/chat-host.md).

## Related

Prefer Prism.Maui if you need regions. Product docs: [repository README](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress). License: MIT.
