# MVVMExpress AuthApp

First-run sample: **sign in → home**, plus register and forgot password.

Demo: `demo@mvvmexpress.dev` / `secret`.

Uses `UseMvvmExpress(o => o.UseShell().UseDialogs().UseAuth<AuthLoginViewModel>())`, `ResetAsync` replace-root, `[RequiresAuth]`, and `FormViewModel` dirty confirm. Do not reconstruct `GuardedNavigator`.

```bash
dotnet build samples/Plugin.Maui.MVVMExpress.AuthApp/Plugin.Maui.MVVMExpress.AuthApp.csproj
```
