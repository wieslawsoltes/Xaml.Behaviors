// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Core;

/// <summary>
/// An action that will change a specified property to a specified value when invoked.
/// </summary>
[RequiresUnreferencedCode("This functionality is not compatible with trimming.")]
public class ChangePropertyAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <seealso cref="PropertyName"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<string?> PropertyNameProperty =
        AvaloniaProperty.Register<ChangePropertyAction, string?>(nameof(PropertyName));

    /// <summary>
    /// Identifies the <seealso cref="TargetObject"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> TargetObjectProperty =
        AvaloniaProperty.Register<ChangePropertyAction, object?>(nameof(TargetObject));

    /// <summary>
    /// Identifies the <seealso cref="Value"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<object?> ValueProperty =
        AvaloniaProperty.Register<ChangePropertyAction, object?>(nameof(Value));

    /// <summary>
    /// Gets or sets the name of the property to change. This is an avalonia property.
    /// </summary>
    public string? PropertyName
    {
        get => GetValue(PropertyNameProperty);
        set => SetValue(PropertyNameProperty, value);
    }

    /// <summary>
    /// Gets or sets the value to set. This is an avalonia property.
    /// </summary>
    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// Gets or sets the object whose property will be changed.
    /// If <seealso cref="TargetObject"/> is not set or cannot be resolved, the sender of <seealso cref="Execute"/> will be used. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public object? TargetObject
    {
        get => GetValue(TargetObjectProperty);
        set => SetValue(TargetObjectProperty, value);
    }

    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior. Generally this is <seealso cref="Avalonia.Xaml.Interactivity.IBehavior.AssociatedObject"/> or a target object.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>True if updating the property value succeeds; else false.</returns>
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var targetObject = GetValue(TargetObjectProperty) is not null ? TargetObject : sender;
        if (targetObject is null)
        {
            return false;
        }

        var propertyName = PropertyName;
        if (propertyName is null)
        {
            return false;
        }

        return PropertyHelper.UpdatePropertyValue(targetObject, propertyName, Value);
    }
}
