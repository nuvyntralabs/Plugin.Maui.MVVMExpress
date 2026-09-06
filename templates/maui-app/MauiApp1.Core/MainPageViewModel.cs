using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Dialogs;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Navigation;

namespace MauiApp1;

[RegisterViewModel]
[Route("main")]
public partial class MainPageViewModel : PageViewModel
{
    private readonly IGreetingService _greetings;

    [Notify] private int _count;
    [Notify] private string _message = "";

    public MainPageViewModel(IGreetingService greetings, INavigator navigator, IDialogs dialogs)
        : base(navigator, dialogs)
    {
        ArgumentNullException.ThrowIfNull(greetings);
        _greetings = greetings;
        Message = _greetings.Greet("MVVMExpress");
    }

    [AsyncModelCommand]
    private async Task IncrementAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(40, cancellationToken).ConfigureAwait(false);
        Count++;
        Message = _greetings.Greet($"click {Count}");
    }

    [AsyncModelCommand]
    private Task OpenListAsync(CancellationToken cancellationToken)
        => Navigator!.NavigateToAsync<ItemListViewModel>(cancellationToken);

    [AsyncModelCommand]
    private Task OpenFormAsync(CancellationToken cancellationToken)
        => Navigator!.NavigateToAsync<ItemEditViewModel>(cancellationToken);

    [AsyncModelCommand]
    private Task OpenLoginAsync(CancellationToken cancellationToken)
        => Navigator!.NavigateToAsync<LoginViewModel>(cancellationToken);
}
