# AOT and trim

MVVMExpress Core, Testing, Reactive, Validation, Pagination, and the host mark `IsAotCompatible`. Registration must not require a reflection scan.

```csharp
services.AddGeneratedViewModels();
MvvmExpressGeneratedRegistrations.ApplyRoutes((type, route) => navigator.Map(type, route));
var navigator = new GuardedNavigator(inner, auth, MvvmExpressGeneratedRegistrations.AuthPolicy);
```

The MAUI sample (Enterprise flyout included) publishes trimmed:

```bash
dotnet publish samples/Plugin.Maui.MVVMExpress.Sample/Plugin.Maui.MVVMExpress.Sample.csproj \
  -c Release -f net10.0-android -p:PublishTrimmed=true -p:TrimMode=partial
```

Device AOT publish (`PublishAot=true`) is platform-specific and is not run in `dotnet test`. See [known-limitations.md](known-limitations.md).
