using System.ComponentModel.DataAnnotations;

namespace Plugin.Maui.MVVMExpress.Samples.Models;

public sealed class Product
{
    public int Id { get; init; }

    [Required(AllowEmptyStrings = false)]
    [MinLength(1)]
    public string Name { get; init; } = "";

    [Range(typeof(decimal), "0.01", "100000")]
    public decimal Price { get; init; }
}
