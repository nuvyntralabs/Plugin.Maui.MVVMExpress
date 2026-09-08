using Plugin.Maui.MVVMExpress.Navigation;
using Plugin.Maui.MVVMExpress.Samples.Modals;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Modals;

public sealed class ModalHostTests
{
    [Fact]
    public async Task OpenModal_PushesNote_ThenClosePops()
    {
        var pages = new InMemoryNavigator();
        var host = new ModalHostViewModel(pages);
        await host.OpenModalCommand.ExecuteAsync();
        Assert.Equal(typeof(NoteModalViewModel), pages.Current);
        Assert.Single(pages.ModalStack);

        var modal = new NoteModalViewModel(pages);
        await modal.CloseCommand.ExecuteAsync();
        Assert.Empty(pages.ModalStack);
    }
}
