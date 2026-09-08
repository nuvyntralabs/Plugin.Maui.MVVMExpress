using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.Operations;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

[RegisterView(typeof(PipelineViewModel))]
public partial class PipelinePage : SampleContentPage
{
    public PipelinePage(PipelineViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
