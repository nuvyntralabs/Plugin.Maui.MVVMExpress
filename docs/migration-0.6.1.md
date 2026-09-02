# Migrate from 0.6.1-preview to 1.0.0

**1.0.0 is a SemVer lock.** Public APIs shipped in `0.6.1-preview` and listed in [API-DESIGN.md](../API-DESIGN.md) are the 1.0 contract. Deprecations only after 1.0. Breaking change = major version.

SourceLink and snupkg are unchanged. Install without `--prerelease`:

```bash
dotnet add package Plugin.Maui.MVVMExpress
```

## Public differences

| 0.6.1-preview | 1.0.0 |
| --- | --- |
| Reconstruct `GuardedNavigator` after `UseShell` / `UseNavigationPage` (`RemoveAll`, wrap, `ChallengeViewModel`) | `UseAuth<TChallenge>()` on `UseMvvmExpress` |

That is the only public 0.6.1 → 1.0 API addition. Everything else you already compile against stays.

### Before

```csharp
builder.UseMvvmExpress(o => o.UseNavigationPage().UseDialogs());
builder.Services.AddSingleton<INavigator>(sp =>
    new GuardedNavigator(
        sp.GetRequiredService<MauiPageNavigator>(),
        sp.GetRequiredService<IAuthState>(),
        MvvmExpressGeneratedRegistrations.AuthPolicy,
        new GuardedNavigatorOptions { ChallengeViewModel = typeof(LoginViewModel) },
        typeof(HomeViewModel)));
```

### After

```csharp
builder.UseMvvmExpress(o => o
    .UseNavigationPage()
    .UseDialogs()
    .UseAuth<LoginViewModel>());
```

Mark protected ViewModels `[RequiresAuth]`. Register `IAuthState` yourself (in-memory for tests; [Plugin.Maui.SecureSession](https://www.nuget.org/packages/Plugin.Maui.SecureSession) in production). `GuardedNavigator` remains the implementation. It is not the getting-started API.

`AddAuth<TChallenge>()` on `IServiceCollection` is the same wrap for `net10.0` tests that call `AddMvvmExpress()`.

## Unchanged

- `[Notify]`, `[ModelCommand]`, `[AsyncModelCommand]`, `[RegisterViewModel]`, `[Route]`, `[RequiresAuth]`
- `FormViewModel.Field` + `Bind`
- `UseNavigationPage` / `UseShell` / `UseDialogs`
- `IAcceptNavArgs<T>`, `INavigator.ResetAsync`
- Package ids, SourceLink, snupkg

CommunityToolkit ViewModels: [migration-communitytoolkit.md](migration-communitytoolkit.md).
