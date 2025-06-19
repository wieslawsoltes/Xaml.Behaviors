// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Toggles a boolean view model property when invoked.
/// </summary>
public class ToggleViewModelBooleanAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="PropertyName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> PropertyNameProperty =
        AvaloniaProperty.Register<ToggleViewModelBooleanAction, string?>(nameof(PropertyName));

    /// <summary>
    /// Gets or sets the name of the property to toggle. This is an avalonia property.
    /// </summary>
    public string? PropertyName
    {
        get => GetValue(PropertyNameProperty);
        set => SetValue(PropertyNameProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        if (sender is not Control control)
        {
            return false;
        }

        var target = control.DataContext;
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
        if (info is null || !info.CanWrite || info.PropertyType != typeof(bool))
        {
            return false;
        }

        var current = info.GetValue(target);
        if (current is bool value)
        {
            info.SetValue(target, !value, null);
            return true;
        }

        return false;
    }
}
