using Plugin.Maui.MVVMExpress.Samples.Basic;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

public partial class CounterPage : SampleContentPage
{
    public CounterPage(CounterViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
