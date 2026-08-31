using Microsoft.Extensions.DependencyInjection;
using Plugin.Maui.MVVMExpress.Busy;
using Plugin.Maui.MVVMExpress.Errors;
using Plugin.Maui.MVVMExpress.Messaging;
using Plugin.Maui.MVVMExpress.Samples.Services;

namespace Plugin.Maui.MVVMExpress.Samples.Tests.Support;

internal static class SampleHarness
{
    public static ServiceProvider CreateProvider()
        => new ServiceCollection().AddMvvmExpressSamples().BuildServiceProvider();

    public static (InMemoryProductCatalog Catalog, MessageHub Hub, RecordingErrorSink Errors, BusyGate Busy) Core()
    {
        var catalog = new InMemoryProductCatalog();
        var hub = new MessageHub();
        var errors = new RecordingErrorSink();
        var busy = new BusyGate();
        return (catalog, hub, errors, busy);
    }
}
