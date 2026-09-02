# Plugin.Maui.MVVMExpress.Validation

DataAnnotations validation for **MVVMExpress** (`IValidator`).

[![NuGet](https://img.shields.io/nuget/v/Plugin.Maui.MVVMExpress.Validation.svg?label=NuGet)](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Validation)

```csharp
public sealed class Product
{
  [Required]
  public string Name { get; set; } = "";
}

var summary = DataAnnotationsValidator.Instance.Validate(product);
if (!summary.IsValid)
  await dialogs.AlertAsync("Invalid", summary.ToString());
```

FluentValidation stays an optional app-level adapter. Trim: the package ships `ILLink.Descriptors.xml` for `Required`, `MinLength`, `MaxLength`, `StringLength`, `Range`, `RegularExpression`, `EmailAddress`, `Compare`, and `MustMatch`. Custom attributes need an app-level descriptor.

## Install

```bash
dotnet add package Plugin.Maui.MVVMExpress.Validation
```

Target framework: `net10.0`. Depends on [Core](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Core). Version `1.0.0`.

## Related

For XAML `Validation.For` on Entries, use [Plugin.Maui.FormValidation](https://www.nuget.org/packages/Plugin.Maui.FormValidation) (Niladri Padhy / MauiEssentials). Alternatives: FluentValidation, CommunityToolkit.Mvvm `ObservableValidator`.

Product docs: [repository README](https://github.com/nuvyntralabs/Plugin.Maui.MVVMExpress). License: MIT.
