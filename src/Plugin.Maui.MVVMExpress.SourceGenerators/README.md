# Plugin.Maui.MVVMExpress.SourceGenerators

Roslyn generators for `[Notify]`, `[ModelCommand]` / `[AsyncModelCommand]`, `[RegisterViewModel]` / `[Route]`, `[PersistState]`, and `[RequiresAuth]`.

```xml
<PackageReference Include="Plugin.Maui.MVVMExpress.SourceGenerators" Version="1.0.0" PrivateAssets="all" />
```

The consuming project must reference Core (attributes live there). Types must be `partial`. Then call `services.AddGeneratedViewModels()`.

Version `1.0.0` (``). Alternatives: CommunityToolkit.Mvvm generators, handwritten `SetProperty`.
