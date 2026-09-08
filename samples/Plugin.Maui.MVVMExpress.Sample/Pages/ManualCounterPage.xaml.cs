using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.EscapeHatch;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

[RegisterView(typeof(ManualCounterViewModel))]
public partial class ManualCounterPage : SampleContentPage
{
    public ManualCounterPage(ManualCounterViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
