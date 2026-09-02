# MVVMExpress documentation

Canonical design documents live at the repository root so they match the first development task:

| Document | Purpose |
| --- | --- |
| [ARCHITECTURE.md](../ARCHITECTURE.md) | Packages, layers, dependency graph, conflicts, AOT, leaks, threading |
| [API-DESIGN.md](../API-DESIGN.md) | Public APIs (shipped vs still proposed) |
| [DESIGN.md](../DESIGN.md) | Product design and developer experience |
| [DESIGN-PLAN.md](../DESIGN-PLAN.md) | Phases 0–5 implementation plan (shipped) |
| [development-plan.md](development-plan.md) | Current work: Phases 8–10 after 1.0.0 |
| [ROADMAP.md](../ROADMAP.md) | Versions and exit criteria |
| [FEATURE-MATRIX.md](../FEATURE-MATRIX.md) | Comparison vs CommunityToolkit, Prism, ReactiveUI (shipping vs designed) |
| [MEMORY-AND-PERFORMANCE.md](../MEMORY-AND-PERFORMANCE.md) | Leaks, memory budgets, Small / Mid / Large scale |
| [getting-started.md](getting-started.md) | 15-minute path (ViewModel, navigate, dialog, form) |
| [cheat-sheet.md](cheat-sheet.md) | CommunityToolkit / Prism → MVVMExpress names |
| [cookbook.md](cookbook.md) | Login, tabs, paged catalog, inbox, dirty form |
| [navigation.md](navigation.md) | Shell / page hosts, toast |
| [forms.md](forms.md) | FormViewModel, dirty guard, undo |
| [reactive.md](reactive.md) | IPropertyObservable / CombineLatest |
| [offline.md](offline.md) | FetchPolicy and capability abstractions |
| [TEST-COVERAGE.md](TEST-COVERAGE.md) | Scenario matrix for Core tests |

**1.0.0** is the SemVer lock (`UseAuth<TChallenge>()`). Next work is [development-plan.md](development-plan.md) Phase 8. Migration: [0.6.1 → 1.0](migration-0.6.1.md), [CommunityToolkit](migration-communitytoolkit.md), [Prism](migration-prism.md), [ReactiveUI](migration-reactiveui.md). AOT: [aot.md](aot.md). Limits: [known-limitations.md](known-limitations.md).
