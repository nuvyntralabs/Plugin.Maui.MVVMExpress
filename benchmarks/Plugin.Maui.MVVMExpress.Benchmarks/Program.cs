using BenchmarkDotNet.Running;
using Plugin.Maui.MVVMExpress.Benchmarks;

if (args is ["--quick"])
{
    QuickMeasure.Run();
    return 0;
}

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
return 0;
