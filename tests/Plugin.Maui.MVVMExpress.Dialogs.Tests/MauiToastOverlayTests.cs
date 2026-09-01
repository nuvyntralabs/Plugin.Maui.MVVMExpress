using Plugin.Maui.MVVMExpress.Dialogs;

namespace Plugin.Maui.MVVMExpress.Dialogs.Tests;

public sealed class MauiToastOverlayTests
{
    [Fact]
    public void Show_DoesNotWrapOrReplacePageContent()
    {
        var original = new VerticalStackLayout { AutomationId = "OriginalContent" };
        var page = new ContentPage { Content = original };
        var window = new Window(page);

        using (MauiToastOverlay.Show(page, "saved"))
        {
            Assert.Same(original, page.Content);
            Assert.False(page.Content is Grid { AutomationId: "MvvmExpressToastHost" });
            Assert.Contains(window.Overlays, overlay => overlay is not null);
        }

        Assert.Same(original, page.Content);
    }
}
