// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Sets a view model property when the associated control is loaded.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class SetViewModelPropertyOnLoadBehavior : StyledElementBehavior<Control>
{
    /// <summary>
    /// Identifies the <see cref="PropertyName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> PropertyNameProperty =
        AvaloniaProperty.Register<SetViewModelPropertyOnLoadBehavior, string?>(nameof(PropertyName));

    /// <summary>
    /// Identifies the <see cref="Value"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ValueProperty =
        AvaloniaProperty.Register<SetViewModelPropertyOnLoadBehavior, object?>(nameof(Value));

    /// <summary>
    /// Gets or sets the property name to change. This is an avalonia property.
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
    protected override void OnLoaded()
    {
        base.OnLoaded();

        var target = AssociatedObject?.DataContext;
        if (target is null)
        {
            return;
        }

        var propertyName = PropertyName;
        if (string.IsNullOrEmpty(propertyName))
        {
            return;
        }

        PropertyHelper.UpdatePropertyValue(target, propertyName!, Value);
    }
}
