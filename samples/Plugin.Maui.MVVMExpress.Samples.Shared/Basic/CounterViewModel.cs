using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Input;

namespace Plugin.Maui.MVVMExpress.Samples.Basic;

public sealed class CounterViewModel : ViewModel
{
    private int _count;

    public CounterViewModel()
    {
        IncrementCommand = new ModelCommand(() =>
        {
            Count++;
            RefreshCommands();
        });
        DecrementCommand = new ModelCommand(
            () =>
            {
                Count--;
                RefreshCommands();
            },
            () => Count > 0);
        ResetCommand = new ModelCommand(
            () =>
            {
                Count = 0;
                RefreshCommands();
            },
            () => Count != 0);
    }

    private void RefreshCommands()
    {
        DecrementCommand.NotifyCanExecuteChanged();
        ResetCommand.NotifyCanExecuteChanged();
    }

    public int Count
    {
        get => _count;
        private set
        {
            if (SetProperty(ref _count, value))
            {
                NotifyDependsOn(nameof(Count), nameof(Label));
            }
        }
    }

    public string Label => $"Count: {Count}";

    public ModelCommand IncrementCommand { get; }

    public ModelCommand DecrementCommand { get; }

    public ModelCommand ResetCommand { get; }
}
