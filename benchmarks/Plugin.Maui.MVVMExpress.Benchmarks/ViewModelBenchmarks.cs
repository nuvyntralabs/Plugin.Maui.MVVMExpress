using BenchmarkDotNet.Attributes;
using Plugin.Maui.MVVMExpress.ComponentModel;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Benchmarks;

[MemoryDiagnoser]
[HideColumns("Error", "StdDev", "Median")]
public class ViewModelBenchmarks
{
    [Params(ApplicationScale.Small, ApplicationScale.Mid, ApplicationScale.Large)]
    public ApplicationScale Scale { get; set; }

    [Benchmark]
    public int CreateAndDispose()
    {
        var n = ScaleProfile.ViewModelBatch(Scale);
        for (var i = 0; i < n; i++)
        {
            var vm = new BenchViewModel { Name = i.ToString() };
            vm.Dispose();
        }

        return n;
    }

    private sealed class BenchViewModel : ViewModel
    {
        private string? _name;

        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }
}
