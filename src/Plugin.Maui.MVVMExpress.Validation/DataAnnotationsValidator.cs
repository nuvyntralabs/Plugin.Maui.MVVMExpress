using System.ComponentModel.DataAnnotations;
using Plugin.Maui.MVVMExpress.Outcome;

namespace Plugin.Maui.MVVMExpress.Validation;

/// <summary><see cref="IValidator"/> using <see cref="Validator"/>.</summary>
public sealed class DataAnnotationsValidator : IValidator
{
    /// <summary>Shared instance.</summary>
    public static DataAnnotationsValidator Instance { get; } = new();

    /// <inheritdoc />
    public ValidationSummary Validate(object instance)
        => ValidateAsync(instance).GetAwaiter().GetResult();

    /// <inheritdoc />
    public Task<ValidationSummary> ValidateAsync(object instance, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(instance);
        cancellationToken.ThrowIfCancellationRequested();
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(instance, new ValidationContext(instance), results, validateAllProperties: true);
        return Task.FromResult(ToSummary(results));
    }

    /// <inheritdoc />
    public Task<ValidationSummary> ValidatePropertyAsync(object instance, string propertyName, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(instance);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);
        cancellationToken.ThrowIfCancellationRequested();
        var property = instance.GetType().GetProperty(propertyName)
            ?? throw new ArgumentException($"Unknown property '{propertyName}'.", nameof(propertyName));
        var results = new List<ValidationResult>();
        Validator.TryValidateProperty(
            property.GetValue(instance),
            new ValidationContext(instance) { MemberName = propertyName },
            results);
        return Task.FromResult(ToSummary(results));
    }

    private static ValidationSummary ToSummary(List<ValidationResult> results)
    {
        var messages = results
            .Select(item => new ValidationMessage(item.MemberNames.FirstOrDefault() ?? "", item.ErrorMessage ?? ""))
            .ToArray();
        return new ValidationSummary(messages);
    }
}
