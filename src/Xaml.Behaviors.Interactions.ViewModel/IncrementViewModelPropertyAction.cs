using System;
using System.Globalization;
using System.Reflection;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.ViewModel;

/// <summary>
/// Increments a numeric view model property when invoked.
/// </summary>
public class IncrementViewModelPropertyAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="PropertyName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> PropertyNameProperty =
        AvaloniaProperty.Register<IncrementViewModelPropertyAction, string?>(nameof(PropertyName));

    /// <summary>
    /// Identifies the <see cref="Delta"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double> DeltaProperty =
        AvaloniaProperty.Register<IncrementViewModelPropertyAction, double>(nameof(Delta), 1);

    /// <summary>
    /// Gets or sets the name of the property to change. This is an avalonia property.
    /// </summary>
    public string? PropertyName
    {
        get => GetValue(PropertyNameProperty);
        set => SetValue(PropertyNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to add. This is an avalonia property.
    /// </summary>
    public double Delta
    {
        get => GetValue(DeltaProperty);
        set => SetValue(DeltaProperty, value);
    }

    /// <inheritdoc />
    public override object? Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var target = AssociatedObject?.DataContext;
        if (target is null)
        {
            return false;
        }

        var propertyName = PropertyName;
        if (string.IsNullOrEmpty(propertyName))
        {
            return false;
        }

        var info = target.GetType().GetRuntimeProperty(propertyName);
        if (info is null || !info.CanWrite)
        {
            return false;
        }

        var current = info.GetValue(target);
        if (current is null)
        {
            return false;
        }

        double value;
        try
        {
            value = Convert.ToDouble(current, CultureInfo.InvariantCulture);
        }
        catch
        {
            return false;
        }

        value += Delta;
        object? result = Convert.ChangeType(value, info.PropertyType, CultureInfo.InvariantCulture);
        info.SetValue(target, result, null);
        return true;
    }
}
