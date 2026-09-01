# MAUI 10 Android — do / don't

Device-shaped notes from a WhatsApp-style host on a mid-range Samsung (SM-M066B) with .NET MAUI 10. Several freezes are MAUI controls, not MVVMExpress logic. The framework still steers you into those controls unless you pick the host APIs.

## Do

- Construct pages on `IMainThread`. `MauiPageNavigator` / `MauiShellNavigator` hop before the factory. `NavigationThread.EnsurePageFactoryOnMainThread` throws if they do not.
- After login, `ResetAsync` / `ReplaceRootAsync` (replace `window.Page` with a `NavigationPage`). Do not `InsertPageBefore` + `PopAsync` on every tab.
- One `SectionHostViewModel` for tabs. Switch visibility; do not `GoToAsync` four `//` routes.
- `SnapshotCollection<T>` + seed before bind. After appear, `Add` / `Insert` one row.
- Bind search to `Entry` → `SearchQuery.Text`. Filter from `SearchQuery.CommittedText`.
- Hub / inbox: `CoalescingDispatcher` (marshal + coalesce).
- Keep `MarshalNotifications = true` (default) so commands and properties hop through `IMainThread`. Do not mix MAUI `MainThread` statics in ViewModels.
- Enable `o.EnableDiagnostics = true` in Debug for off-thread navigation breadcrumbs. Pair a production build with [Plugin.Maui.Diagnostics](https://www.nuget.org/packages/Plugin.Maui.Diagnostics) (Niladri Padhy / Nuvyntra Labs) or Crashlytics / Sentry.

## Don't

- `ConfigureAwait(false)` then `new Page()` / `Shell.GoToAsync` without an `IMainThread` hop. That ANRs (`Input dispatching timed out`).
- `UseShell()` + `//chats` `TabBar` as the only host for a WhatsApp-style app. `GoToAsync` hangs or blacks the screen on this class of device.
- `DelegatePagedCollection` + `CollectionView` + `RefreshView` + `RemainingItemsThreshold` when the fetch is `Task.FromResult` / instant. Layout loops.
- Two-way `SearchQuery` on Android `SearchBar`. `TextChanged` during measure loops.
- `ObservableRangeCollection.ReplaceRange` / `BindableLayout` after the page is visible.
- `RefreshAsync` from `OnAppearingAsync` on a live inbox.
- SVG invalidate in a hot header if SurfaceFlinger starts `HighHint` (MAUI 10, not MVVMExpress).
- `MarshalNotifications = false` unless every command, dialog, and navigation hop is yours.
