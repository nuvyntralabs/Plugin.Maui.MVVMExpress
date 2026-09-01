# Plugin.Maui.MVVMExpress.SourceGenerators

Roslyn generators for `[Notify]`, `[ModelCommand]` / `[AsyncModelCommand]`, `[RegisterViewModel]` / `[Route]`, `[PersistState]`, and `[RequiresAuth]`.

```xml
<PackageReference Include="Plugin.Maui.MVVMExpress.SourceGenerators" Version="0.6.0-preview" PrivateAssets="all" />
```

The consuming project must reference Core (attributes live there). Types must be `partial`. Then call `services.AddGeneratedViewModels()`.

Version `0.6.0-preview` (`--prerelease`). Alternatives: CommunityToolkit.Mvvm generators, handwritten `SetProperty`.
