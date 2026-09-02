# Cheat sheet

One page. If you already know CommunityToolkit.Mvvm or Prism, write the MVVMExpress name.

| If you know | Write |
| --- | --- |
| `[ObservableProperty]` | `[Notify]` |
| `AsyncRelayCommand` | `AsyncModelCommand` |
| `IMessenger` | `IMessageHub` |
| Prism `INavigationService` | `INavigator` |

Also useful:

| If you know | Write |
| --- | --- |
| `ObservableObject` | `ViewModel` / `PageViewModel` |
| `[RelayCommand]` | `[ModelCommand]` |
| `IDialogService` / `Page.DisplayAlert` | `IDialogs` |
| Prism `INavigationAware` | `INavigable` + `IAcceptNavArgs<T>` |

```csharp
builder.UseMvvmExpress(o => o
    .UseNavigationPage()
    .UseDialogs()
    .UseAuth<LoginViewModel>());
```

Stay on [CommunityToolkit.Mvvm](https://www.nuget.org/packages/CommunityToolkit.Mvvm) when you only need properties, commands, and a messenger. Take MVVMExpress when you also need a MAUI application shell (navigation, dialogs, lifecycle, forms).

Fifteen-minute path: [getting-started.md](getting-started.md). Recipes: [cookbook.md](cookbook.md).
