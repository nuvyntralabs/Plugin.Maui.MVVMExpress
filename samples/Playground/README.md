# MVVMExpress Playground

Fifteen-minute click tour: command, navigation, dialog, form, auth, list. Runs on Android, iOS, Mac Catalyst, and Windows (single-window).

```bash
dotnet run --project samples/Playground/Plugin.Maui.MVVMExpress.Playground.csproj -f net10.0-maccatalyst
```

Host registration:

```csharp
builder.UseMvvmExpress(o => o
    .UseNavigationPage(...)
    .UseDialogs()
    .UseAuth<PlaygroundLoginViewModel>());
```

Demo credentials for **Auth**: `demo@mvvmexpress.dev` / `secret`.
