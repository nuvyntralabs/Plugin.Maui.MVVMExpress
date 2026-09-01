# Test coverage — Core audit

Scenarios covered by `tests/Plugin.Maui.MVVMExpress.Core.Tests` after the 2026-08-31 recheck.

Latest run (2026-09-01): **191 Core** + **60 sample** + **3 generator** + **2 compatibility** + Reactive + Validation (5) + Pagination + Navigation + Dialogs (13, including toast overlay + Button pop-GC). Phase 4 added notify/command/persist/auth generation and `CommunityToolkitMessageHub`. Phase 5 added Testing fakes, lifecycle driver, ScopedNavigator pop-GC, and sample leak / scale tests.

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
| Weak `CanExecuteChanged` + Button-shaped pop | `CommandGcTests.CommandBoundToButtonThenPopPage_DoesNotPinPage`, `WeakCanExecuteChangedTests` |
| Real Button + command + pop page | `ButtonCommandGcTests.ButtonBoundToCommand_CanBeCollectedAfterPagePop` (Dialogs) |
| Toast does not wrap `Page.Content` | `MauiToastOverlayTests.Show_DoesNotWrapOrReplacePageContent` (Dialogs) |

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
| Navigation | typed args, URI query, toast, missing product empty, dirty guard, page-stack push/pop/replace/reset, DI Home URI |
| Auth | bad password, success, `CanExecute`, anonymous block, sign-out |
| Offline | cache-first fallback, no-cache failure |
| Pagination | page `AddRange` Resets, load-more exhaust, refresh |
| Reactive | filter, debounce cancel, `FullName` dependents, dispose cancel |
| Enterprise | online load, offline sink, hub notices, auth gate, DI composition (includes scoped flow) |
| Page scopes | appear-once list, details + back, pop-GC of details |
| Memory | counter + list VM GC, enterprise weak hub, search dispose, FakeMessageHub subscriber |

## Forms / cache / pipeline / Reactive

| Scenario | Test |
| --- | --- |
| Form dirty blocks `CanNavigateAwayAsync` | `FormViewModelTests.Edit_MarksDirty_AndBlocksNavigation` |
| Undo / redo / reset | `FormViewModelTests.UndoRedo_RestoresValues` |
| Sample edit dirty + save | `ProductCrudTests.Edit_Dirty_BlocksNavigation_UntilSaved` |
| Search debounce | `SearchViewModelTests.Debounce_CancelsPrevious` |
| FetchPolicy cache / network / SWR | `CachedFetcherTests` |
| Operation debounce / queue | `OperationExecutorTests` |
| Command queue / debounce / throttle / allow | `CommandPipelineTests` |
| Child attach + scope | `ViewModelComposerTests` |
| CombineLatest | `PropertyObservableTests` |
| FakeMainThread / FakeConnectivity / FakeMessageHub | `TestingFakeTests` |
| `AppearAsync` initializes once | `ViewModelLifecycleDriverTests` |
| `ScopedNavigator` pop GC | `NavigationPopGcTests` |
| Pagination Small / Mid single Reset per page | `PagedProductViewModelTests.LoadMore_Scale_UsesSingleResetPerPage` |

## Out of 1.0 catalog scope

Hardware RSS, on-device `CollectionView` scroll, and MAUI-window `ViewModelLifecycleBehavior` attach/detach GC. Generators have snapshot tests in `Plugin.Maui.MVVMExpress.Generator.Tests`. See [known-limitations.md](known-limitations.md).
