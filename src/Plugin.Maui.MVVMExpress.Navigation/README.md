# Plugin.Maui.MVVMExpress.Navigation

Shell navigation host for **MVVMExpress**: map a ViewModel type to a Shell route.

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.MVVMExpress.Navigation.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Navigation)

`INavigator` and `GuardedNavigator` live in Core. This package is `MauiShellNavigator`.

```csharp
var navigator = new MauiShellNavigator()
    .Map<ProductListViewModel>("//products")
    .Map<ProductDetailsViewModel>("details");

await navigator.NavigateToAsync<ProductDetailsViewModel, ProductId>(new(42));
```

Typed args become a query string via `MauiShellNavigator.FormatQuery`. There is no page-stack `INavigation` host yet.

## Install

```bash
dotnet add package Plugin.Maui.MVVMExpress.Navigation --prerelease
```

Target frameworks: `net10.0`, `net10.0-android` (API 21+), `net10.0-ios` (iOS 15+). Requires the [host](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress) package. Version `0.1.0-preview`.

Register `INavigator` as `GuardedNavigator` wrapping `MauiShellNavigator` in the MAUI app (see the sample).

## Related

Prefer Prism.Maui if you need page-stack navigation, not Shell. Product docs: [repository README](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress). License: MIT.
