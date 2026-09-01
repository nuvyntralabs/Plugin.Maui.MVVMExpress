# Known limitations (0.5.0-preview)

**Open gate (blocks 1.0.0):** design review sign-off (Phase 0). That is a human decision, not remaining product work.

**Accepted 1.0 scope** (not open work):

- Shipped public APIs in [API-DESIGN.md](../API-DESIGN.md) are the 1.0 contract. Breaking changes after 1.0.0 follow SemVer.
- Device RSS and 50k `CollectionView` scroll: the 1.0 claim is host-process BenchmarkDotNet, `ScaleProfile` (Small / Mid / Large), and virtualization rules in [MEMORY-AND-PERFORMANCE.md](../MEMORY-AND-PERFORMANCE.md). Hardware RSS / on-device scroll is out of catalog scope.
- `ViewModelLifecycleBehavior` attach/detach on a MAUI window: the 1.0 claim is in-memory pop-GC (`ScopedNavigator` + Core leak tests). A device/window detach run is out of catalog scope.
- Windows / Mac Catalyst compile TFMs may exist; they are not catalog-primary.
- Prism regions, ReactiveUI `IScreen`, remote flag/auth providers, bottom-sheet controls, and a VS binding visualizer stay deferred (see [ROADMAP.md](../ROADMAP.md)).
