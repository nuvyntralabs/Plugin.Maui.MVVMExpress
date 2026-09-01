# Reactive derived state

Phase 3. `Plugin.Maui.MVVMExpress.Reactive` does **not** take System.Reactive. Core stays Rx-free.

```csharp
using Plugin.Maui.MVVMExpress.Reactive;

_fullName = PropertyObservable.CombineLatest(
    PropertyObservable.Observe(this, nameof(First), () => First ?? ""),
    PropertyObservable.Observe(this, nameof(Last), () => Last ?? ""),
    static (first, last) => $"{first} {last}".Trim());

_fullName.Subscribe(_ => Notify(nameof(FullName)));
```

Search debounce remains `SearchQuery` in Pagination (no MAUI). Use ReactiveUI when the app already wants Rx operators everywhere.

```bash
dotnet add package Plugin.Maui.MVVMExpress.Reactive --prerelease
```
