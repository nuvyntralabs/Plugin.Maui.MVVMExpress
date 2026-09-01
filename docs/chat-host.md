# Chat host cookbook

WhatsApp-style apps are one persistent screen, in-place tabs, a filterable inbox, and a thread on a `NavigationPage` stack. That is **not** a Shell + `PagedCollection` + appear/refresh app.

Auth and forms stay on the framework path (`FormViewModel`, `IAuthState`, `GuardedNavigator`). Chat is a **host** sample.

## Register a NavigationPage host

```csharp
builder.UseMvvmExpress(o =>
{
    o.UseNavigationPage((nav, _) => nav
        .Map<LoginViewModel, LoginPage>("login")
        .Map<ChatHostViewModel, ChatHostPage>("chats")
        .Map<ChatThreadViewModel, ChatThreadPage>("thread"));
    o.UseDialogs();
});
```

`UseShell()` is optional. Do not register both unless you really have two hosts.

After sign-in:

```csharp
await Navigator.ResetAsync<ChatHostViewModel>(); // replace-root — also IPageNavigator.ReplaceRootAsync
await Navigator.NavigateToAsync<ChatThreadViewModel, ChatNavArgs>(new(id)); // push on UI thread
```

`MauiPageNavigator` hops to `IMainThread` **before** `new ChatThreadPage()`. Off-thread page construction throws `Page factory must run on the main thread.`

## Tabs are sections, not routes

```csharp
public sealed class ChatHostViewModel : SectionHostViewModel
{
    public ChatHostViewModel()
    {
        Inbox = Add("chats", new ChatInboxViewModel(seed));
        Add("updates", new ChatInboxViewModel([]));
    }

    public ChatInboxViewModel Inbox { get; }
}
```

Bind tab buttons to `SelectCommand` and visibility to `CurrentKey`. Do not `GoToAsync("//chats")` or replace `window.Page` on every tab.

Shared sample: [`samples/Plugin.Maui.MVVMExpress.Samples.Shared/ChatHost`](../samples/Plugin.Maui.MVVMExpress.Samples.Shared/ChatHost/).

## Safe list + search

- Use `SnapshotCollection<T>`: load once in `InitializeAsync`, mutate with `AddLocal` / `Insert`.
- Do not pair `DelegatePagedCollection` with `CollectionView` + `RemainingItemsThreshold` when the fetch is sync or instant.
- Do not call `RefreshAsync` from `OnAppearingAsync`.
- Bind `SearchQuery.Text` to an **Entry**. Watch `CommittedText` to filter. Android `SearchBar` `TextChanged` during layout loops.
- Hub handlers: `CoalescingDispatcher` (marshal + coalesce). Do not `ReplaceRange` a visible `BindableLayout`.

## Composer

`FormViewModel.Bind(_draft, nameof(Draft), () => SendCommand.NotifyCanExecuteChanged())` wires the public property and `CanExecute`. Do not write a manual `PropertyChanged` wrapper.

## Compose sibling plugins

These are Niladri Padhy / MauiEssentials / Nuvyntra Labs packages. Usual alternatives in parentheses.

- Tokens / 401 retry / biometrics: [Plugin.Maui.SecureSession](https://www.nuget.org/packages/Plugin.Maui.SecureSession) (MAUI `SecureStorage`)
- ANR / crash breadcrumbs: [Plugin.Maui.Diagnostics](https://www.nuget.org/packages/Plugin.Maui.Diagnostics) (Firebase Crashlytics / Sentry)
- Keyboard pan / safe area / dismiss: [Plugin.Maui.KeyboardManager](https://www.nuget.org/packages/Plugin.Maui.KeyboardManager) (MAUI `HideSoftInputOnTapped`)

## Android do / don't

See [maui-android.md](maui-android.md).
