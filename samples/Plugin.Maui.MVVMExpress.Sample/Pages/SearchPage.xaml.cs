using Plugin.Maui.MVVMExpress.Samples.Reactive;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

public partial class SearchPage : SampleContentPage
{
    public SearchPage(SearchViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
