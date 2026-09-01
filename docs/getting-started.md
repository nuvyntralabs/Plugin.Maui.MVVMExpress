# Getting started

**First run:** the login → home sample at [`samples/Plugin.Maui.MVVMExpress.AuthApp`](../samples/Plugin.Maui.MVVMExpress.AuthApp/). Demo credentials: `demo@mvvmexpress.dev` / `secret`.

The flyout catalog stays at [`samples/Plugin.Maui.MVVMExpress.Sample`](../samples/Plugin.Maui.MVVMExpress.Sample/).

Phase extras: [forms](forms.md), [reactive](reactive.md), [cache policies](offline.md), [navigation](navigation.md).

## Host footguns

1. Call `InitializeComponent()` on `App` **before** resolving `AppShell` / pages (`App(IServiceProvider)` + `CreateWindow`).
2. Bind `Button.Command` to `AsyncModelCommand` only on **0.6.0-preview+** (UI-thread marshal + weak `CanExecuteChanged`). On 0.5.0-preview, wrap or hop yourself.
3. Dirty forms confirm “Discard changes?” when `IDialogs` is registered. Tests keep a silent block when dialogs are null.

```bash
dotnet add package Plugin.Maui.MVVMExpress.Core --prerelease
dotnet add package Plugin.Maui.MVVMExpress --prerelease
```

Or project-reference this repository while iterating.

## ViewModel

```csharp
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.State;

public sealed class HomeViewModel : ViewModel
{
    public AsyncState<IReadOnlyList<int>> Items { get; } = new();

    public AsyncModelCommand RefreshCommand { get; }

    public HomeViewModel()
    {
        RefreshCommand = new AsyncModelCommand(
            ct => Items.LoadAsync(_ => Task.FromResult<IReadOnlyList<int>>([1, 2, 3]), ct));
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            RefreshCommand.Cancel();
        }

        base.Dispose(disposing);
    }
}
```

`Dispose` cancels `ViewModelCancellationToken`. The token remains readable after dispose (`IsCancellationRequested` is true). Do not call `Page.DisplayAlert` or `Shell.Current` from the ViewModel.

## Commands

- `ModelCommand` / `ModelCommand<T>` — sync; weak `CanExecuteChanged`
- `AsyncModelCommand` / `AsyncModelCommand<T>` — async, `IsRunning`, `Cancel`, timeout / retry / `ConcurrencyMode` via `AsyncCommandOptions`; weak `CanExecuteChanged` (a popped Button does not stay pinned)

## Collections (mid / large lists)

```csharp
var items = new ObservableRangeCollection<Product>();
items.AddRange(page);      // one CollectionChanged Reset
items.ReplaceRange(next);
```

Do not `Add` in a loop for mid or large lists.

## Messaging

```csharp
hub.Subscribe<HomeViewModel, CartChanged>(this, static (vm, msg) => vm.Refresh(), weak: true);
```

The handler must use the recipient argument so a weak subscribe does not pin the ViewModel.

## Host registration

```csharp
services.AddMvvmExpress();          // tests / net10.0
builder.UseMvvmExpress(o => o.UseShell().UseDialogs()); // MAUI app
```

`INavigator`, `MauiShellNavigator`, `MauiPageNavigator`, `IDialogs`, `INotifier` / `MauiNotifier`, `ICache` / `ICachedFetcher`, `IConnectivityProbe`, `IAuthState`, `IValidator`, `FormViewModel`, `IOperationExecutor`, and `PagedCollection<T>` ship with tests. Optional `[Notify]` / `[ModelCommand]` / `[RegisterViewModel]` generation: `Plugin.Maui.MVVMExpress.SourceGenerators` plus `services.AddGeneratedViewModels()`. Navigation hosts: [navigation.md](navigation.md). Samples: [samples/README.md](../samples/README.md). Known limits: [known-limitations.md](known-limitations.md).
