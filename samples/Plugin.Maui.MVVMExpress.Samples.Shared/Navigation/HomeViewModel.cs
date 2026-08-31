using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Crud;

namespace Plugin.Maui.MVVMExpress.Samples.Navigation;

public sealed class HomeViewModel : PageViewModel
{
    public HomeViewModel(INavigator navigator)
        : base(navigator)
    {
        OpenProductsCommand = new AsyncModelCommand(ct => Navigator!.NavigateToAsync<ProductListViewModel>(ct));
        OpenDetailsCommand = new AsyncModelCommand<int>(
            (id, ct) => Navigator!.NavigateToAsync<ProductDetailsViewModel, ProductDetailsArgs>(
                new ProductDetailsArgs(id), ct));
    }

    public AsyncModelCommand OpenProductsCommand { get; }

    public AsyncModelCommand<int> OpenDetailsCommand { get; }
}
