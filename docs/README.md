# MVVMExpress documentation

Canonical design documents live at the repository root so they match the first development task:

| Document | Purpose |
| --- | --- |
| [ARCHITECTURE.md](../ARCHITECTURE.md) | Packages, layers, dependency graph, conflicts, AOT, leaks, threading |
| [API-DESIGN.md](../API-DESIGN.md) | Public APIs (shipped vs still proposed) |
| [DESIGN.md](../DESIGN.md) | Product design and developer experience |
| [DESIGN-PLAN.md](../DESIGN-PLAN.md) | Phased work, acceptance, review checklist |
| [ROADMAP.md](../ROADMAP.md) | Versions and exit criteria |
| [FEATURE-MATRIX.md](../FEATURE-MATRIX.md) | Comparison vs CommunityToolkit, Prism, ReactiveUI (shipping vs designed) |
| [MEMORY-AND-PERFORMANCE.md](../MEMORY-AND-PERFORMANCE.md) | Leaks, memory budgets, Small / Mid / Large scale |
| [getting-started.md](getting-started.md) | Core usage that ships today |
| [navigation.md](navigation.md) | Shell / page hosts, toast |
| [forms.md](forms.md) | FormViewModel, dirty guard, undo |
| [reactive.md](reactive.md) | IPropertyObservable / CombineLatest |
| [offline.md](offline.md) | FetchPolicy and capability abstractions |
| [TEST-COVERAGE.md](TEST-COVERAGE.md) | Scenario matrix for Core tests |

Forms / reactive / offline shipped in `0.4.0-preview`. Generators / persist / auth ship in `0.5.0-preview`. Migration: [CommunityToolkit](migration-communitytoolkit.md), [Prism](migration-prism.md), [ReactiveUI](migration-reactiveui.md). AOT: [aot.md](aot.md). Limits: [known-limitations.md](known-limitations.md).
