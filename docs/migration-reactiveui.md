# Migrate from ReactiveUI

ReactiveUI requires System.Reactive for `WhenAnyValue` / `ReactiveCommand`. MVVMExpress derived state is optional and does not take Rx in Core.

| ReactiveUI | MVVMExpress |
| --- | --- |
| `ReactiveObject` | `ObservableModel` / `ViewModel` |
| `[Reactive]` | `[Notify]` |
| `ReactiveCommand` | `AsyncModelCommand` + `AsyncCommandOptions` |
| `WhenAnyValue` / OAPH | `PropertyObservable.CombineLatest` |
| `WhenActivated` | `InitializeAsync` / `OnAppearingAsync` / dispose |
| `IScreen` / `RoutingState` | `INavigator` (Shell or page). `IScreen` is **not** a first-class host |

Use ReactiveUI when the app is already an Rx pipeline. Use MVVMExpress when you want bindable `AsyncState<T>`, typed navigation, and optional CombineLatest without System.Reactive.
