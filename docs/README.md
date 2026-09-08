# MVVMExpress documentation

Canonical design documents live at the repository root so they match the first development task:

| Document | Purpose |
| --- | --- |
| [ARCHITECTURE.md](../ARCHITECTURE.md) | Packages, layers, dependency graph, conflicts, AOT, leaks, threading |
| [API-DESIGN.md](../API-DESIGN.md) | Public APIs (shipped vs still proposed) |
| [DESIGN.md](../DESIGN.md) | Product design and developer experience |
| [DESIGN-PLAN.md](../DESIGN-PLAN.md) | Phases 0–5 implementation plan (shipped) |
| [development-plan.md](development-plan.md) | Current work: Phases 8–10 after 1.0.0 |
| [phase-8-10-implementation-plan.md](phase-8-10-implementation-plan.md) | How to implement 8–10 without breaking 1.0: tests, samples, SemVer |
| [ROADMAP.md](../ROADMAP.md) | Versions and exit criteria |
| [FEATURE-MATRIX.md](../FEATURE-MATRIX.md) | Comparison vs CommunityToolkit, Prism, ReactiveUI (shipping vs designed) |
| [MEMORY-AND-PERFORMANCE.md](../MEMORY-AND-PERFORMANCE.md) | Leaks, memory budgets, Small / Mid / Large scale |
| [getting-started.md](getting-started.md) | 15-minute path (ViewModel, navigate, dialog, form) and `dotnet new mvvmexpress` |
| [templates](../templates/README.md) | `dotnet new mvvmexpress` — MainPage, login, list, form, tests |
| [cheat-sheet.md](cheat-sheet.md) | CommunityToolkit / Prism → MVVMExpress names |
| [cookbook.md](cookbook.md) | Login, tabs, paged catalog, inbox, dirty form |
| [navigation.md](navigation.md) | Shell / page hosts, toast |
| [forms.md](forms.md) | FormViewModel, dirty guard, undo |
| [reactive.md](reactive.md) | IPropertyObservable / CombineLatest |
| [offline.md](offline.md) | FetchPolicy and capability abstractions |
| [TEST-COVERAGE.md](TEST-COVERAGE.md) | Scenario matrix for Core tests |
| [device-report.md](device-report.md) | Phase 10 hardware numbers (manual) |
| [production-postmortem.md](production-postmortem.md) | Phase 10 production write-up slot |

**1.3.0** ships Phases 8–10 on the 1.0 SemVer lock (`UseAuth<TChallenge>()` from 1.0.0). Implementation record: [phase-8-10-implementation-plan.md](phase-8-10-implementation-plan.md). Migration: [0.6.1 → 1.0](migration-0.6.1.md), [CommunityToolkit](migration-communitytoolkit.md), [Prism](migration-prism.md), [ReactiveUI](migration-reactiveui.md). AOT: [aot.md](aot.md). Limits: [known-limitations.md](known-limitations.md).
