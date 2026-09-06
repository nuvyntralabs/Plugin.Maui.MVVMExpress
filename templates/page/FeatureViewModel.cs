using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;

namespace MauiApp1;

[RegisterViewModel]
[Route("feature")]
public partial class FeatureViewModel : PageViewModel
{
    private readonly IFeatureService _service;

    [Notify] private string _title = "Feature";
    [Notify] private int _count;

    public FeatureViewModel(IFeatureService service, INavigator navigator, IDialogs dialogs)
        : base(navigator, dialogs)
    {
        ArgumentNullException.ThrowIfNull(service);
        _service = service;
    }

    public override async Task InitializeAsync(CancellationToken cancellationToken = default)
        => Title = await _service.LoadAsync(cancellationToken).ConfigureAwait(false);

    [AsyncModelCommand]
    private async Task IncrementAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(40, cancellationToken).ConfigureAwait(false);
        Count++;
    }
}
