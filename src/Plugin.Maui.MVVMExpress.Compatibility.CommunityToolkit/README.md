# Plugin.Maui.MVVMExpress.Compatibility.CommunityToolkit

Optional adapters from [CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm) onto MVVMExpress. This package does **not** type-forward `ObservableObject` or `IMessenger`.

```csharp
IMessageHub hub = new CommunityToolkitMessageHub(WeakReferenceMessenger.Default);
```

Install with `--prerelease`. Target: `net10.0`. Version `0.5.0-preview`. Prefer staying on MVVMExpress `IMessageHub` unless you already have CommunityToolkit messaging.

Niladri Padhy / MauiEssentials / Nuvyntra Labs. Alternatives: use one messenger only (CT *or* MVVMExpress).
