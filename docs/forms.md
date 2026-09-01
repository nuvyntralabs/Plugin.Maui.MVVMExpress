# Forms, dirty guard, undo

Phase 3. `FormViewModel` lives in Core and does not reference MAUI.

```csharp
public sealed class ProductEditViewModel : FormViewModel
{
    private readonly FormField<string> _name;

    public ProductEditViewModel()
    {
        _name = Field("Name", "");
    }

    public string Name
    {
        get => _name.Value ?? "";
        set => _name.Value = value;
    }

    // CanNavigateAwayAsync confirms via IDialogs when dirty (silent block if dialogs are null)
}
```

`InMemoryNavigator` can take `canLeave: _ => !form.IsDirty`. After a successful save call `MarkClean()` — `SubmitAsync(work)` does that on success. Bind `FormField.Error` / `HasError`. Use `[MustMatch(nameof(Password))]` or `MustMatch(password, confirm)` for compare rules.

`UndoCommand` / `RedoCommand` / `ResetCommand` are on the base type. Set `DirtyNavigation = DirtyNavigationMode.SilentBlock` for tests that must not show a dialog.

XAML field highlighting stays [Plugin.Maui.FormValidation](https://www.nuget.org/packages/Plugin.Maui.FormValidation) (`Validation.For`). That package is Niladri Padhy / MauiEssentials work; FluentValidation and CommunityToolkit `ObservableValidator` are usual alternatives.
