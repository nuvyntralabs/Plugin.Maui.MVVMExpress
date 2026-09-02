# Getting started

A new hire can finish this in about fifteen minutes. Click along in [`samples/Playground`](../samples/Playground/) (Android, iOS, Mac Catalyst, or Windows).

```bash
dotnet add package Plugin.Maui.MVVMExpress
dotnet add package Plugin.Maui.MVVMExpress.Navigation
dotnet add package Plugin.Maui.MVVMExpress.Dialogs
dotnet add package Plugin.Maui.MVVMExpress.SourceGenerators
```

```csharp
builder.UseMvvmExpress(o => o
    .UseNavigationPage((nav, _) => nav
        .Map<HomeViewModel, HomePage>("home")
        .Map<DetailsViewModel, DetailsPage>("details")
        .Map<EditViewModel, EditPage>("edit")
        .Map<LoginViewModel, LoginPage>("login"))
    .UseDialogs()
    .UseAuth<LoginViewModel>());
```

Register an `IAuthState` adapter (Playground uses an in-memory demo; production uses [Plugin.Maui.SecureSession](https://www.nuget.org/packages/Plugin.Maui.SecureSession)). Do not reconstruct `GuardedNavigator`.

---

## Page 1 — ViewModel

A `partial` class, `[Notify]`, and `[AsyncModelCommand]`. Bind the generated `IncrementCommand` to a `Button`.

```csharp
public partial class HomeViewModel : PageViewModel
{
    [Notify] private int _count;

    [AsyncModelCommand]
    private async Task IncrementAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(80, cancellationToken);
        Count++;
    }
}
```

```xml
<Button Text="Increment" Command="{Binding IncrementCommand}" />
```

Do not call `Page.DisplayAlert` or `Shell.Current` from the ViewModel.

---

## Page 2 — Navigation

Inject `INavigator` (already on `PageViewModel`) and push a page.

```csharp
[AsyncModelCommand]
private Task OpenDetailsAsync(CancellationToken cancellationToken)
    => Navigator!.NavigateToAsync<DetailsViewModel>(cancellationToken);
```

After sign-in, replace the root so Back cannot return to login:

```csharp
await Navigator.ResetAsync<HomeViewModel>(cancellationToken);
```

---

## Page 3 — Dialogs

`IDialogs` is on `PageViewModel` when you call `UseDialogs()`.

```csharp
[AsyncModelCommand]
private Task AlertAsync(CancellationToken cancellationToken)
    => Dialogs!.AlertAsync("Saved", "The item is stored.", cancellationToken: cancellationToken);
```

---

## Page 4 — Form

`FormViewModel.Field` + `Bind`. Bind an `Entry` to the public property. Leaving a dirty form confirms “Discard changes?”.

```csharp
public sealed class EditViewModel : FormViewModel
{
    private readonly FormField<string> _name;

    public EditViewModel(INavigator navigator, IDialogs dialogs)
        : base(navigator, dialogs)
    {
        _name = Field("Name", "");
        Bind(_name, nameof(Name), () => SaveCommand.NotifyCanExecuteChanged());
        SaveCommand = new AsyncModelCommand(SaveAsync, () => !string.IsNullOrWhiteSpace(Name));
    }

    public string Name
    {
        get => _name.Value ?? "";
        set => _name.Value = value;
    }

    public AsyncModelCommand SaveCommand { get; }

    private async Task SaveAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(80, cancellationToken);
        MarkClean();
    }
}
```

```xml
<Entry Placeholder="Name" Text="{Binding Name}" />
<Button Text="Save" Command="{Binding SaveCommand}" />
```

`SubmitAsync` calls `MarkClean()` on success.

---

## Next

| Want | Open |
| --- | --- |
| CommunityToolkit / Prism names | [cheat-sheet.md](cheat-sheet.md) |
| Login, tabs, list, inbox, dirty form | [cookbook.md](cookbook.md) |
| Shell vs NavigationPage | [navigation.md](navigation.md) |
| Chat-style host | [chat-host.md](chat-host.md) |

Playground: command, navigation, dialog, form, auth, list. Auth demo: `demo@mvvmexpress.dev` / `secret`.
