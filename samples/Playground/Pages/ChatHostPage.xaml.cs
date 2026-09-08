using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Samples.ChatHost;

namespace Plugin.Maui.MVVMExpress.Playground.Pages;

[RegisterView(typeof(ChatHostViewModel))]
public partial class ChatHostPage : ContentPage
{
    public ChatHostPage(ChatHostViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
