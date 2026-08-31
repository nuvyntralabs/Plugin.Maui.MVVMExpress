using BenchmarkDotNet.Attributes;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Benchmarks;

[MemoryDiagnoser]
[HideColumns("Error", "StdDev", "Median")]
public class NotifyBenchmarks
{
    private BenchViewModel _vm = null!;

    [Params(ApplicationScale.Small, ApplicationScale.Mid, ApplicationScale.Large)]
    public ApplicationScale Scale { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _vm = new BenchViewModel();
        _vm.PropertyChanged += (_, _) => { };
        _vm.Name = "warm";
    }

    [Benchmark]
    public void SetProperty_Unchanged()
    {
        var n = ScaleProfile.ViewModelBatch(Scale);
        for (var i = 0; i < n; i++)
        {
            _vm.Name = "warm";
        }
    }

    [Benchmark]
    public void SetProperty_Changed()
    {
        var n = ScaleProfile.ViewModelBatch(Scale);
        for (var i = 0; i < n; i++)
        {
            _vm.Name = i.ToString();
        }
    }

    private sealed class BenchViewModel : ObservableModel
    {
        private string? _name;

        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }
}
