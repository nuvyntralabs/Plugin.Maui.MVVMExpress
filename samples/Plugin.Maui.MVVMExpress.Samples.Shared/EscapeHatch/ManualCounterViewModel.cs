using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Hosting;
using Plugin.Maui.MVVMExpress.Input;

namespace Plugin.Maui.MVVMExpress.Samples.EscapeHatch;

/// <summary>Hand-written <c>SetProperty</c> remains a supported 1.0 escape hatch.</summary>
[RegisterViewModel]
[Route("manual")]
public sealed class ManualCounterViewModel : ViewModel
{
    private int _count;

    /// <summary>Creates the escape-hatch counter.</summary>
    public ManualCounterViewModel()
    {
        IncrementCommand = new ModelCommand(() => Count++);
    }

    /// <summary>Increment.</summary>
    public ModelCommand IncrementCommand { get; }

    /// <summary>Count.</summary>
    public int Count
    {
        get => _count;
        set => SetProperty(ref _count, value);
    }
}
