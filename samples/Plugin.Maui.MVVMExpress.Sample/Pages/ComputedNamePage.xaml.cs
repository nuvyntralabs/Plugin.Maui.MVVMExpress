using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Computed;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

[RegisterView(typeof(ComputedNameViewModel))]
public partial class ComputedNamePage : SampleContentPage
{
    public ComputedNamePage(ComputedNameViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
