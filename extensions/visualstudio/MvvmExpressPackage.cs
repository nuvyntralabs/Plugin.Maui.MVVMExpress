using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace NuvyntraLabs.MVVMExpress.VisualStudio;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration("MVVMExpress", "Create MVVMExpress MAUI apps and pages from Plugin.Maui.MVVMExpress.Templates.", "1.0.2")]
[ProvideMenuResource("Menus.ctmenu", 1)]
[ProvideAutoLoad(UIContextGuids80.NoSolution, PackageAutoLoadFlags.BackgroundLoad)]
[ProvideAutoLoad(UIContextGuids80.SolutionExists, PackageAutoLoadFlags.BackgroundLoad)]
[Guid(MvvmExpressPackage.PackageGuidString)]
public sealed class MvvmExpressPackage : AsyncPackage
{
    public const string PackageGuidString = "5f2c8a91-7e4b-4d16-9a3c-1b8e6f0d4c22";

    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        await Commands.InitializeAsync(this).ConfigureAwait(true);
        try
        {
            await DotnetTemplates.EnsureInstalledAsync(cancellationToken).ConfigureAwait(true);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            ActivityLog.LogWarning("MVVMExpress", ex.Message);
        }
    }
}
