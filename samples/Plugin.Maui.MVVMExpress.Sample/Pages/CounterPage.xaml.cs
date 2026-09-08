using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Basic;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

[RegisterView(typeof(CounterViewModel))]
public partial class CounterPage : SampleContentPage
{
    public CounterPage(CounterViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
