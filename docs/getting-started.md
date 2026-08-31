# Getting started — Core (what ships today)

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

- `ModelCommand` / `ModelCommand<T>` — sync
- `AsyncModelCommand` / `AsyncModelCommand<T>` — async, `IsRunning`, `Cancel`, timeout / retry / `ConcurrencyMode` via `AsyncCommandOptions`

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
builder.UseMvvmExpress();           // MAUI app
```

`INavigator`, `MauiShellNavigator`, `IDialogs`, `ICache`, `IConnectivityProbe`, `IAuthState`, `IValidator`, and `PagedCollection<T>` ship with tests. Source generators remain a later phase. Samples: [samples/README.md](../samples/README.md).
