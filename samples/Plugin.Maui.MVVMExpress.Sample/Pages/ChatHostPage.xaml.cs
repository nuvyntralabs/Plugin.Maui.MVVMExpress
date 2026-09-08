using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.ChatHost;

namespace Plugin.Maui.MVVMExpress.Sample.Pages;

[RegisterView(typeof(ChatHostViewModel))]
public partial class ChatHostPage : SampleContentPage
{
    public ChatHostPage(ChatHostViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}
