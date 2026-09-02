# MVVMExpress development plan (post-0.6.1)

Work plan for the next phases after **0.6.1-preview**. Phases 0–5 in [ROADMAP.md](../ROADMAP.md) and [DESIGN-PLAN.md](../DESIGN-PLAN.md) are shipped. This document is the current implementation contract.

**Product:** Plugin.Maui.MVVMExpress  
**Baseline:** `0.6.1-preview`  
**Rule:** do not implement a later phase in the same change as an earlier one.

The goal is a stable 1.0, one obvious path, a Prism-level application shell, and a cheap exit from CommunityToolkit.Mvvm. It is **not** “beat CommunityToolkit on every comparison cell.” CommunityToolkit is a ViewModel library. MVVMExpress is a MAUI application framework. Win that frame against Prism.Maui, ReactiveUI, and MvvmCross. Treat CommunityToolkit as the migration source.

---

## Goal and non-goals

| Do | Do not |
| --- | --- |
| Make the first hour as small as CommunityToolkit, while keeping the shell that already exists | Redesign the 0.6.1 API in the 1.0 lock |
| Freeze a boring SemVer contract | Fold “generators only,” one registration, and “convention scanning is an error” into 1.0 |
| Auth as a host feature (`UseAuth<T>`) | Ship a built-in auth provider or remote flag service |
| Sibling adapters (DeepLinks, SecureSession, FormValidation, KeyboardManager) | Add more packages to getting started |
| Thin modules + modal stack | Prism-style regions |
| Manual device numbers after the API is frozen | Block 1.0 on Windows / Mac Catalyst or emulator RSS CI |

---

## Current baseline

Shipped and in scope to keep:

- `UseMvvmExpress` + `UseNavigationPage` / `UseShell` / `UseDialogs`
- `[Notify]`, `[ModelCommand]`, `[AsyncModelCommand]`, `[RegisterViewModel]`, `[Route]`, `[RequiresAuth]`, `ModuleInitializer` routes
- `FormViewModel.Field` + `Bind` (do not write a manual `PropertyChanged` wrapper)
- `NotifyDependsOn` / `[NotifyAlso]`, `IPropertyObservable` / `CombineLatest`
- `AsyncStateView`, `BusyOverlayBehavior`
- `IAcceptNavArgs<T>` and `IAcceptNavQuery`
- `GuardedNavigator` + in-memory `IAuthState` (samples / tests)
- `Compatibility.CommunityToolkit` messenger bridge
- Host-process BenchmarkDotNet, `ScaleProfile`, in-memory `LeakProbe`
- Android + iOS first; Windows / Mac Catalyst compile-only

The gaps are default developer experience, one vocabulary, 1.0 trust, and a few Prism holes (modal stack, feature modules, production auth / deep-link adapters).

---

## Phase map

| Phase | Version | Outcome | Depends on |
| --- | --- | --- | --- |
| **6 — Default story** | Docs on 0.6.1 (optional `0.6.2-preview` docs-only) | A new hire can finish the first hour without a feature matrix | — |
| **7 — 1.0 lock** | `1.0.0` | Stable contract + `UseAuth<TChallenge>()` | Phase 6 |
| **8 — Default DX** | `1.1.0` | One path for ViewModels, registration, forms, nav-args | Phase 7 |
| **9 — MAUI finished** | `1.2.0` | Shell parity, modules, modal stack, sibling host adapters | Phase 8 |
| **10 — Proof** | `1.3.0` | Public numbers, trim, device Back-press GC, one production post-mortem | Phase 9 |

Phase 6 is docs only. Phase 7 adds one convenience API and freezes the rest. Phases 8–10 may deprecate, not break, 1.0 types.

---

## Phase 6 — Default story

**Intent:** understanding first. No API break. Generators already exist; they are not the first-page story.

### Deliverables

1. Rewrite [getting-started.md](getting-started.md) as a 15-minute path:
   - Page 1: `partial` ViewModel + `[Notify]` + `[AsyncModelCommand]`
   - Page 2: `NavigateToAsync<T>`
   - Page 3: `IDialogs`
   - Page 4: `FormViewModel` + `Bind`
2. Hide until a later chapter: `Outcome`, `IAcceptNavQuery`, `SectionHostViewModel`, `CoalescingDispatcher`, Snapshot vs Paged, `GuardedNavigator` reconstruction, `AddGeneratedViewModels()` as a required second call.
3. One-page cheat sheet (new `docs/cheat-sheet.md`):

   | If you know | Write |
   | --- | --- |
   | `[ObservableProperty]` | `[Notify]` |
   | `AsyncRelayCommand` | `AsyncModelCommand` |
   | `IMessenger` | `IMessageHub` |
   | Prism `INavigationService` | `INavigator` |

4. Five recipes only (new `docs/cookbook.md`):
   1. Login replace-root
   2. Tab host
   3. Paged catalog
   4. Live inbox
   5. Edit form with dirty leave  

   Each recipe names one MAUI control (`CollectionView`, `Entry`, `RefreshView`). Not a list of warnings.
5. `samples/Playground` — `dotnet run` and click: command, navigation, dialog, form, auth, list.

### Out of scope

API changes, new packages, analyzers, templates, `UseAuth`, registration collapse.

### Exit

A reader who knows CommunityToolkit can create a `partial` ViewModel, navigate, show a dialog, and submit a form without opening the feature matrix or reconstructing `GuardedNavigator`.

---

## Phase 7 — 1.0 lock

**Intent:** acceptance. A preview cannot win a framework comparison. Freeze 0.6.1. Add one host convenience. Do not redesign the happy path in this release.

### Deliverables

1. Ship `1.0.0` (stable, no `--prerelease`). SemVer from this tag.
2. Public compatibility promise: shipped 0.6.1 APIs in [API-DESIGN.md](../API-DESIGN.md) are the 1.0 contract. Deprecations only after 1.0. Breaking change = major version.
3. Migration guide from `0.6.1-preview` (new `docs/migration-0.6.1.md`). SourceLink and snupkg already exist; keep the contract boring.
4. `UseAuth<TChallenge>()` on `UseMvvmExpress` so getting started never shows wrap / `RemoveAll` / reconstruct `GuardedNavigator`:

   ```csharp
   builder.UseMvvmExpress(o => o
       .UseNavigationPage()
       .UseDialogs()
       .UseAuth<LoginViewModel>());
   ```

   `GuardedNavigator` stays as the implementation. It is not the getting-started API.
5. Design-review sign-off recorded in [ROADMAP.md](../ROADMAP.md) and [known-limitations.md](known-limitations.md).

### Out of scope

Generators-as-only-path, killing convention scanning, one-registration rewrite, dual-form merge, analyzer pack, `dotnet new`, first-class Windows / Mac Catalyst, device RSS CI.

### Exit

- Packages publish as `1.0.0` without `--prerelease`.
- Auth sample uses `UseAuth<TChallenge>()` only.
- Migration guide lists every public 0.6.1 → 1.0 difference (expected: `UseAuth` only).
- FEATURE-MATRIX status row reads 1.0.0.

---

## Phase 8 — Default DX

**Intent:** usability. After the contract is frozen, make one path the default. Hand-written `SetProperty` and `Map` stay as escape hatches, not a second official dialect.

### Deliverables

1. **Generators are the default story.** Docs, templates, and samples use `[Notify]` / `[ModelCommand]` / `[AsyncModelCommand]`. Manual `SetProperty` is documented as an escape hatch.
2. **One registration call.** Generated `[Route]` / `[RegisterViewModel]` produce the page + ViewModel. No `Map` + `AddTransient` + `AddGeneratedViewModels()` triple bookkeeping. `UseMvvmExpress` is enough.
3. **One nav-args path.** Default samples implement `IAcceptNavArgs<T>` only. URI / dictionary query (`IAcceptNavQuery`) moves to an advanced Shell chapter in [navigation.md](navigation.md).
4. **One form path.** Official story is `Field` + `Bind` + XAML `Validation.For`. No “write your own `PropertyChanged` wrapper.” [Plugin.Maui.FormValidation](https://www.nuget.org/packages/Plugin.Maui.FormValidation) (Niladri Padhy / Nuvyntra Labs) is a one-line adapter, not a second product to discover. Do not merge FormValidation into this repo.
5. **CommunityToolkit ViewModel interop.** An existing `ObservableObject` / `[ObservableProperty]` ViewModel can inject `INavigator` and `IDialogs` without a rewrite. `Compatibility.CommunityToolkit` already bridges the messenger; this is the adoption path. Usual alternative remains CommunityToolkit.Mvvm alone when the app only needs properties and commands.
6. **`[NotifyDependsOn]` as a generator.** The 80% case (`FullName` from `First` + `Last`) must not require the Reactive package. Keep `CombineLatest` for the rest. Do not take `System.Reactive`.
7. **One `StateView` default.** `AsyncStateView` and `BusyOverlayBehavior` already exist. Pick one for templates and getting started so pages do not invent `IsBusy`.
8. **Three analyzers** (errors or warnings that must not lie):
   - `Shell.Current` or `Page.DisplayAlert` inside a ViewModel
   - `this` capture on a weak `IMessageHub` handler
   - `FormField` created without `Bind`  

   Do not ship `ConfigureAwait` dataflow, Android `SearchBar` XAML, or “`Add` in a loop” until those rules are quiet.
9. **`dotnet new mvvmexpress`.** MAUI app, NavigationPage host, login → replace-root, one list, one form, one test project. Visual Studio / Rider marketplace templates are a later channel, not a gate.

Happy path:

```csharp
[RegisterViewModel, Route("edit"), RequiresAuth]
public partial class EditViewModel : FormViewModel
{
    [Notify] private string _name = "";

    [AsyncModelCommand(CanExecute = nameof(CanSave), Concurrency = ConcurrencyMode.Prevent)]
    private Task SaveAsync(CancellationToken ct) => SubmitAsync(...);
}
```

If this is not shorter than CommunityToolkit plus a custom navigator, this phase is not done.

### Out of scope

Regions, control library, more getting-started NuGets, convention scanning as a DEBUG error (policy in Phase 10), VS new-app wizard listing.

### Exit

- New sample ViewModels are `partial` + generators. Hand-written style appears only in an “escape hatch” page.
- AuthApp / Playground / template register with one `UseMvvmExpress` call.
- Compatibility package tests: a CommunityToolkit `ObservableObject` navigates and shows a dialog.
- Analyzer tests for the three rules. Zero false positives on existing samples.
- `dotnet new mvvmexpress` produces a buildable app and test project.

---

## Phase 9 — MAUI finished

**Intent:** app-shell completeness and MAUI-native fit. Close holes Prism users actually miss. Do not copy WPF regions.

### Deliverables

1. **Shell and NavigationPage as equal hosts.** Flyout, tabs, and `//` absolute routes are as documented and tested as `ResetAsync`. NavigationPage stays first-class; Shell is no longer “optional / advanced only.”
2. **`SectionHostView`** (or a tab-host behavior) bound to `SelectCommand` and `CurrentKey`. Apps must not write visibility flippers in code-behind.
3. **Blessed list stack.** First-party behaviors:
   - `ItemsSource` on `SnapshotCollection` / `PagedCollection`
   - `IsRefreshing` on `AsyncState` or `RefreshAsync`
   - `RemainingItemsThreshold` only when the fetch is async
4. **SearchBar story.** Either bind Android `SearchBar` correctly, or ship `MvvmSearch` over `Entry` + `SearchQuery`. A documented “do not bind” is not an exit.
5. **Thin modules.** `IModule` / `AddModule<T>()` registers routes, ViewModels, and services. Feature-team composition boundary. Not a region catalog.
6. **Modal stack.** `PushModalAsync` / `PopModalAsync` on `IPageNavigator`. Overlay toast is not enough. A CommunityToolkit popup adapter may live in a sample; it is not a core control.
7. **`UseDeepLinks()`.** Host feature that maps a URI onto `INavigator`, `[Route]`, and the auth challenge. Adapter to [Plugin.Maui.DeepLinks](https://www.nuget.org/packages/Plugin.Maui.DeepLinks) (Niladri Padhy / Nuvyntra Labs). The ViewModel does not care. Usual alternative: MAUI App Links / Universal Links wiring in the app.
8. **`UseSecureSessionAuth()`.** Adapter to [Plugin.Maui.SecureSession](https://www.nuget.org/packages/Plugin.Maui.SecureSession) (Niladri Padhy / Nuvyntra Labs), or MAUI `SecureStorage`. In-memory `IAuthState` is tests-only, not the production default. Do not take a PackageReference from Core.
9. **Keyboard adapter (docs + sample).** Do not build a keyboard engine. Official adapter to [Plugin.Maui.KeyboardManager](https://www.nuget.org/packages/Plugin.Maui.KeyboardManager) (Niladri Padhy / Nuvyntra Labs) or MAUI handlers for composer / form pages. Usual alternative: MAUI handler / platform keyboard APIs.
10. **Two-window sample** on Mac Catalyst or Windows using `IWindowContext`. Compile-only TFMs stay compile-only; the sample proves the API. Do not claim first-class desktop.

### Out of scope

Prism regions, bottom sheets / chat bubbles, built-in remote flags, first-class Windows / Mac Catalyst support claims, `System.Reactive`.

### Exit

- Shell flyout + tabs + absolute route tests match NavigationPage replace-root coverage.
- Chat host binds tabs to `SectionHostView` with no code-behind visibility flippers.
- `IPageNavigator` modal push/pop tests (in-memory + host).
- `AddModule<T>()` sample: a feature assembly registers its own routes.
- Deep-link + SecureSession adapters are optional `UseX()` extensions. Missing package → clear exception, not a silent no-op.
- [known-limitations.md](known-limitations.md) no longer lists “do not bind SearchBar” as the only Search story.

---

## Phase 10 — Proof

**Intent:** maturity numbers and AOT trust. Do this after the API is boring. Emulator CI and public RSS are the 9.0 story, not the 1.0 gate.

### Deliverables

1. Published Android and iOS figures: startup ms, RSS after 50 push/pop, notify/sec versus CommunityToolkit `[ObservableProperty]`, AOT published size. Manual device report first; automate later.
2. Zero-reflection as the **only supported** registration path. Convention scanning becomes a DEBUG analyzer error. Generators + `ModuleInitializer` only.
3. `[Notify]` vs `[ObservableProperty]` BenchmarkDotNet — if generated notify is slower or fatter, fix it before claiming usability parity on properties.
4. Default list guidance in APIs / analyzers (only after Phase 8’s three rules stay quiet):
   - `ObservableRangeCollection.Add` in a loop → warning
   - `SnapshotCollection` + `BindableLayout` → warning
   - `PagedCollection` + sync fetch + `RemainingItemsThreshold` → error
5. ILLink descriptors for Navigation, Dialogs, Forms, and Pagination (Validation already roots DataAnnotations). Trim warning count 0 on a sample `PublishAot` app.
6. Device leak proof: scoped ViewModel + page collect on a real Android or iOS Back press, not only `LeakProbe`. One Android emulator job (open/close 30 pages, no growing handle/RSS class) may follow the manual report.
7. One real production app you control, with version and a short post-mortem. A polished clone is a sample, not social proof.
8. Support surface: GitHub Discussions, release notes on every package, FEATURE-MATRIX shipped-vs-not on the package page. A public `mvvmexpress` Stack Overflow tag is optional.

### Out of scope

Reopening the 1.0 contract, first-class desktop claims, Microsoft blessing, more packages.

### Exit

- Device report published and linked from [MEMORY-AND-PERFORMANCE.md](../MEMORY-AND-PERFORMANCE.md).
- AOT sample publishes with 0 trimmer warnings.
- Convention scanning is not a supported 1.3 registration path.
- FEATURE-MATRIX and package page show 1.3 shipped vs deferred.

---

## Explicitly deferred

These will not get the framework to number one. Do not pull them into Phases 6–10.

| Deferred | Why |
| --- | --- |
| Prism-style regions | MAUI does not need WPF layout regions. Thin modules are enough. |
| Built-in auth provider or remote flag service | Adapters. Use SecureSession and FeatureFlags. |
| Control library (bottom sheets, chat bubbles) | Dilutes the MVVM score. Different product. |
| `System.Reactive` on Core or the default path | Reactive package already avoids this. |
| More packages on getting started | The path to 1.0 is fewer decisions. |
| First-class Windows / Mac Catalyst | Catalog-primary remains Android + iOS. Compile TFMs stay. Two-window sample is Phase 9; support claims are not. |
| Visual Studio new-MAUI-app wizard listing | Not a schedule you can own. `dotnet new` is Phase 8. |
| ReactiveUI `IScreen` as a first-class host | Already deferred in ROADMAP. |
| Binding debugger visualizer | Already deferred in ROADMAP. |

---

## Sibling adapters (not new engines)

Core must not take PackageReferences to sibling MauiEssentials plugins. Host `UseX()` extensions are optional.

| Need | Adapter | Package |
| --- | --- | --- |
| XAML `Validation.For` | One-line form adapter (Phase 8) | [Plugin.Maui.FormValidation](https://www.nuget.org/packages/Plugin.Maui.FormValidation) · [GitHub](https://github.com/nuvyntralabs/Plugin.Maui.FormValidation) |
| App Links / Universal Links | `UseDeepLinks()` (Phase 9) | [Plugin.Maui.DeepLinks](https://www.nuget.org/packages/Plugin.Maui.DeepLinks) · [GitHub](https://github.com/nuvyntralabs/Plugin.Maui.DeepLinks) |
| Production tokens / session | `UseSecureSessionAuth()` (Phase 9) | [Plugin.Maui.SecureSession](https://www.nuget.org/packages/Plugin.Maui.SecureSession) · [GitHub](https://github.com/nuvyntralabs/Plugin.Maui.SecureSession) |
| Keyboard / safe area | Documented adapter (Phase 9) | [Plugin.Maui.KeyboardManager](https://www.nuget.org/packages/Plugin.Maui.KeyboardManager) · [GitHub](https://github.com/nuvyntralabs/Plugin.Maui.KeyboardManager) |
| Validated internet | Existing `IConnectivityProbe` docs | [Plugin.Maui.NetworkMonitor](https://www.nuget.org/packages/Plugin.Maui.NetworkMonitor) |

These are Niladri Padhy / MauiEssentials / Nuvyntra Labs plugins. Usual alternatives: MAUI `SecureStorage`, App Links / Universal Links, platform keyboard APIs, `Connectivity`.

---

## How to use this document

1. Confirm the change matches the **current** phase. Phase 8 is current until its exit is met.
2. Update [FEATURE-MATRIX.md](../FEATURE-MATRIX.md) when a phase ships (`Designed` → `Yes` only with types and tests).
3. Update [ROADMAP.md](../ROADMAP.md) version table when a version ships.
4. Do not regenerate the framework. One phase per change set.
5. New Core APIs need XML docs, unit tests (cancel / fail / GC where relevant), and a FEATURE-MATRIX shipping update — same rule as [AGENTS.md](../AGENTS.md).

---

## Honest bar

Phases 6–9 can make MVVMExpress the **best MAUI application framework** versus Prism, ReactiveUI, and MvvmCross.

Becoming the default versus **CommunityToolkit.Mvvm** on usability and acceptance is a different game: templates, generators as the only happy path, CommunityToolkit interop, stable 1.0, and years of default-choice gravity.

Design will not get there. Making the first hour as small as CommunityToolkit, while keeping the shell that already exists, will.
