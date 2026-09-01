namespace Plugin.Maui.MVVMExpress.ComponentModel;

/// <summary>Marks a field for a generated bindable property on a <c>partial</c> <see cref="ObservableModel"/>.</summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class NotifyAttribute : Attribute;

/// <summary>Also raises <see cref="ObservableModel.Notify"/> for <see cref="PropertyName"/> when the annotated field changes.</summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
public sealed class NotifyAlsoAttribute : Attribute
{
    /// <summary>Creates the attribute.</summary>
    /// <param name="propertyName">Dependent property name.</param>
    public NotifyAlsoAttribute(string propertyName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
        PropertyName = propertyName;
    }

    /// <summary>Dependent property to notify.</summary>
    public string PropertyName { get; }
}
