# Migrate from Prism.Maui

Prism.Maui is a page-navigation + dialog + container stack. It does not host .NET MAUI Shell. MVVMExpress hosts **Shell or** `INavigation`.

| Prism.Maui | MVVMExpress |
| --- | --- |
| `BindableBase` | `ObservableModel` / `ViewModel` |
| `DelegateCommand` | `ModelCommand` / `AsyncModelCommand` |
| `INavigationService` + URI / dictionary | `INavigator.NavigateToAsync<TViewModel, TArgs>(args)` |
| `INavigationParameters` | typed `record` + `IAcceptNavArgs<T>` (URI query remains for deep links) |
| `IConfirmNavigation` | `CanNavigateAwayAsync` / `FormViewModel` dirty |
| `IDialogService` | `IDialogs` / `INotifier` |
| DryIoc / Unity | `Microsoft.Extensions.DependencyInjection` via `AddMvvmExpress` / `UseMvvmExpress` |
| Regions | **Not in v1** (deferred) |

Register pages with `[RegisterViewModel]` / `[Route]` and `MvvmExpressGeneratedRegistrations.AddGeneratedViewModels()` instead of a reflection scan. Keep Prism only if you need regions.
