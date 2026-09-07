# MVVMExpress templates

Install the packed template, then scaffold an app:

```bash
dotnet new install Plugin.Maui.MVVMExpress.Templates
dotnet new mvvmexpress -n MyApp
dotnet new mvvmexpress-page -n Catalog --namespace MyApp
```

| Short name | Creates |
| --- | --- |
| `mvvmexpress` | MAUI app: MainPage + MainPageViewModel + `IGreetingService`, login, list, form, tests |
| `mvvmexpress-page` | XAML page, ViewModel, service, `Add{Name}()`, `{Binding}` and `Command` |

IDE wrappers (same `dotnet new` commands): [extensions/README.md](../extensions/README.md).

From this repo (no NuGet install of the template pack):

```bash
dotnet new install templates/maui-app
dotnet new install templates/page
dotnet new mvvmexpress -n MyApp -o $TMPDIR/MyApp
```

See [getting started](../docs/getting-started.md).
