using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Reactive;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

[RegisterView(typeof(SearchViewModel))]
public partial class SearchPage : SampleContentPage
{
    public SearchPage(SearchViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
