# Device report (Phase 10)

Host-process numbers remain the 1.0 claim: BenchmarkDotNet, `ScaleProfile`, and [MEMORY-AND-PERFORMANCE.md](../MEMORY-AND-PERFORMANCE.md).

Hardware figures (startup ms, RSS after 50 push/pop, AOT published size on a physical Android or iOS device) are **manual**. Record them here when a device run is available. Do not invent numbers.

| Metric | Android | iOS | Source |
| --- | --- | --- | --- |
| Startup ms | — | — | Manual |
| RSS after 50 push/pop | — | — | Manual |
| `[Notify]` notify/sec vs `[ObservableProperty]` | host BenchmarkDotNet | host BenchmarkDotNet | `benchmarks/` |
| AOT published size | — | — | Manual `PublishAot` |

In-memory Back-press GC stays covered by `ScopedNavigator` + Core leak tests. A real Back-press collect on a device is still a manual follow-up.
