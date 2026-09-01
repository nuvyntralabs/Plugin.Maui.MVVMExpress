# Migrate from CommunityToolkit.Mvvm

Stay on CommunityToolkit.Mvvm when you only need properties, commands, and `IMessenger`. Take MVVMExpress when you also need lifecycle, `AsyncState<T>`, or Shell/page navigation.

| CommunityToolkit.Mvvm | MVVMExpress |
| --- | --- |
| `ObservableObject` | `ObservableModel` |
| `[ObservableProperty]` | `[Notify]` (or handwritten `SetProperty`) |
| `[NotifyPropertyChangedFor]` | `[NotifyAlso]` / `NotifyDependsOn` |
| `[RelayCommand]` / `[RelayCommand(AllowConcurrentExecutions)]` | `[ModelCommand]` / `[AsyncModelCommand]` or `ModelCommand` |
| `IMessenger` | `IMessageHub` (or `CommunityToolkitMessageHub` adapter) |
| `ObservableValidator` | `IValidator` / `FormViewModel` |

```csharp
// Before
public partial class HomeViewModel : ObservableObject
{
    [ObservableProperty] private string name = "";
    [RelayCommand] private void Save() { }
}

// After
public partial class HomeViewModel : ViewModel
{
    [Notify] private string _name = "";
    [ModelCommand] private void Save() { }
}
```

Install `Plugin.Maui.MVVMExpress.Core` and `Plugin.Maui.MVVMExpress.SourceGenerators` (`--prerelease`). If you must keep `WeakReferenceMessenger`, add `Plugin.Maui.MVVMExpress.Compatibility.CommunityToolkit` and wrap it with `CommunityToolkitMessageHub`. Do not reference both messengers for the same messages.
