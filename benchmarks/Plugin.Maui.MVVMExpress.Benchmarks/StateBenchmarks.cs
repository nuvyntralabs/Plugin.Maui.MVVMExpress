using BenchmarkDotNet.Attributes;
using Plugin.Maui.MVVMExpress.State;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Benchmarks;

[MemoryDiagnoser]
[HideColumns("Error", "StdDev", "Median")]
public class StateBenchmarks
{
    private AsyncState<int> _state = null!;

    [Params(ApplicationScale.Small, ApplicationScale.Mid, ApplicationScale.Large)]
    public ApplicationScale Scale { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _state = new AsyncState<int>();
        _state.PropertyChanged += (_, _) => { };
    }

    [Benchmark]
    public async Task<int> LoadAsync_Success()
    {
        var rounds = ScaleProfile.ViewModelBatch(Scale);
        var last = 0;
        for (var i = 0; i < rounds; i++)
        {
            last = await _state.LoadAsync(_ => Task.FromResult(i));
        }

        return last;
    }
}
