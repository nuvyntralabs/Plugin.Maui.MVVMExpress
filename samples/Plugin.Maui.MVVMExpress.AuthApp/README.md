# MVVMExpress AuthApp

First-run sample: **sign in → home**, plus register and forgot password.

Demo: `demo@mvvmexpress.dev` / `secret`.

Uses `UseMvvmExpress(o => o.UseShell().UseDialogs())`, `ResetAsync` replace-root, `GuardedNavigator` + `[RequiresAuth]`, and `FormViewModel` dirty confirm. No `UiBoundCommand` — 0.6 marshals command notifications.

```bash
dotnet build samples/Plugin.Maui.MVVMExpress.AuthApp/Plugin.Maui.MVVMExpress.AuthApp.csproj
```
