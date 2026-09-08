# Phase 8–10 implementation plan

Work plan for implementing [development-plan.md](development-plan.md) Phases **8**, **9**, and **10** on top of the shipped **1.0.2** contract (Phases 0–7).

This document is the implementation contract. [development-plan.md](development-plan.md) remains the product intent. Do not regenerate the framework. **One phase per change set.** Do not implement Phase 9 types in a Phase 8 PR.

**Baseline:** `1.0.2` (stable, deployed). SemVer lock: shipped 0.6.1 APIs plus `UseAuth<TChallenge>()` from `1.0.0`.  
**Rule:** Phases 8–10 may **add** or **deprecate**. They must not **break** 1.0 types, signatures, or existing sample/test behavior.

---

## 1. Non-negotiables (Phase 0–7 stay green)

| Rule | Meaning |
| --- | --- |
| No breaking 1.0 APIs | Do not rename, remove, or change signatures in `API-DESIGN.md` **Shipped** sections |
| Escape hatches stay | `SetProperty`, `Map<TViewModel, TPage>`, `AddTransient`, `AddGeneratedViewModels()`, `IAcceptNavQuery` remain public and tested |
| Default interface methods for new interface members | Adding a required method to `INavigator` / `IPageNavigator` would break app implementers. Use C# default interface methods (same pattern as `ReplaceRootAsync`) |
| Core stays UI-free | No `Microsoft.Maui.Controls`, CommunityToolkit.Mvvm, Prism, ReactiveUI, FluentValidation, or sibling plugin PackageReferences on Core |
| Sibling plugins are adapters | `UseDeepLinks()` / `UseSecureSessionAuth()` / FormValidation / KeyboardManager live in Host (or optional host extension types). Missing package → clear exception, never a silent no-op |
| No local NuGet publish | Bump `Version` / `PackageVersion` in the plugin repo. GitHub Actions publishes |
| Existing tests are a gate | Every phase PR must keep the 1.0 suite green before new tests are considered sufficient |

### 1.1 Frozen 1.0 surfaces (do not change)

Keep these exactly as shipped. New overloads / optional parameters / default interface members are allowed; existing callers must compile and behave the same.

- `UseMvvmExpress` / `AddMvvmExpress` / `UseAuth<TChallenge>()` / `AddAuth<TChallenge>()`
- `UseNavigationPage` / `UseShell` / `UseDialogs`
- `ObservableModel.SetProperty` / `Notify` / `NotifyDependsOn`
- `[Notify]` / `[NotifyAlso]` / `[ModelCommand]` / `[AsyncModelCommand]` / `[RegisterViewModel]` / `[RegisterView]` / `[Route]` / `[RequiresAuth]` / `[PersistState]`
- `INavigator` (typed args, route+query, `GoBack` / `PopToRoot` / `Replace` / `Reset`)
- `IPageNavigator.ReplaceRootAsync` (DIM → `ResetAsync`)
- `InMemoryNavigator.Map` / `GuardedNavigator`
- `FormViewModel.Field` / `Bind` / dirty `CanNavigateAwayAsync`
- `SectionHostViewModel` / `SnapshotCollection` / `SearchQuery`
- `CommunityToolkitMessageHub`
- Generated `AddGeneratedViewModels()` + `GeneratedRegistrationHooks.Apply`
- Template `dotnet new mvvmexpress` / `mvvmexpress-page` (refine in Phase 8; do not delete)

### 1.2 Already done (do not redo)

These Phase 8 items shipped early in `1.0.1` / `1.0.2`:

- `Plugin.Maui.MVVMExpress.Templates` (`dotnet new mvvmexpress`, `mvvmexpress-page`)
- VS Code / Visual Studio wrappers under `extensions/`
- Template smoke tests in `tests/Plugin.Maui.MVVMExpress.Templates.Tests`

Phase 8 **refines** the template to the one-registration happy path. It does not recreate the template pack.

---

## 2. Compatibility strategy

### 2.1 Additive API patterns

| Need | Safe pattern | Forbidden |
| --- | --- | --- |
| New navigator method | Default interface method + override on `InMemoryNavigator` / `MauiPageNavigator` / `MauiShellNavigator` | Adding an abstract member to `INavigator` |
| New host feature | `MvvmExpressOptions` extension (`UseX()`) via `AddRegistration` | Changing `UseMvvmExpress` required arguments |
| New generator output | Extra generated members / extra file | Changing existing generated property/command names |
| New analyzer | New diagnostic IDs in SourceGenerators (or a new analyzer project referenced the same way) | Errors on existing samples / template (false positives fail the phase) |
| Registration collapse | `UseMvvmExpress` applies generated `[Route]` + `[RegisterView]` so `Map` is optional | Removing `Map` or making `AddGeneratedViewModels()` throw |

### 2.2 Deprecation (not removal)

If a 1.0 path is no longer the *default story*, mark it `[Obsolete]` only when:

1. The replacement is shipped and tested in the same phase
2. The obsolete member still works
3. Docs show the escape-hatch page
4. Existing Samples.Tests that use the old path still pass (update samples to the new default; keep at least one escape-hatch test)

Do **not** obsolete `SetProperty`, `Map`, or `IAcceptNavQuery` in Phase 8. Those stay first-class escape hatches. Phase 10 may make *convention scanning* a DEBUG analyzer error — not a runtime throw on 1.0 apps.

### 2.3 Regression gate (every phase PR)

Run the full 1.0 suite before merging. Minimum:

```bash
dotnet test Plugin.Maui.MVVMExpress.slnx -c Release --nologo
```

Required green projects (Phase 7 lock):

| Project | Why |
| --- | --- |
| `Core.Tests` | Properties, commands, lifecycle, GC, forms, auth |
| `Navigation.Tests` | In-memory + host-thread hops, guards, stacks |
| `Dialogs.Tests` | Alerts, toast overlay (no `Page.Content` wrap) |
| `Validation.Tests` | DataAnnotations |
| `Pagination.Tests` | `PagedCollection` / `SnapshotCollection` |
| `Reactive.Tests` | `CombineLatest` (must stay optional) |
| `Generator.Tests` | Existing `[Notify]` / command / registration snapshots |
| `Compatibility.Tests` | Existing `CommunityToolkitMessageHub` |
| `Samples.Tests` | Basic, CRUD, Navigation, Auth, Offline, Pagination, Reactive, Enterprise, ChatHost, Playground, memory |
| `Templates.Tests` | Instantiated template Core + tests still pass |
| `Integration.Tests` | Host skeleton |

Add `tests/Plugin.Maui.MVVMExpress.Contract.Tests` in **Phase 8 first PR** (before feature work) so later phases cannot silently change 1.0 public signatures.

---

## 3. Sequencing

```
Contract tests (lock 1.0)
        │
        ▼
 Phase 8  ─── 1.1.0  (Default DX)
        │
        ▼
 Phase 9  ─── 1.2.0  (MAUI finished)
        │
        ▼
 Phase 10 ─── 1.3.0  (Proof)
```

| Version | Phase | Ship when |
| --- | --- | --- |
| `1.1.0` | 8 | All Phase 8 exit criteria + regression gate |
| `1.2.0` | 9 | All Phase 9 exit criteria + regression gate + Phase 8 still green |
| `1.3.0` | 10 | All Phase 10 exit criteria + regression gate + Phases 8–9 still green |

Bump `Version` in `Directory.Build.props` (and any per-project `Version`) only at phase ship. Pipeline publishes. Do not `dotnet nuget push` from this workspace.

---

## 4. Phase 8 — Default DX (`1.1.0`)

**Intent:** one obvious path. Hand-written `SetProperty` and `Map` stay as escape hatches.

**Already shipped:** `dotnet new` + IDE extensions. Remaining work is generators-as-default, one registration, CT ViewModel interop, `[NotifyDependsOn]`, one `StateView`, three analyzers, and sample/docs alignment.

### 4.1 Implementation work

#### 8.1 Contract lock (do this first)

New project `tests/Plugin.Maui.MVVMExpress.Contract.Tests`:

- Public-API snapshot (or reflection assert) of shipped types in Core, Host, Navigation, Dialogs, Validation, Pagination, Reactive, Testing, Compatibility
- Assert `UseMvvmExpress`, `UseAuth<TChallenge>`, `INavigator` method set, `FormViewModel.Bind`, `ReplaceRootAsync` still exist
- Fail the build if a 1.0 public member disappears or changes arity

Add the project to `Plugin.Maui.MVVMExpress.slnx`.

#### 8.2 Generators are the default story

- Docs: [getting-started.md](getting-started.md), [cheat-sheet.md](cheat-sheet.md), [cookbook.md](cookbook.md) already show `[Notify]`. Move any remaining handwritten `SetProperty` samples to an **Escape hatch** page
- Samples: new ViewModels in Playground / AuthApp / Shared are `partial` + `[Notify]` / `[AsyncModelCommand]`
- Keep one handwritten VM: `samples/Plugin.Maui.MVVMExpress.Samples.Shared/EscapeHatch/ManualCounterViewModel.cs`

#### 8.3 One registration call

**Gap today:** `UseMvvmExpress` already calls `GeneratedRegistrationHooks.Apply`, but the template and Playground still `Map<TViewModel, TPage>` and `AddTransient<TPage>()`.

**Add (do not remove Map):**

1. Generator: `[RegisterView(typeof(HomePage))]` + `[Route("home")]` + `[RegisterViewModel]` emit page + ViewModel DI and navigator map
2. Host: `UseNavigationPage()` / `UseShell()` without a `configure` callback apply generated maps automatically
3. `Map` remains for tests and apps that are not `partial`

Happy path after 8.3:

```csharp
builder.UseMauiApp<App>().UseMvvmExpress(o => o
    .UseNavigationPage()
    .UseDialogs()
    .UseAuth<LoginViewModel>());
```

No `Map`, no `AddTransient<LoginPage>()`, no second `AddGeneratedViewModels()` in getting started.

#### 8.4 One nav-args path

- Default samples implement `IAcceptNavArgs<T>` only
- Move URI / `IAcceptNavQuery` cookbook content to an **Advanced Shell** chapter in [navigation.md](navigation.md)
- Keep `IAcceptNavQuery` tests in Navigation.Tests (escape hatch)

#### 8.5 One form path

- Official story: `FormField` + `FormViewModel.Bind` + XAML `Validation.For`
- Optional one-line adapter type in Host or Compatibility that documents [Plugin.Maui.FormValidation](https://www.nuget.org/packages/Plugin.Maui.FormValidation) — **no PackageReference from Core**
- Do not merge FormValidation into this repo

#### 8.6 CommunityToolkit ViewModel interop

Extend `Plugin.Maui.MVVMExpress.Compatibility.CommunityToolkit` (messenger already ships):

- A CT `ObservableObject` can take `INavigator` / `IDialogs` in its constructor and navigate / alert without subclassing `PageViewModel`
- Helper (additive): e.g. `CommunityToolkitViewModelServices` or documented ctor injection only — prefer the smallest type
- Do not type-forward `ObservableObject` or `[ObservableProperty]`

#### 8.7 `[NotifyDependsOn]` generator

**Gap today:** `[NotifyAlso("Label")]` is on the *source* field. `FullName` from `First` + `Last` still needs Reactive `CombineLatest` or two `[NotifyAlso]` annotations.

Add (Core + generator):

```csharp
[NotifyDependsOn(nameof(First), nameof(Last))]
public string FullName => $"{First} {Last}";
```

- Lives in Core (`Plugin.Maui.MVVMExpress.ComponentModel`)
- Generator emits `NotifyDependsOn` wiring so the Reactive package is not required
- Keep `CombineLatest` for multi-source / async cases
- Do not take `System.Reactive`

`[NotifyAlso]` stays. New attribute is additive.

#### 8.8 One `StateView` default

Pick **`AsyncStateView`** for templates and getting started. `BusyOverlayBehavior` stays documented as the overlay alternative. Pages must not invent a third `IsBusy` pattern in new samples.

#### 8.9 Three analyzers

Ship in `Plugin.Maui.MVVMExpress.SourceGenerators` (already an analyzer pack — no new getting-started package).

| ID | Severity | Rule |
| --- | --- | --- |
| `MVVME001` | Error | `Shell.Current` or `Page.DisplayAlert` inside a type derived from `ViewModel` / `PageViewModel` / `ObservableModel` |
| `MVVME002` | Warning | `this` captured on a weak `IMessageHub` handler (`Subscribe` lambda that closes over the ViewModel) |
| `MVVME003` | Warning | `new FormField<T>(...)` in a `FormViewModel` without a matching `Bind(...)` |

Do **not** ship `ConfigureAwait`, Android `SearchBar` XAML, or “`Add` in a loop” until Phase 10 (and only if these three stay quiet).

Zero false positives on existing samples and the template is an exit criterion.

#### 8.10 Template refine

`templates/maui-app`:

- ViewModels: `partial` + generators (already mostly true)
- `MauiProgram`: one `UseMvvmExpress` — drop `Map` / `AddTransient<TPage>` once 8.3 works
- Keep login → replace-root, list, form, test project
- Item template `mvvmexpress-page`: emit `[RegisterView]` + `[Route]` so a second `Map` is not required

### 4.2 Phase 8 tests

| Area | Project | Test types (add) |
| --- | --- | --- |
| 1.0 lock | `Contract.Tests` (new) | Public API snapshot; `UseAuth` / `INavigator` / `Bind` / `ReplaceRootAsync` present |
| Registration | `Generator.Tests` | `[RegisterView]` + `[Route]` emit page + VM registration; `AddGeneratedViewModels` still emitted (compat) |
| Registration | `Integration.Tests` | `UseMvvmExpress` + `UseNavigationPage()` with **no** `Map` resolves page + VM and navigates |
| Registration | `Core.Tests` / Navigation | `Map` + `AddTransient` still works (escape hatch) |
| Nav args | `Samples.Tests` | Playground details use `IAcceptNavArgs<T>` only |
| Nav args | `Navigation.Tests` | Existing `IAcceptNavQuery` tests remain |
| Forms | `Core.Tests` | `Bind` + dirty + `CanExecute` (existing) plus “unbound field still works at runtime” |
| Forms | `Samples.Tests` | Edit form uses `Field` + `Bind` only |
| CT interop | `Compatibility.Tests` | `ObservableObject` + injected `INavigator` navigates; `IDialogs` records an alert via `FakeDialogs` |
| `[NotifyDependsOn]` | `Generator.Tests` | Snapshot: `FullName` notifies when `First` or `Last` changes |
| `[NotifyDependsOn]` | `Core.Tests` | Runtime: set `First`, assert `PropertyChanged` for `FullName` (generated or handwritten `NotifyDependsOn`) |
| `[NotifyAlso]` | `Generator.Tests` | Existing snapshot still contains `NotifyDependsOn(nameof(Count), "Label")` |
| StateView | `Samples.Tests` | Playground list binds `AsyncStateView` (or `AsyncState<T>`), not a custom `IsBusy` |
| Analyzer MVVME001 | `Generator.Tests` (analyzer driver) | `Shell.Current.GoToAsync` in a `PageViewModel` → error; same call in a `ContentPage` → no diagnostic |
| Analyzer MVVME002 | analyzer driver | Weak hub `Subscribe((s, m) => this.Name = m)` → warning; static / `Unsubscribe` in `Dispose` pattern documented as OK |
| Analyzer MVVME003 | analyzer driver | `Field("Name")` without `Bind` → warning; with `Bind` → clean |
| False positives | analyzer driver | Run analyzer over `samples/Plugin.Maui.MVVMExpress.Samples.Shared` + template Core — **zero** MVVME001/002/003 |
| Template | `Templates.Tests` | Instantiated app `MauiProgram` has a single `UseMvvmExpress`; Core + tests pass; **no** `Map<` in `MauiProgram.cs` after 8.3 |
| Regression | all 1.0 projects | Full `dotnet test` green |

Suggested new test files (do not rename existing ones):

```
tests/Plugin.Maui.MVVMExpress.Contract.Tests/PublicApiContractTests.cs
tests/Plugin.Maui.MVVMExpress.Generator.Tests/NotifyDependsOnSnapshotTests.cs
tests/Plugin.Maui.MVVMExpress.Generator.Tests/RegisterViewSnapshotTests.cs
tests/Plugin.Maui.MVVMExpress.Generator.Tests/AnalyzerMvvmE001Tests.cs
tests/Plugin.Maui.MVVMExpress.Generator.Tests/AnalyzerMvvmE002Tests.cs
tests/Plugin.Maui.MVVMExpress.Generator.Tests/AnalyzerMvvmE003Tests.cs
tests/Plugin.Maui.MVVMExpress.Generator.Tests/AnalyzerSampleFalsePositiveTests.cs
tests/Plugin.Maui.MVVMExpress.Compatibility.Tests/CommunityToolkitViewModelInteropTests.cs
tests/Plugin.Maui.MVVMExpress.Integration.Tests/GeneratedRegistrationHostTests.cs
tests/Plugin.Maui.MVVMExpress.Samples.Tests/EscapeHatch/ManualCounterTests.cs
tests/Plugin.Maui.MVVMExpress.Core.Tests/ComponentModel/NotifyDependsOnRuntimeTests.cs
```

### 4.3 Phase 8 samples

| Sample | Change |
| --- | --- |
| `samples/Playground` | One `UseMvvmExpress`; generated routes; `IAcceptNavArgs<T>` details; `AsyncStateView` on list; form `Bind` |
| `samples/Plugin.Maui.MVVMExpress.AuthApp` | Same one-call registration; keep `UseAuth<TChallenge>()` |
| `samples/Plugin.Maui.MVVMExpress.Samples.Shared` | New VMs are `partial` + generators |
| `Samples.Shared/EscapeHatch/ManualCounterViewModel.cs` | **New** handwritten `SetProperty` page so 1.0 style stays proven |
| `Samples.Shared/Compatibility/ToolkitInboxViewModel.cs` | **New** CT `ObservableObject` that injects `INavigator` + `IDialogs` |
| `samples/Plugin.Maui.MVVMExpress.Sample` | Align MauiProgram with generated registration (Map remains if needed for mixed pages) |
| `templates/maui-app` | One `UseMvvmExpress`; `[RegisterView]` on pages |

Do not add a new getting-started NuGet.

### 4.4 Phase 8 docs

- [getting-started.md](getting-started.md) — drop `Map` / `AddGeneratedViewModels()` from page 1
- [cheat-sheet.md](cheat-sheet.md) — `[NotifyDependsOn]` vs `[NotifyAlso]`
- [forms.md](forms.md) — FormValidation adapter one-liner; no second product
- [migration-communitytoolkit.md](migration-communitytoolkit.md) — “keep your `ObservableObject`, inject `INavigator`”
- [known-limitations.md](known-limitations.md) — Phase 8 DX unification shipped; list remaining Phase 9 holes
- [FEATURE-MATRIX.md](../FEATURE-MATRIX.md) — status row `1.1.0`; `[NotifyDependsOn]` Yes; analyzers Yes; `dotnet new` already Yes
- [API-DESIGN.md](../API-DESIGN.md) — move new shipped signatures out of Proposed
- [TEST-COVERAGE.md](TEST-COVERAGE.md) — add Phase 8 rows
- [CHANGELOG.md](../CHANGELOG.md) — `1.1.0`
- Hub catalog (`MauiEssentials/llms.txt`, README) after the plugin tag ships

### 4.5 Phase 8 exit

From [development-plan.md](development-plan.md), plus this plan:

- [ ] New sample ViewModels are `partial` + generators; handwritten style only on the escape-hatch page
- [ ] AuthApp / Playground / template register with one `UseMvvmExpress` call
- [ ] Compatibility tests: CT `ObservableObject` navigates and shows a dialog
- [ ] Analyzer tests for MVVME001–003; zero false positives on existing samples + template
- [ ] `dotnet new mvvmexpress` still produces a buildable app and test project
- [ ] Contract tests + full 1.0 suite green
- [ ] Version `1.1.0` in `Directory.Build.props`

---

## 5. Phase 9 — MAUI finished (`1.2.0`)

**Intent:** app-shell completeness. Close holes Prism users miss. Do not copy WPF regions. **Do not start until Phase 8 has shipped.**

### 5.1 Implementation work

#### 9.1 Shell and NavigationPage as equal hosts

- Document and test flyout, tabs, and `//` absolute routes at the same depth as `ResetAsync`
- NavigationPage stays first-class; Shell is no longer “optional / advanced only”
- Additive: Shell host tests + docs. Do not change NavigationPage replace-root behavior

#### 9.2 `SectionHostView`

- New Host control / behavior bound to `SelectCommand` and `CurrentKey`
- Chat host must not use code-behind visibility flippers
- `SectionHostViewModel` (Core, already shipped) stays the ViewModel

#### 9.3 Blessed list stack

First-party behaviors (Host, not Core):

- `ItemsSource` on `SnapshotCollection` / `PagedCollection`
- `IsRefreshing` on `AsyncState` or `RefreshAsync`
- `RemainingItemsThreshold` only when the fetch is async

These are attachable behaviors or documented XAML patterns with tests. Do not change collection APIs.

#### 9.4 SearchBar story

Either:

1. Bind Android `SearchBar` correctly (handler / behavior), **or**
2. Ship `MvvmSearch` over `Entry` + `SearchQuery`

A documented “do not bind SearchBar” is **not** an exit. Update [known-limitations.md](known-limitations.md) and [maui-android.md](maui-android.md).

#### 9.5 Thin modules

```csharp
public interface IModule
{
    void Configure(IServiceCollection services);
}

public static class ModuleExtensions
{
    public static IServiceCollection AddModule<TModule>(this IServiceCollection services)
        where TModule : IModule, new();
}
```

A feature assembly registers its own routes, ViewModels, and services. Not a region catalog. Not Prism `IModule` copy — keep our type names if a collision is likely (`IMvvmModule` / `AddMvvmModule<T>()` is acceptable).

#### 9.6 Modal stack

`INavigator` already has `ModalStack` and `NavOptions.Modal`. `InMemoryNavigator` already pushes modal frames.

Add **default interface methods** (do not break implementers):

```csharp
public interface IPageNavigator : INavigator
{
    Task<Outcome> PushModalAsync<TViewModel>(CancellationToken cancellationToken = default)
        where TViewModel : class, IViewModel
        => NavigateToAsync<TViewModel>(/* modal */); // DIM

    Task<Outcome> PopModalAsync(CancellationToken cancellationToken = default)
        => GoBackAsync(cancellationToken); // DIM when ModalStack is non-empty
}
```

Implement for real on `MauiPageNavigator` (`PushModalAsync` / `PopModalAsync` on `INavigation`). Overlay toast is not a modal. A CommunityToolkit popup adapter may live in a **sample**, not Core.

#### 9.7 `UseDeepLinks()`

Host extension. Maps a URI onto `INavigator`, `[Route]`, and the auth challenge.

- Adapter to [Plugin.Maui.DeepLinks](https://www.nuget.org/packages/Plugin.Maui.DeepLinks)
- ViewModel does not reference DeepLinks types
- Optional package: if the adapter assembly is missing, throw a clear exception
- Core: no PackageReference

Usual alternative: MAUI App Links / Universal Links in the app.

#### 9.8 `UseSecureSessionAuth()`

Host extension. Adapter to [Plugin.Maui.SecureSession](https://www.nuget.org/packages/Plugin.Maui.SecureSession) or MAUI `SecureStorage`.

- In-memory `IAuthState` remains tests-only
- Core: no PackageReference
- Missing package → clear exception

#### 9.9 Keyboard adapter (docs + sample)

Do not build a keyboard engine. Official adapter notes for [Plugin.Maui.KeyboardManager](https://www.nuget.org/packages/Plugin.Maui.KeyboardManager) or MAUI handlers on composer / form pages.

#### 9.10 Two-window sample

Mac Catalyst or Windows using `IWindowContext` / `WindowNavigatorRegistry`. Single-window remains the default path. The sample proves per-window navigation only.

### 5.2 Phase 9 tests

| Area | Project | Test types (add) |
| --- | --- | --- |
| Shell flyout | `Navigation.Tests` | In-memory + host: flyout item → ViewModel; back stack |
| Shell tabs | `Navigation.Tests` | Tab switch does not reset NavigationPage stack |
| Absolute route | `Navigation.Tests` | `//home` / `ResetAsync` parity (same Outcome, same stack) |
| SectionHostView | `Samples.Tests` / Integration | Chat host binds `SectionHostView`; no code-behind visibility |
| List behaviors | `Pagination.Tests` + Host tests | Snapshot + `IsRefreshing`; Paged + async threshold; sync fetch + threshold still documented as invalid |
| Search | `Pagination.Tests` + Samples | `MvvmSearch` / SearchBar: debounce, cancel previous, committed text |
| Modules | `Integration.Tests` | `AddModule<CatalogModule>()` registers route + VM; navigate succeeds |
| Modal | `Navigation.Tests` | `PushModalAsync` / `PopModalAsync` on `InMemoryNavigator` (stack + History.Modal) |
| Modal host | `Navigation.Tests` | `MauiPageNavigator` modal push/pop (window fake / main-thread hop) |
| Deep links | `Integration.Tests` | Mapped URI → `INavigator` + auth challenge; unmapped URI → failure Outcome |
| Deep links missing pkg | `Integration.Tests` | `UseDeepLinks()` without adapter → exception message names the package |
| SecureSession | `Integration.Tests` | `UseSecureSessionAuth()` with a test double `IAuthState`; missing adapter → exception |
| Keyboard | `Samples.Tests` | Form / chat composer sample documents adapter (compile + optional no-op host) |
| Two-window | `Navigation.Tests` | Two `IWindowContext` keys; navigate on A does not change B’s stack |
| Search limitation | docs test / comment | `known-limitations.md` no longer lists “do not bind SearchBar” as the only story |
| Regression | all Phase 7 + 8 projects | Full `dotnet test` green |

Suggested new test files:

```
tests/Plugin.Maui.MVVMExpress.Navigation.Tests/ShellFlyoutTests.cs
tests/Plugin.Maui.MVVMExpress.Navigation.Tests/ShellTabTests.cs
tests/Plugin.Maui.MVVMExpress.Navigation.Tests/AbsoluteRouteTests.cs
tests/Plugin.Maui.MVVMExpress.Navigation.Tests/PageModalStackTests.cs
tests/Plugin.Maui.MVVMExpress.Navigation.Tests/TwoWindowNavigatorTests.cs
tests/Plugin.Maui.MVVMExpress.Integration.Tests/ModuleRegistrationTests.cs
tests/Plugin.Maui.MVVMExpress.Integration.Tests/UseDeepLinksTests.cs
tests/Plugin.Maui.MVVMExpress.Integration.Tests/UseSecureSessionAuthTests.cs
tests/Plugin.Maui.MVVMExpress.Samples.Tests/ChatHost/SectionHostViewTests.cs
tests/Plugin.Maui.MVVMExpress.Samples.Tests/Search/MvvmSearchTests.cs
tests/Plugin.Maui.MVVMExpress.Samples.Tests/Modules/CatalogModuleTests.cs
```

### 5.3 Phase 9 samples

| Sample | Change |
| --- | --- |
| `samples/Playground` or new `samples/ShellPlayground` | Flyout + tabs + `//` absolute route buttons |
| `Samples.Shared/ChatHost` | Bind `SectionHostView` (no visibility flippers) |
| `samples/Plugin.Maui.MVVMExpress.Modules.Catalog` (new class library) | `IModule` registers catalog routes |
| `Samples.Shared/Modals/NoteModalViewModel.cs` | Push/pop modal from a list |
| `Samples.Shared/DeepLinks/DeepLinkMap.cs` | URI → route (test double; production composes Plugin.Maui.DeepLinks) |
| `Samples.Shared/Auth/SecureSessionAuthState.cs` | Test double implementing `IAuthState` shaped like SecureSession |
| `Samples.Shared/Keyboard/ComposerPage` notes | KeyboardManager / handler adapter comments |
| `samples/TwoWindow` (Catalyst/Windows) | Two windows, two navigators via `IWindowContext` |

AuthApp / existing NavigationPage Playground **must keep working** (Phase 7 + 8 path).

### 5.4 Phase 9 docs

- [navigation.md](navigation.md) — Shell flyout/tabs/`//` as equal to NavigationPage
- [cookbook.md](cookbook.md) — tab host uses `SectionHostView`; add modal recipe
- [known-limitations.md](known-limitations.md) — remove SearchBar-only limitation
- New `docs/modules.md`, `docs/deeplinks.md`, `docs/auth-adapters.md` (short)
- FEATURE-MATRIX / API-DESIGN / CHANGELOG → `1.2.0`
- Hub catalog after the tag

### 5.5 Phase 9 exit

- [ ] Shell flyout + tabs + absolute route tests match NavigationPage replace-root coverage
- [ ] Chat host binds `SectionHostView` with no code-behind visibility flippers
- [ ] `IPageNavigator` modal push/pop tests (in-memory + host)
- [ ] `AddModule<T>()` sample: a feature assembly registers its own routes
- [ ] Deep-link + SecureSession adapters are optional `UseX()` extensions; missing package throws
- [ ] Search story is bindable (SearchBar or `MvvmSearch`)
- [ ] Two-window sample exists; default path is still single-window
- [ ] Phase 7 + 8 tests still green
- [ ] Version `1.2.0`

---

## 6. Phase 10 — Proof (`1.3.0`)

**Intent:** maturity numbers and AOT trust. Do this after the API is boring. **Do not start until Phase 9 has shipped.**

### 6.1 Implementation work

1. **Device report** — Android and iOS: startup ms, RSS after 50 push/pop, notify/sec vs CommunityToolkit `[ObservableProperty]`, AOT published size. Manual first; link from [MEMORY-AND-PERFORMANCE.md](../MEMORY-AND-PERFORMANCE.md)
2. **Zero-reflection registration** — convention scanning becomes a **DEBUG analyzer error** (`MVVME010`). Generators + `ModuleInitializer` only. Runtime 1.0 `Map` / `AddTransient` stay supported (they are not convention scanning)
3. **`[Notify]` vs `[ObservableProperty]` BenchmarkDotNet** — if generated notify is slower or fatter, fix before claiming property parity
4. **List analyzers** (only if MVVME001–003 stay quiet):
   - `MVVME011` — `ObservableRangeCollection.Add` in a loop → warning
   - `MVVME012` — `SnapshotCollection` + `BindableLayout` → warning
   - `MVVME013` — `PagedCollection` + sync fetch + `RemainingItemsThreshold` → error
5. **ILLink** for Navigation, Dialogs, Forms, Pagination (Validation already roots DataAnnotations). Trim warning count 0 on a sample `PublishAot` app
6. **Device leak proof** — scoped ViewModel + page collect on a real Android or iOS Back press (not only `LeakProbe`). Optional emulator job later
7. **Production post-mortem** — one real app you control, version + short write-up. A polished clone is a sample, not proof
8. **Support surface** — Discussions, release notes on every package, FEATURE-MATRIX shipped-vs-not on the package page

### 6.2 Phase 10 tests

| Area | Project | Test types (add) |
| --- | --- | --- |
| Convention scan | analyzer driver | Reflection `GetTypes()` registration helper → MVVME010 in DEBUG |
| Convention scan | `Integration.Tests` | Generated `ModuleInitializer` + `UseMvvmExpress` still registers without a scan |
| Map escape | `Navigation.Tests` | Handwritten `Map` still works (not an error) |
| Benchmarks | `benchmarks/` | `[Notify]` vs `[ObservableProperty]` — assert we do not regress after a fix |
| List analyzers | analyzer driver | Add-in-loop / Snapshot+BindableLayout / sync Paged+threshold |
| AOT / trim | `docs/aot.md` + CI or local script | Sample `PublishAot` trim warning count 0 |
| Leak (host-process) | existing `LeakProbe` / `ScopedNavigator` | Still green (1.0 claim) |
| Leak (device) | manual report + optional test notes | Back press collects VM; link from MEMORY-AND-PERFORMANCE |
| Contract | `Contract.Tests` | 1.0 + 1.1 + 1.2 members still present |
| Regression | all prior projects | Full `dotnet test` green |

Suggested new files:

```
tests/Plugin.Maui.MVVMExpress.Generator.Tests/AnalyzerMvvmE010Tests.cs
tests/Plugin.Maui.MVVMExpress.Generator.Tests/AnalyzerListGuidanceTests.cs
docs/device-report.md
docs/production-postmortem.md
```

### 6.3 Phase 10 samples

| Sample | Change |
| --- | --- |
| Existing AOT sample (`docs/aot.md`) | ILLink on Navigation / Dialogs / Forms / Pagination; 0 trimmer warnings |
| Playground | No convention scan; generators + modules only |
| No new “clone app” as proof | Production post-mortem is a doc, not a second Playground |

### 6.4 Phase 10 exit

- [ ] Device report published and linked from MEMORY-AND-PERFORMANCE
- [ ] AOT sample publishes with 0 trimmer warnings
- [ ] Convention scanning is not a supported 1.3 registration path (DEBUG analyzer)
- [ ] FEATURE-MATRIX and package page show 1.3 shipped vs deferred
- [ ] Phase 7–9 tests still green
- [ ] Version `1.3.0`

---

## 7. Sample and test map (all three phases)

### 7.1 Existing samples that must keep working

| Sample | Phase 7 role | Later change |
| --- | --- | --- |
| `Samples.Shared` Basic / CRUD / Navigation / Auth / Offline / Pagination / Reactive / Enterprise | 1.0 scenario suite | Phase 8: generators + one registration where edited; keep scenario tests |
| ChatHost | `SectionHostViewModel` | Phase 9: `SectionHostView` |
| Playground | 15-minute click path | Phase 8 registration; Phase 9 Shell optional extra |
| AuthApp | `UseAuth<TChallenge>()` | Phase 8 one-call; Phase 9 optional SecureSession adapter |
| `Plugin.Maui.MVVMExpress.Sample` | Host smoke | Align, do not delete |
| `templates/maui-app` | Scaffold | Phase 8 refine only |

### 7.2 New samples

| When | Path | Validates |
| --- | --- | --- |
| 8 | `Samples.Shared/EscapeHatch` | Handwritten `SetProperty` still works |
| 8 | `Samples.Shared/Compatibility` | CT `ObservableObject` + `INavigator` / `IDialogs` |
| 9 | `samples/ShellPlayground` or Playground Shell mode | Flyout / tabs / `//` |
| 9 | `samples/Plugin.Maui.MVVMExpress.Modules.Catalog` | `AddModule<T>()` |
| 9 | `Samples.Shared/Modals` | `PushModalAsync` / `PopModalAsync` |
| 9 | `samples/TwoWindow` | Per-window navigator |
| 10 | AOT publish of an existing sample | Trim 0 |

### 7.3 New test projects

| When | Project |
| --- | --- |
| 8 (first) | `tests/Plugin.Maui.MVVMExpress.Contract.Tests` |
| 9 | Module project referenced by Integration / Samples tests (library, not a test host) |
| 8–10 | Analyzer cases stay in `Generator.Tests` unless the analyzer project splits |

---

## 8. What must not land in Phases 8–10

From [development-plan.md](development-plan.md) and [ROADMAP.md](../ROADMAP.md):

| Deferred | Why |
| --- | --- |
| Prism-style regions | Thin modules are enough |
| Built-in auth provider or remote flag service | Adapters only |
| Control library (bottom sheets, chat bubbles) | Different product |
| `System.Reactive` on Core or the default path | Reactive package already optional |
| Extra packages on getting started | Fewer decisions |
| Multi-window as the **default** desktop path | Two-window is a Phase 9 sample only |
| Visual Studio new-MAUI-app wizard listing | Not a schedule we own |
| ReactiveUI `IScreen` as a first-class host | Already deferred |
| Binding debugger visualizer | Already deferred |
| Breaking 1.0 types to “clean up” DX | Deprecate later, never break in 1.x |

---

## 9. How to execute a phase

1. Confirm the change matches the **current** phase (8 until 1.1.0 ships).
2. Add or extend tests **with** the feature (not after).
3. Run the regression gate (`dotnet test` on the slnx).
4. Update FEATURE-MATRIX (`Designed` → `Yes` only with types **and** tests).
5. Update API-DESIGN shipped sections, CHANGELOG, known-limitations.
6. Bump `Version` only when that phase’s exit is met.
7. Do not start the next phase in the same change set.

Hub catalog (`MauiEssentials` README / `llms.txt`) updates after the plugin CI publishes the version. Do not publish nupkgs from this workspace.

---

## 10. Honest bar

Phases 8–9 can make MVVMExpress the best **MAUI application framework** versus Prism, ReactiveUI, and MvvmCross.

Phase 10 is proof (numbers, trim, one production app). It is not a redesign.

Becoming the default versus CommunityToolkit.Mvvm is a different game (years of default-choice gravity). This plan only makes the first hour as small as CommunityToolkit while keeping the 1.0 shell.
