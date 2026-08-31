# Test coverage — Core audit

Scenarios covered by `tests/Plugin.Maui.MVVMExpress.Core.Tests` after the 2026-08-31 recheck.

Latest run (2026-08-31): **103 Core** + **37 sample** + **3 Validation** + **4 Pagination** + **2 Navigation** + **2 Dialogs** + identity facts for Reactive / Generators / Integration. Generators and Reactive remain identity-only until those packages ship APIs.

## Properties

| Scenario | Test |
| --- | --- |
| Change notifies once | `ObservableModelTests.SetProperty_RaisesChanged_OnlyWhenValueDiffers` |
| Same value does not notify | same |
| Same value does not allocate EventArgs | `SameValueSetProperty_DoesNotAllocateEventArgsAfterWarmup` |
| Changing then changed order | `SetProperty_RaisesPropertyChanging_BeforeChanged` |
| Dependent property | `NotifyDependsOn_RaisesDependent` |
| OnChanging / OnChanged callbacks | `SetProperty_Callbacks_RunOnChangeOnly` |
| Null / empty notify name | `Notify_NullOrEmpty_Throws` |
| Custom equality comparer | `SetProperty_CustomComparer_TreatsEqualAsNoChange` |
| EventArgs identity cache | `PropertyEventArgs_AreCachedByName` |

## ViewModel lifecycle

| Scenario | Test |
| --- | --- |
| Initialize / appear / disappear | `ViewModelLifecycleTests` |
| `IViewModel` surface | `ViewModel_ImplementsIViewModel` |
| Loading ⇒ `IsBusy` | `StatusLoading_IsBusy` |
| Refreshing / Saving ⇒ `IsBusy` | `WorkingStatuses_AreBusy` |
| Double dispose + token still readable | `Dispose_IsIdempotent` |
| `DisposeAsync` | `DisposeAsync_CancelsToken` |
| Token cancelled on dispose | `ViewModelGcTests.Dispose_CancelsLifetimeToken` |
| GC after dispose (S/M/L batch) | `ViewModelGcTests` |

## Commands

| Scenario | Test |
| --- | --- |
| CanExecute false | `ModelCommand_DoesNotRun_WhenCanExecuteFalse` |
| CanExecuteChanged | `ModelCommand_Runs_AndRaisesCanExecuteChanged` |
| Typed parameter / wrong type | `ModelCommandOfT_RejectsWrongType` |
| Null ctor (sync / async) | `ModelCommand_NullExecute_Throws`, `AsyncCommand_NullExecute_Throws` |
| Async complete | `AsyncCommand_Completes` |
| Async fail + rethrow | `AsyncCommand_Failure_SetsFailed_AndRethrows` |
| Concurrent prevent | `AsyncCommand_PreventsConcurrentExecution` |
| CanExecute false while running | `AsyncCommand_CanExecute_FalseWhileRunning` |
| External token cancel | `AsyncCommand_ExternalToken_Cancels` |
| `Cancel()` | `CommandGcTests.AsyncCommand_Cancel_StopsWork` |
| Generic async parameter / wrong type | `AsyncCommandOfT_PassesParameter`, `AsyncCommandOfT_RejectsWrongType` |
| Command GC | `CommandGcTests.Command_IsCollectable_WithOwningViewModel` |

## State / outcome / busy / thread

| Scenario | Test |
| --- | --- |
| Load success / empty / cancel / error | `AsyncStateTests` |
| Null loader / null payload | `LoadAsync_NullLoader_Throws`, `LoadAsync_NullPayload_SetsEmpty` |
| Refresh keeps data | `RefreshAsync_KeepsPreviousData_UntilSuccess` |
| Refresh error keeps previous data | `RefreshAsync_Error_KeepsPreviousData` |
| Outcome success / failure / null ErrorInfo | `OutcomeTests` |
| Nested busy + exception restore + double dispose | `BusyGateTests` |
| Immediate dispatcher + cancel | `ImmediateMainThreadTests` |

## Messaging

| Scenario | Test |
| --- | --- |
| Multiple subscribers | `Publish_InvokesAllSubscribers` |
| `PublishAsync` | `PublishAsync_InvokesSubscriber` |
| Dispose subscription (incl. double dispose) | `DisposeSubscription_StopsDelivery`, `Publish_AfterDisposedSubscription_DoesNotInvoke` |
| `Unsubscribe` | `Unsubscribe_StopsAllDeliveryForSubscriber` |
| Null arguments | `Subscribe_NullArguments_Throw` |
| Empty publish | `Publish_NoSubscribers_DoesNotThrow` |
| Cancelled publish | `PublishAsync_Cancelled_Throws` |
| Weak GC / strong pin | `MessageHubGcTests` |

## Collections and scale

| Scenario | Test |
| --- | --- |
| AddRange one Reset (S/M/L) | `AddRange_RaisesSingleReset` + `ScaleAnalysisTests` |
| Add loop per item | `Add_InLoop_RaisesPerItem` |
| Replace / remove / reset / empty / null / ctor | `ObservableRangeCollectionTests` |
| Time + allocation budgets S/M/L | `ScaleAnalysisTests` |
| Host Release timings | `benchmarks/... -- --quick` → [MEMORY-AND-PERFORMANCE.md](../MEMORY-AND-PERFORMANCE.md) §2.1 |

## Sample scenarios (`Plugin.Maui.MVVMExpress.Samples.Tests`)

| Area | Tests |
| --- | --- |
| Basic counter | increment / decrement `CanExecute` / reset / dependent `Label` |
| CRUD | load, empty, error, delete + hub, save, validation `CanExecute`, appear-once |
| Navigation | typed args, missing product empty, dirty guard |
| Auth | bad password, success, `CanExecute`, anonymous block, sign-out |
| Offline | cache-first fallback, no-cache failure |
| Pagination | page `AddRange` Resets, load-more exhaust, refresh |
| Reactive | filter, debounce cancel, `FullName` dependents, dispose cancel |
| Enterprise | online load, offline sink, hub notices, auth gate, DI composition |
| Memory | counter + list VM GC |

## Not covered here (later phases)

Framework Host (`UseMvvmExpress`), Navigation/Dialogs/Validation/Pagination/Reactive **packages**, generators, MAUI behaviors, device RSS, CollectionView virtualization, command timeout/retry, `PageViewModel`. Sample-local adapters cover those *scenarios* until the packages ship.
