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

`UseMvvmExpress` applies generated `[Route]` / `[RequiresAuth]` via a `[ModuleInitializer]`. Do not rely on a DEBUG reflection scan in trimmed Release builds.

### DataAnnotations / trim

`Validator.TryValidateObject` is annotated **IL2026**. `Plugin.Maui.MVVMExpress.Validation` ships `ILLink.Descriptors.xml` as a `TrimmerRootDescriptor` and roots the same types with `[DynamicDependency]` on `DataAnnotationsValidator`.

Preserved in the 0.6 validator path:

| Attribute | Assembly |
| --- | --- |
| `Required` | `System.ComponentModel.Annotations` |
| `StringLength` | `System.ComponentModel.Annotations` |
| `MinLength` | `System.ComponentModel.Annotations` |
| `MaxLength` | `System.ComponentModel.Annotations` |
| `Range` | `System.ComponentModel.Annotations` |
| `RegularExpression` | `System.ComponentModel.Annotations` |
| `EmailAddress` | `System.ComponentModel.Annotations` |
| `Compare` | `System.ComponentModel.Annotations` |
| `MustMatch` | `Plugin.Maui.MVVMExpress.Validation` |

Also rooted: `ValidationAttribute`, `Validator`, `ValidationContext`, `ValidationResult`, `DataAnnotationsValidator`.

Prefer `[MustMatch]` over handwritten password-confirm. If you add a custom `ValidationAttribute`, ship an app-level trimmer descriptor (or `[DynamicDependency]`) for that type — the package descriptor does not see it.
