# Cookbook

Five recipes. Each names one MAUI control. Click the same flows in [`samples/Playground`](../samples/Playground/).

## 1. Login replace-root

**Control:** `Entry`

Sign in, then `ResetAsync` so Back cannot return to login. Host auth with `UseAuth<LoginViewModel>()`. Mark the home ViewModel `[RequiresAuth]`.

```csharp
builder.UseMvvmExpress(o => o
    .UseNavigationPage((nav, _) => nav
        .Map<LoginViewModel, LoginPage>("login")
        .Map<HomeViewModel, HomePage>("home"))
    .UseDialogs()
    .UseAuth<LoginViewModel>());

await _auth.SignInAsync(Email, Password, cancellationToken);
await Navigator.ResetAsync<HomeViewModel>(cancellationToken);
```

```xml
<Entry Placeholder="Email" Text="{Binding Email}" Keyboard="Email" />
<Entry Placeholder="Password" IsPassword="True" Text="{Binding Password}" />
```

Demo credentials in Playground / AuthApp: `demo@mvvmexpress.dev` / `secret`. Production tokens: [Plugin.Maui.SecureSession](https://www.nuget.org/packages/Plugin.Maui.SecureSession) (Niladri Padhy / MauiEssentials). Usual alternative: MAUI `SecureStorage`.

## 2. Tab host

**Control:** `Button`

One page, in-place sections. Bind tab buttons to `SelectCommand` and visibility to `CurrentKey`. Do not `GoToAsync` on every tab.

```csharp
public sealed class ChatHostViewModel : SectionHostViewModel
{
    public ChatHostViewModel()
    {
        Inbox = Add("chats", new InboxViewModel());
        Add("updates", new InboxViewModel());
    }

    public InboxViewModel Inbox { get; }
}
```

```xml
<Button Text="Chats" Command="{Binding SelectCommand}" CommandParameter="chats" />
```

Longer chat host: [chat-host.md](chat-host.md).

## 3. Paged catalog

**Control:** `CollectionView`

`PagedCollection<T>` + `RefreshView` for a catalog that loads more. Do not pair a sync fetch with `RemainingItemsThreshold`.

```csharp
Pages = new DelegatePagedCollection<Product>(catalog.ListPageAsync, pageSize: 20);
RefreshCommand = new AsyncModelCommand(ct => Pages.RefreshAsync(ct));
```

```xml
<RefreshView IsRefreshing="{Binding Pages.State.IsLoading}" Command="{Binding RefreshCommand}">
    <CollectionView ItemsSource="{Binding Items}" />
</RefreshView>
```

## 4. Live inbox

**Control:** `CollectionView`

Load once with `SnapshotCollection<T>`. Mutate with `AddLocal`. Bind search to an `Entry`, not Android `SearchBar`.

```csharp
Chats = new SnapshotCollection<Chat>(ct => store.ListAsync(ct));
// InitializeAsync → Chats.LoadAsync()
// inbound → Chats.AddLocal(row)
```

```xml
<Entry Text="{Binding Search.Text}" Placeholder="Search" />
<CollectionView ItemsSource="{Binding Chats.Items}" />
```

## 5. Edit form with dirty leave

**Control:** `Entry`

`FormViewModel.Field` + `Bind`. Back on a dirty form confirms discard.

```csharp
_name = Field("Name", "");
Bind(_name, nameof(Name), () => SaveCommand.NotifyCanExecuteChanged());
```

```xml
<Entry Placeholder="Name" Text="{Binding Name}" />
<Button Text="Save" Command="{Binding SaveCommand}" />
```

`SubmitAsync` marks the form clean on success. XAML field highlighting stays [Plugin.Maui.FormValidation](https://www.nuget.org/packages/Plugin.Maui.FormValidation) (`Validation.For`). That package is Niladri Padhy / MauiEssentials work; FluentValidation and CommunityToolkit `ObservableValidator` are usual alternatives.
