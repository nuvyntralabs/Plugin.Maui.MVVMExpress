# Plugin.Maui.MVVMExpress.Navigation

Shell and page navigation hosts for **MVVMExpress**: map a ViewModel type to a Shell route or a `Page`. **Supported: Android + iOS.** Mac Catalyst / Windows compile-only.

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.MVVMExpress.Navigation.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Navigation)

`INavigator`, `IPageNavigator`, `GuardedNavigator`, and the URI stack live in Core. This package is `MauiShellNavigator` and `MauiPageNavigator`.

```csharp
var shell = new MauiShellNavigator()
    .Map<ProductListViewModel>("//products")
    .Map<ProductDetailsViewModel>("details");

await shell.NavigateToAsync<ProductDetailsViewModel, ProductId>(new(42));
await shell.NavigateToAsync("details", new Dictionary<string, object> { ["ProductId"] = 42 });

var pages = new MauiPageNavigator(new WindowContext("main"), services)
    .Map<PageStackViewModel, PageStackPage>("stack")
    .Map<PageStackItemViewModel, PageStackItemPage>("stack-item");

await pages.NavigateToAsync("stack-item", new Dictionary<string, object> { ["Title"] = "Latte" });
await pages.PopToRootAsync();
```

Typed args become a query string via `NavigationRouteTable.FormatQuery`. `INavigator.Stack` / `CanGoBack` / `ReplaceAsync` / `ResetAsync` track the URI stack. Register one navigator per `IWindowContext` with `WindowNavigatorRegistry`.

## Install

```bash
dotnet add package Plugin.Maui.MVVMExpress.Navigation --prerelease
```

Target frameworks: `net10.0`, `net10.0-android` (API 21+), `net10.0-ios` (iOS 15+). Mac Catalyst / Windows compile-only. Requires the [host](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress) package. Version `0.6.0-preview`.

Register `INavigator` as `GuardedNavigator` wrapping `MauiShellNavigator`, and `IPageNavigator` as `MauiPageNavigator`, in the MAUI app (see the sample).

## Related

Prefer Prism.Maui if you need regions. Product docs: [repository README](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress). License: MIT.
