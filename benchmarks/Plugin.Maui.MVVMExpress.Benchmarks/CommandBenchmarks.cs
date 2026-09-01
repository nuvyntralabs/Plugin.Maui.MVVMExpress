using BenchmarkDotNet.Attributes;
using Plugin.Maui.MVVMExpress.Input;
using Plugin.Maui.MVVMExpress.Testing;

namespace Plugin.Maui.MVVMExpress.Benchmarks;

[MemoryDiagnoser]
[HideColumns("Error", "StdDev", "Median")]
public class CommandBenchmarks
{
    private ModelCommand _command = null!;
    private int _n;

    [Params(ApplicationScale.Small, ApplicationScale.Mid, ApplicationScale.Large)]
    public ApplicationScale Scale { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _command = new ModelCommand(() => _n++);
        _command.Execute(null);
    }

    [Benchmark]
    public int Execute_Sync()
    {
        var rounds = ScaleProfile.ViewModelBatch(Scale);
        for (var i = 0; i < rounds; i++)
        {
            _command.Execute(null);
        }

        return _n;
    }
}
