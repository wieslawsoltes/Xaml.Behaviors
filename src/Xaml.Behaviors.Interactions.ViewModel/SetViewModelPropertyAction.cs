using System;
using System.Globalization;
using System.Reflection;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.ViewModel;

/// <summary>
/// Sets a view model property to a specified value when invoked.
/// </summary>
public class SetViewModelPropertyAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="PropertyName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> PropertyNameProperty =
        AvaloniaProperty.Register<SetViewModelPropertyAction, string?>(nameof(PropertyName));

    /// <summary>
    /// Identifies the <see cref="Value"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ValueProperty =
        AvaloniaProperty.Register<SetViewModelPropertyAction, object?>(nameof(Value));

    /// <summary>
    /// Gets or sets the name of the property to change. This is an avalonia property.
    /// </summary>
    public string? PropertyName
    {
        get => GetValue(PropertyNameProperty);
        set => SetValue(PropertyNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to assign. This is an avalonia property.
    /// </summary>
    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
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

        object? result = Value;
        var type = info.PropertyType;
        if (result is not null && !type.IsAssignableFrom(result.GetType()))
        {
            var str = Value?.ToString();
            if (str is not null)
            {
                if (type.GetTypeInfo().IsEnum)
                {
                    result = Enum.Parse(type, str);
                }
                else
                {
                    result = Interactivity.TypeConverterHelper.Convert(str, type);
                }
            }
        }

        info.SetValue(target, result, null);
        return true;
    }
}
