# Plugin.Maui.MVVMExpress.Templates

`dotnet new` template for a .NET MAUI app on **Plugin.Maui.MVVMExpress**.

```bash
dotnet new install Plugin.Maui.MVVMExpress.Templates
dotnet new mvvmexpress -n MyApp
cd MyApp
dotnet test MyApp.Tests
```

The generated app starts on **MainPage** / **MainPageViewModel** (`[Notify]` + `IncrementCommand`), with `IGreetingService` registered by `AddMain()`, and `{Binding}` / `Command` in XAML.

Add another screen:

```bash
cd MyApp
dotnet new mvvmexpress-page -n Catalog --namespace MyApp
```

Then map the route and call `services.AddCatalog()` in `MauiProgram`. Move the ViewModel and service into `MyApp.Core` if you keep that split.

Login → replace-root back to MainPage, a list, a form, and `MyApp.Tests` are also included. Supported: Android, iOS, Mac Catalyst, and Windows (single-window).

Demo sign-in: `demo@mvvmexpress.dev` / `secret`. Production tokens: [Plugin.Maui.SecureSession](https://www.nuget.org/packages/Plugin.Maui.SecureSession).

Version `1.3.0`. Product docs: https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress  
IDE extensions: [VS Code Marketplace](https://marketplace.visualstudio.com/search?term=MVVMExpress&target=VSCode&category=All%20categories&sortBy=Relevance) · [Visual Studio Marketplace](https://marketplace.visualstudio.com/search?term=MVVMExpress&target=VS&category=All%20categories&vsVersion=&sortBy=Relevance). Details: [extensions](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress/tree/main/extensions)
