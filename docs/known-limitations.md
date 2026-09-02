# Known limitations (1.0.0)

**1.0.0 design-review sign-off (2026-09-02).** Shipped public APIs in [API-DESIGN.md](../API-DESIGN.md) plus `UseAuth<TChallenge>()` are the SemVer contract. Deprecations only after 1.0. Breaking change = major version. Phase 8 DX unification is not part of this lock.

**Host footguns (lists / MAUI 10 Android):**

1. **Do not** pair `DelegatePagedCollection` with `CollectionView` + `RemainingItemsThreshold` when the fetch is sync. Use `SnapshotCollection<T>`. See [maui-android.md](maui-android.md).
2. Bind `SearchQuery.Text` to `Entry`, not Android `SearchBar`.
3. `OnAppearingAsync` does not refresh. Chat lists load once and mutate locally.

**Host footguns (0.5.0-preview consumers):**

1. **Command threading** — `0.5.0-preview` raised `CanExecuteChanged` off the UI thread after `ConfigureAwait(false)`. Wrap commands or hop yourself. **Fixed in 0.6.0-preview** (`NotificationMarshaller` + `MauiMainThread`). `CanExecuteChanged` is also a **weak event** so a long-lived command does not pin a Button or popped page.
2. **Resource timing** — do not inject `AppShell` into `App` before `InitializeComponent()`. Resolve the shell in `CreateWindow` from `IServiceProvider`.
3. **Dirty-guard silence** — `0.5.0-preview` returned `E_GUARD` with no UI. **0.6** confirms via `IDialogs` when dialogs are injected (`DirtyNavigationMode.SilentBlock` for tests).

**Accepted 1.0 scope** (not open work):

- Shipped public APIs in [API-DESIGN.md](../API-DESIGN.md) are the 1.0 contract. Breaking changes after 1.0.0 follow SemVer.
- Device RSS and 50k `CollectionView` scroll: the 1.0 claim is host-process BenchmarkDotNet, `ScaleProfile` (Small / Mid / Large), and virtualization rules in [MEMORY-AND-PERFORMANCE.md](../MEMORY-AND-PERFORMANCE.md). Hardware RSS / on-device scroll is out of catalog scope.
- `ViewModelLifecycleBehavior` attach/detach on a MAUI window: the 1.0 claim is in-memory pop-GC (`ScopedNavigator` + Core leak tests). A device/window detach run is out of catalog scope.
- Mac Catalyst and Windows are single-window host targets (shared MAUI APIs on Host / Navigation / Dialogs). Defaults still use `Windows[0]` / `Shell.Current`. Multi-window, `Window.AddOverlay` toast QA, and a Windows nupkg RID when packing on macOS are not a separate desktop product. Sibling adapters (KeyboardManager, DeepLinks, SecureSession, FormValidation, NetworkMonitor) remain Android + iOS.
- Prism regions, ReactiveUI `IScreen`, remote flag/auth providers, bottom-sheet controls, and a VS binding visualizer stay deferred (see [ROADMAP.md](../ROADMAP.md)).
