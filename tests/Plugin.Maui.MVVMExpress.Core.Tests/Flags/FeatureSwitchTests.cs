using Plugin.Maui.MVVMExpress.Files;
using Plugin.Maui.MVVMExpress.Flags;
using Plugin.Maui.MVVMExpress.Media;
using Plugin.Maui.MVVMExpress.Permissions;

namespace Plugin.Maui.MVVMExpress.Core.Tests.Flags;

public sealed class FeatureSwitchTests
{
    [Fact]
    public void MemoryFeatureSwitch_DefaultsOff()
    {
        var flags = new MemoryFeatureSwitch();
        Assert.False(flags.IsEnabled("offline-banner"));
        flags.Set("offline-banner", true);
        Assert.True(flags.IsEnabled("offline-banner"));
    }

    [Fact]
    public async Task PermissionGate_RespectsGrant()
    {
        var gate = new MemoryPermissionGate().Set("camera", true);
        Assert.True(await gate.EnsureAsync("camera"));
        Assert.True(await AllowAllPermissionGate.Instance.EnsureAsync("x"));
    }

    [Fact]
    public async Task MemoryFileStore_RoundTrip()
    {
        var store = new MemoryFileStore();
        using var input = new MemoryStream("hello"u8.ToArray());
        await store.WriteAsync("note.txt", input);
        await using var output = await store.OpenReadAsync("note.txt");
        Assert.NotNull(output);
        using var reader = new StreamReader(output!);
        Assert.Equal("hello", await reader.ReadToEndAsync());
        Assert.Null(await store.OpenReadAsync("missing.txt"));
    }

    [Fact]
    public async Task MediaPicker_NullAndMemory()
    {
        Assert.Null(await NullMediaPicker.Instance.PickPhotoAsync());
        var picker = new MemoryMediaPicker { NextPath = "/tmp/shot.jpg" };
        Assert.Equal("/tmp/shot.jpg", await picker.PickPhotoAsync());
    }
}
