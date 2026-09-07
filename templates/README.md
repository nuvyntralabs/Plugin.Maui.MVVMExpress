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

IDE extensions (same `dotnet new` commands): [VS Code Marketplace](https://marketplace.visualstudio.com/search?term=MVVMExpress&target=VSCode&category=All%20categories&sortBy=Relevance) · [Visual Studio Marketplace](https://marketplace.visualstudio.com/search?term=MVVMExpress&target=VS&category=All%20categories&vsVersion=&sortBy=Relevance). Details: [extensions/README.md](../extensions/README.md).

From this repo (no NuGet install of the template pack):

```bash
dotnet new install templates/maui-app
dotnet new install templates/page
dotnet new mvvmexpress -n MyApp -o $TMPDIR/MyApp
```

See [getting started](../docs/getting-started.md).
