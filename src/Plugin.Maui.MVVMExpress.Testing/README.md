# Plugin.Maui.MVVMExpress.Testing

Test fakes and leak probes for **MVVMExpress** ViewModels.

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.MVVMExpress.Testing.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Testing)

```csharp
var nav = new FakeNavigator();
await nav.NavigateToAsync<HomeViewModel>();

await viewModel.AppearAsync();
await viewModel.DisappearAsync();

var leak = LeakProbe.Track(viewModel);
viewModel.Dispose();
Assert.True(LeakProbe.IsCollected(leak));
```

Also: `FakeDialogs`, `FakeMainThread`, `FakeConnectivity`, `FakeMessageHub`, `ScopedNavigator` (page-scope push/pop + dispose), `ScaleProfile` (Small / Mid / Large list sizes).

## Install

```bash
dotnet add package Plugin.Maui.MVVMExpress.Testing --prerelease
```

Target framework: `net10.0`. Depends on [Core](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Core). Reference from test projects only. Version `0.6.0-preview`.

## Related

Product docs: [repository README](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress). Alternatives: xUnit + hand-rolled fakes, Prism.Maui testing helpers. License: MIT.
