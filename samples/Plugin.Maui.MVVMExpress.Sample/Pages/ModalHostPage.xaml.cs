using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Modals;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

[RegisterView(typeof(ModalHostViewModel))]
public partial class ModalHostPage : SampleContentPage
{
    public ModalHostPage(ModalHostViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
