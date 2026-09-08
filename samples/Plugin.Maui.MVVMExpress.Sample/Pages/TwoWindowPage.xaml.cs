using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Windows;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

[RegisterView(typeof(TwoWindowDemoViewModel))]
public partial class TwoWindowPage : SampleContentPage
{
    public TwoWindowPage(TwoWindowDemoViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
