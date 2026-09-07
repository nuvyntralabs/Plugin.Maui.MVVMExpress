# MVVMExpress IDE wrappers

Thin Visual Studio Code and Visual Studio extensions. They install [`Plugin.Maui.MVVMExpress.Templates`](https://www.nuget.org/packages/Plugin.Maui.MVVMExpress.Templates) and run `dotnet new`. The scaffold stays in [`templates/`](../templates/).

| Host | Commands |
| --- | --- |
| Visual Studio Code | **MVVMExpress: Create New App**, **MVVMExpress: Add Page** |
| Visual Studio 2022+ | **Tools → MVVMExpress → Create New App…**, **Add Page…** |

After the template pack is installed, Visual Studio’s **File → New → Project** lists **MVVMExpress MAUI App** (`ide.host.json` on the project template). **Add → New Item** lists **MVVMExpress Page**.

Requires the .NET SDK on PATH. Extension version is `1.0.2`, same as Plugin.Maui.MVVMExpress.

## Install from Marketplace

Search **MVVMExpress** and install:

| Host | Marketplace |
| --- | --- |
| Visual Studio Code | [MVVMExpress](https://marketplace.visualstudio.com/search?term=MVVMExpress&target=VSCode&category=All%20categories&sortBy=Relevance) |
| Visual Studio 2022+ | [MVVMExpress](https://marketplace.visualstudio.com/search?term=MVVMExpress&target=VS&category=All%20categories&vsVersion=&sortBy=Relevance) |

In the editor: **Extensions** → search **MVVMExpress** → **Install**. Then **MVVMExpress: Create New App** / **Add Page** (VS Code) or **Tools → MVVMExpress** (Visual Studio).

## Install (sideload)

Packed installers (version `1.0.2`) are in [`dist/`](dist/):

| Host | File | Install |
| --- | --- | --- |
| Visual Studio Code | `dist/nuvyntralabs.mvvmexpress-1.0.2.vsix` | `code --install-extension extensions/dist/nuvyntralabs.mvvmexpress-1.0.2.vsix` |
| Visual Studio 2022+ | `dist/nuvyntralabs.MVVMExpress.VisualStudio.1.0.2.vsix` | Double-click the `.vsix`, or **Extensions → Manage Extensions → Install from VSIX…** |

After Visual Studio install, the package loads in the background and installs `Plugin.Maui.MVVMExpress.Templates`, so **File → New → Project** lists **MVVMExpress MAUI App**. **Tools → MVVMExpress** is present after install.

Rebuild both:

```bash
./extensions/pack.sh
```

Do not publish to the Marketplace from a local clone. CI publishes after **Version alignment** succeeds.

## Pipeline

Library CI (`ci.yml`) packs NuGet only. It does not build VSIX files.

The **IDE extensions** workflow (`.github/workflows/ide-extensions.yml`) packs both wrappers. It runs on `main` / tags when `extensions/` changes, or from **Actions → IDE extensions → Run workflow**. It fails unless the extension version fields match [`Directory.Build.props`](../Directory.Build.props). Check locally:

```bash
python3 .github/scripts/check-versions.py --scope extensions
```

That run uploads the `.vsix` files as Actions artifacts. GitHub’s usual workflow success/fail notification links to it. There is no email address in the workflow and no Marketplace PAT.

| Artifact | Listing |
| --- | --- |
| `vscode-MVVMExpress` | Visual Studio Code / Cursor — `nuvyntralabs.mvvmexpress` |
| `vsix-MVVMExpress` | Visual Studio 2022+ — **MVVMExpress for Visual Studio** |

Open the run from the notification → **Artifacts** → download the VSIX → update the matching listing at [Marketplace manage](https://marketplace.visualstudio.com/manage). Do not create a new Visual Studio listing.

Bump `Directory.Build.props` `Version` and the extension version fields together (the alignment job lists every file). Do not run `vsce publish` or `VsixPublisher` from a laptop.

## Visual Studio Code

```bash
./extensions/vscode/pack.sh
```

## Visual Studio

`pack-vsix.sh` compiles the `.vsct` with Wine on macOS or `VSCT.exe` on Windows, then writes the same Marketplace VSIX layout CI uploads.

```bash
./extensions/visualstudio/pack-vsix.sh
```

## CLI (no extension)

```bash
dotnet new install Plugin.Maui.MVVMExpress.Templates
dotnet new mvvmexpress -n MyApp
dotnet new mvvmexpress-page -n Catalog --namespace MyApp
```

See [templates/README.md](../templates/README.md) and [getting started](../docs/getting-started.md).
