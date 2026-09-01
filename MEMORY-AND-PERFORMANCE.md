# Memory, leaks, and performance

How **Plugin.Maui.MVVMExpress** is designed to stay safe and usable in **small**, **mid**, and **large** MAUI applications.

This is an engineering contract. Numbers that are not yet measured on a device are marked **budget** (target), not **benchmark** (measured). Core leak and allocation tests in this repository enforce the parts that do not need a device.

Related: [ARCHITECTURE.md](ARCHITECTURE.md) §15–16, [ROADMAP.md](ROADMAP.md) Phase 1 / 5.

## 1. Application scale profiles

| Profile | Typical app | Live pages / stack | Bound list size | Concurrent async ops | Framework overhead budget (not app data) |
| --- | --- | --- | --- | --- | --- |
| **Small** | Counter, settings, 5–15 screens | ≤ 8 | ≤ 200 | 1–2 | ≤ 2 MB extra RSS |
| **Mid** | CRUD / retail / field app | ≤ 20 | ≤ 5_000 (paged or virtualized) | 4–8 | ≤ 8 MB extra RSS |
| **Large** | Enterprise (catalog, POS, inspection, multi-window) | ≤ 40 on stack; 200+ routes | 50_000–100_000 **virtualized**; never 100k fully realized cells | ≤ 16 | ≤ 20 MB extra RSS |

**App data is not framework overhead.** A 50_000-row product list occupies whatever the `Product` objects occupy. The framework is responsible for:

- How many `PropertyChanged` allocations fire
- How many `CollectionChanged` events fire
- Whether popped ViewModels are collectable
- Whether messengers and commands pin graphs
- Whether scopes dispose

`ScaleProfile` in `Plugin.Maui.MVVMExpress.Testing` encodes the list sizes used by tests and benchmarks: Small = 200, Mid = 5_000, Large = 50_000.

## 2. Current check (this revision)

| Area | Result |
| --- | --- |
| Runtime (0.4.0-preview) | Real subscriptions exist (MessageHub, lifecycle behavior, navigators). Leak tests cover VM / command / weak hub. |
| `ObservableModel` | Event args **cached by property name**. Same-value `SetProperty` does not notify |
| `ViewModel` | `Dispose` cancels `ViewModelCancellationToken`. The token stays readable after dispose (cached at construction). WeakReference tests require collectability |
| `MessageHub` | Default subscribe is **weak** and uses `(recipient, message)` handlers so the delegate does not capture the subscriber. `Dispose` / `Unsubscribe` remove the slot |
| `ObservableRangeCollection<T>` | `AddRange` / `ReplaceRange` raise **one** `Reset`, not N `Add` events |
| `AsyncModelCommand` | Single-flight (`Interlocked` lock). Concurrent `ExecuteAsync` is a no-op. `Cancel` + ViewModel dispose cancel in-flight work. Generic command releases the lock if linked CTS construction fails |
| Device RSS / UI virtualization | **Not measured yet** (needs Android/iOS sample). Budgets above apply when samples exist |

### 2.1 Measured on host (not a device)

Recorded 2026-08-31 with `dotnet run --project benchmarks/Plugin.Maui.MVVMExpress.Benchmarks -c Release -- --quick` after a warmup pass. Runtime: .NET 10.0.2 on macOS (Unix 26.5.2). These are **host process** figures. They are not Android/iOS RSS.

| Operation | Measured | vs target (§5) |
| --- | --- | --- |
| `SetProperty` unchanged × 10_000 | **0.048 ms** (~4.8 ns/call), **40 B** allocated | Target under 50 ns; allocation budget held |
| `SetProperty` change × 10_000 | **0.189 ms** (~19 ns/call), **40 B** allocated | Target under 200 ns; EventArgs cache holds |
| `AddRange` Small n=200 | **0.011 ms**, **1** `CollectionChanged`, 2.4 KB | One Reset. Bytes are list backing store, not N events |
| `AddRange` Mid n=5_000 | **0.055 ms**, **1** event, 66 KB | Same |
| `AddRange` Large n=50_000 | **0.190 ms**, **1** event, 525 KB | Same; 525 KB ≈ `int` list capacity, not 50k EventArgs |
| ViewModel create+dispose × 32 | **0.003 ms**, 3.6 KB (~113 B/VM) | Target &lt; 5 µs/VM |
| ViewModel create+dispose × 256 | **0.014 ms**, 29 KB (~112 B/VM) | Same |
| `ModelCommand.Execute` × 10_000 | **0.050 ms** (~5 ns/call) | Target &lt; 1 µs |

40 B leftover on the notify loops is measurement noise (thread alloc bucket), not a new `PropertyChangedEventArgs` per raise. Tests assert the same-value path stays under 1–2 KB for 10_000 sets.

CI also enforces time/notify budgets in `ScaleAnalysisTests` (Debug). Debug is slower; those tests use 50 / 250 / 2000 ms ceilings, not the Release numbers above.

```bash
dotnet test tests/Plugin.Maui.MVVMExpress.Core.Tests
dotnet run --project benchmarks/Plugin.Maui.MVVMExpress.Benchmarks -c Release -- --quick
dotnet run --project benchmarks/Plugin.Maui.MVVMExpress.Benchmarks -c Release
```

## 3. Memory leak catalog

| Risk | Why it happens in MAUI MVVM | MVVMExpress rule | Test |
| --- | --- | --- | --- |
| Page → Behavior → VM → Page | Behavior holds page; VM holds page for `DisplayAlert` | VM never references `Page`. Lifecycle is a behavior/host hook that **unsubscribes on Unloaded** | Host Phase 1; WeakReference after pop |
| Strong messenger | Hub stores `Action` that captured `this` | Weak register + `Action<TRecipient, TMessage>` (recipient passed in). Strong is explicit | `MessageHubGcTests` |
| `ICommand.CanExecuteChanged` | Static command or long-lived service holds command | Commands are instance fields of the VM; die with the VM | `CommandGcTests` |
| Navigation stack | Popped VM stays in a list | Hosts track `Stack` / pop; page-scope dispose on pop is still the contract | Navigation tests exist; pop-GC device case remains Phase 5 |
| Child ViewModels | Parent list never cleared | Parent `Dispose` walks children (Phase 3) | Composition GC test |
| Reactive subscriptions | `Subscribe` without dispose | `ViewModel` trash bag cleared on dispose (Phase 3) | Reactive GC test |
| Static `Application.Current` | Host event never unhooked | Host registers `IDisposable` on app lifetime (Phase 1 host) | Integration |
| CollectionView cell bindings | 100k realized views | Pagination + virtualization; `AddRange` one notify | Scale collection tests |
| `PropertyChangedEventArgs` | New args per notify | Cache by property name | Allocation test |
| Async continuation | `async` lambda captures VM after disappear | Cancel token on disappear/dispose | `ViewModelGcTests` + cancel test |

**Required GC protocol** (used by `LeakProbe`):

```text
drop strong refs
GC.Collect(MaxGeneration, Forced, blocking: true, compacting: true)
GC.WaitForPendingFinalizers()
repeat
assert WeakReference.IsAlive == false
```

Debug JIT and XAML inspector can keep extras alive. Leak tests run in **Release** on CI when possible; Debug may be flaky. Tests use `LeakProbe.IsCollected` with multiple rounds.

## 4. Memory utilization (what we control)

### 4.1 Per notify

| Technique | Small | Mid | Large |
| --- | --- | --- | --- |
| Cache `PropertyChangedEventArgs` | Yes | Yes | **Required** |
| Equality check before notify | Yes | Yes | **Required** |
| Dependent properties: notify only named set | Yes | Yes | Yes |
| Do not raise `PropertyChanged(null)` except `RefreshAll` | Default | Default | Default |

### 4.2 Per collection

| Technique | Small (≤200) | Mid (≤5k) | Large (50k+) |
| --- | --- | --- | --- |
| `Add` in a loop | OK | Avoid | **Forbidden** in library/sample code |
| `AddRange` → one `Reset` | OK | **Required** | **Required** |
| Bind full list to `CollectionView` | OK | Prefer paging | **Must virtualize** + page/cursor |
| Replace list by new `ObservableCollection` | OK | Allocates | Allocates; prefer `ReplaceRange` |

Large apps must use `PagedCollection<T>` (Phase 2) or `CollectionView` virtualization. The framework will not pretend 100k realized cells are fine.

### 4.3 Per ViewModel

| Object | Budget mindset |
| --- | --- |
| `ViewModel` + 1 CTS | Small, one per page |
| Commands | One instance per command, created in ctor, not per execute |
| `AsyncState<T>` | One per async surface; do not allocate a new state object per load |
| Page `IServiceScope` | Disposed on pop — services in that scope become collectable |

### 4.4 Messaging

| Mode | Use | Pin risk |
| --- | --- | --- |
| Weak (default) | Cross-VM events | Low if handler does not capture subscriber |
| Strong | Must survive until `Unsubscribe` / `Dispose` | High — caller owns lifetime |

## 5. Performance by scale

| Operation | Small target | Mid target | Large target | How |
| --- | --- | --- | --- | --- |
| `SetProperty` no-change | < 50 ns (measured ~5 ns host) | same | same | Equality exit, no event |
| `SetProperty` change (cached args) | < 200 ns + handler (measured ~19 ns host) | same | same | No EventArgs alloc |
| `AddRange` N items, 1 notify | N=200 cheap | N=5k one Reset | N=50k one Reset; do not measure UI bind here | `ObservableRangeCollection` |
| Command execute (sync) | < 1 µs overhead | same | same | No reflection |
| ViewModel create+dispose | < 5 µs + DI | batch 10k in bench | same | No static tables |
| Navigation | one serialized op / window | same | never parallel push | `INavigator` |
| Search | debounce 300 ms | same | cancel previous | `SearchQuery` |

UI frame budget remains **16 ms**. Library code on the UI thread must not walk 50k items except inside a single `AddRange` that the view virtualizes.

`ConfigureAwait(false)` in Core pipeline code. Marshal to `IMainThread` only for `PropertyChanged` if the caller is off-thread (opt-in / dispatcher present).

## 6. What the app must still do

The framework cannot fix:

- Binding a non-virtualized `StackLayout` to 50k items
- Storing bitmaps on each row ViewModel
- Singleton ViewModels that accumulate lists for the whole session
- Strong `MessageHub` subscriptions never unsubscribed
- Calling `.Result` on the UI thread

Samples show Small (Basic), Mid (CRUD + pagination), Large (Enterprise). Device RSS and 50k CollectionView scroll remain Phase 5. Use `ScaleProfile` in tests.

## 7. Telemetry hooks (optional)

`IMvvmExpressTelemetry` (Phase 1+) may record:

- Command duration
- ViewModel create / dispose counts
- Collection reset size
- Failed GC assertion in debug only

Never enabled as a default Release cost. Diagnostics off unless `EnableDiagnostics` is set.

## 8. Definition of done for scale

| Gate | Small | Mid | Large |
| --- | --- | --- | --- |
| Leak tests (VM, command, weak hub) | Required now | Required now | Required now |
| `AddRange` single notification | Required now | Required now | Required now |
| Allocation bound on repeated same-value `SetProperty` | Required now | Required now | Required now |
| BenchmarkDotNet S/M/L params | Harness now | Harness now | Harness now |
| Device RSS vs budget | Phase 5 sample | Phase 5 | Phase 5 Enterprise |
| Navigation pop GC | Tests exist (in-memory / host) | same | Device RSS still Phase 5 |
| 50k CollectionView scroll | — | Phase 5 Pagination sample | Phase 5 Enterprise |
