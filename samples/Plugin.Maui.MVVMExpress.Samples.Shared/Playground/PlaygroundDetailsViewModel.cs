using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Navigation;

namespace Plugin.Maui.MVVMExpress.Samples.Playground;

public sealed record PlaygroundDetailsArgs(string Title);

[RegisterViewModel]
public partial class PlaygroundDetailsViewModel : PageViewModel, IAcceptNavArgs<PlaygroundDetailsArgs>
{
    [Notify]
    private string _title = "";

    /// <inheritdoc />
    public void Accept(PlaygroundDetailsArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);
        Title = args.Title;
    }
}
