using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.ChatHost;
using Plugin.Maui.MVVMExpress.Samples.Crud;
using Plugin.Maui.MVVMExpress.Samples.Modals;

namespace Plugin.Maui.MVVMExpress.Samples.Navigation;

public sealed class HomeViewModel : PageViewModel
{
    public HomeViewModel(INavigator navigator, INotifier? notifier = null, IDeepLinkBridge? deepLinks = null)
        : base(navigator)
    {
        Notifier = notifier;
        DeepLinks = deepLinks;
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
        OpenChatCommand = new AsyncModelCommand(ct => Navigator!.NavigateToAsync<ChatHostViewModel>(ct));
        OpenModalCommand = new AsyncModelCommand(ct => Navigator!.NavigateToAsync<ModalHostViewModel>(ct));
        OpenDeepLinkCommand = new AsyncModelCommand(ct =>
            (DeepLinks ?? new SampleDeepLinkBridge()).NavigateAsync(
                "details?ProductId=2",
                Navigator!,
                ct));
    }

    public INotifier? Notifier { get; }

    public IDeepLinkBridge? DeepLinks { get; }

    public AsyncModelCommand OpenProductsCommand { get; }

    public AsyncModelCommand<int> OpenDetailsCommand { get; }

    public AsyncModelCommand OpenDetailsByRouteCommand { get; }

    public AsyncModelCommand ShowToastCommand { get; }

    public AsyncModelCommand OpenChatCommand { get; }

    public AsyncModelCommand OpenModalCommand { get; }

    public AsyncModelCommand OpenDeepLinkCommand { get; }
}
