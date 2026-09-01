using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Crud;

namespace Plugin.Maui.MVVMExpress.Samples.Navigation;

public sealed class HomeViewModel : PageViewModel
{
    public HomeViewModel(INavigator navigator, INotifier? notifier = null)
        : base(navigator)
    {
        Notifier = notifier;
        OpenProductsCommand = new AsyncModelCommand(ct => Navigator!.NavigateToAsync<ProductListViewModel>(ct));
        OpenDetailsCommand = new AsyncModelCommand<int>(
            (id, ct) => Navigator!.NavigateToAsync<ProductDetailsViewModel, ProductDetailsArgs>(
                new ProductDetailsArgs(id), ct));
        OpenDetailsByRouteCommand = new AsyncModelCommand(ct =>
            Navigator!.NavigateToAsync(
                "details",
                new Dictionary<string, object> { ["ProductId"] = 2 },
                cancellationToken: ct));
        ShowToastCommand = new AsyncModelCommand(ct =>
            (Notifier ?? NullDialogs.Instance).ToastAsync("Opened from Navigation sample", cancellationToken: ct));
    }

    public INotifier? Notifier { get; }

    public AsyncModelCommand OpenProductsCommand { get; }

    public AsyncModelCommand<int> OpenDetailsCommand { get; }

    public AsyncModelCommand OpenDetailsByRouteCommand { get; }

    public AsyncModelCommand ShowToastCommand { get; }
}
