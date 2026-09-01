# Navigation and dialogs

Phase 2 hosts. ViewModels depend on `INavigator` / `IPageNavigator` / `IDialogs` / `INotifier` — never on `Shell` or `Page`.

## Shell

```csharp
var navigator = new MauiShellNavigator()
    .Map<ProductDetailsViewModel>("details");
await navigator.NavigateToAsync<ProductDetailsViewModel, ProductDetailsArgs>(new(2));
await navigator.NavigateToAsync("details", new Dictionary<string, object> { ["ProductId"] = 2 });
```

## Page stack

```csharp
IPageNavigator pages = new MauiPageNavigator(new WindowContext("main"), services)
    .Map<PageStackViewModel, PageStackPage>("stack")
    .Map<PageStackItemViewModel, PageStackItemPage>("stack-item");

await pages.NavigateToAsync("stack-item", new Dictionary<string, object> { ["Title"] = "Latte" });
if (pages.CanGoBack)
    await pages.GoBackAsync();
await pages.PopToRootAsync();
```

`InMemoryNavigator` implements `IPageNavigator` for tests. `WindowNavigatorRegistry` stores one navigator per `IWindowContext`.

## Toast

```csharp
services.AddSingleton<INotifier, MauiNotifier>();
await notifier.ToastAsync("Saved");
```

`MauiToastPresenter` draws on `Window.AddOverlay`. It does not wrap or replace `Page.Content` (so `ResetAsync` cannot restore a stale tree). Tests inject `FakeDialogs` or an `IToastPresenter`.

## Auth: push vs replace-root

Flyout **Auth** sample: login **pushes** `secure`, sign-out `GoBackAsync`.

Apps that must not leak a back-stack use **replace-root**. `ResetAsync<HomeViewModel>()` only works when `//home` is a **root** `ShellContent` (see AuthApp):

```csharp
await Navigator.ResetAsync<AuthHomeViewModel>(); // after sign-in
await Navigator.ResetAsync<AuthLoginViewModel>(); // after sign-out
```

`GuardedNavigatorOptions.ChallengeViewModel` opens login on `E_AUTH` and resumes the original route after `IAuthState.Changed`.
