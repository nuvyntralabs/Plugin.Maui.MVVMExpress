using System.Windows.Input;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Dialogs.Tests;

public sealed class ButtonCommandGcTests
{
    [Fact]
    public void ButtonBoundToCommand_CanBeCollectedAfterPagePop()
    {
        var command = new ModelCommand(() => { });
        var page = BindButtonThenDrop(command);
        Assert.True(LeakProbe.IsCollected(page), "Button + command + popped page stayed alive through CanExecuteChanged.");
        GC.KeepAlive(command);
    }

    private static WeakReference BindButtonThenDrop(ICommand command)
    {
        var page = new ContentPage();
        var button = new Button { Command = command, BindingContext = page };
        page.Content = button;
        return LeakProbe.Track(page);
    }
}
