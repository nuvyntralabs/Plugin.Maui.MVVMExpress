# Plugin.Maui.MVVMExpress.Reactive

Optional derived-state package for **MVVMExpress**. System.Reactive is not required by Core.

```csharp
var fullName = PropertyObservable.CombineLatest(
    PropertyObservable.Observe(vm, nameof(vm.First), () => vm.First ?? ""),
    PropertyObservable.Observe(vm, nameof(vm.Last), () => vm.Last ?? ""),
    static (first, last) => $"{first} {last}".Trim());
```

## Install

```bash
dotnet add package Plugin.Maui.MVVMExpress.Reactive --prerelease
```

Target framework: `net10.0`. Version `0.6.1-preview`. Prefer [ReactiveUI](https://www.nuget.org/packages/ReactiveUI) when the app already wants Rx operators.

Product docs: [repository README](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress). License: MIT.
