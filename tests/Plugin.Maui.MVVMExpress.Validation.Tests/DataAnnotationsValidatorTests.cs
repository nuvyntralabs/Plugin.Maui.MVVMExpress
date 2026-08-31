using System.ComponentModel.DataAnnotations;
using Plugin.Maui.MVVMExpress.Validation;

namespace Plugin.Maui.MVVMExpress.Validation.Tests;

public sealed class DataAnnotationsValidatorTests
{
    [Fact]
    public void Validation_ExposesPackageIdentity()
    {
        Assert.Equal("Plugin.Maui.MVVMExpress.Validation", ValidationMarker.PackageId);
    }

    [Fact]
    public void Validate_Required_FailsWhenEmpty()
    {
        var summary = DataAnnotationsValidator.Instance.Validate(new Named { Name = "" });
        Assert.False(summary.IsValid);
        Assert.Contains(summary.Messages, item => item.PropertyName == nameof(Named.Name));
        Assert.Contains("Name", summary.ToString());
    }

    [Fact]
    public async Task ValidateAsync_SucceedsWhenValid()
    {
        var summary = await DataAnnotationsValidator.Instance.ValidateAsync(new Named { Name = "Ada" });
        Assert.True(summary.IsValid);
        Assert.Equal(string.Empty, summary.ToString());
    }

    private sealed class Named
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; init; } = "";
    }
}
