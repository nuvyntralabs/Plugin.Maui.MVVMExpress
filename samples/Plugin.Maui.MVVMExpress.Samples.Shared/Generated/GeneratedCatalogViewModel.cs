using Plugin.Maui.MVVMExpress.Auth;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.State;

namespace Plugin.Maui.MVVMExpress.Samples.Generated;

[RegisterViewModel]
[Route("generated")]
[RequiresAuth]
public partial class GeneratedCatalogViewModel : ViewModel
{
    [Notify]
    [NotifyAlso(nameof(Label))]
    private string _query = "";

    [Notify]
    [PersistState]
    private string _draft = "";

    public string Label => $"Q: {Query}";

    [ModelCommand]
    private void Clear() => Query = "";

    [AsyncModelCommand]
    private Task RefreshAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}
