// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Applies a binding to a target property when the control is attached to the visual tree.
/// </summary>
public class BindingBehavior : AttachedToVisualTreeBehavior<Control>
{
    /// <summary>
    /// Gets or sets the property that will be bound.
    /// </summary>
    public static readonly StyledProperty<AvaloniaProperty?> TargetPropertyProperty =
        AvaloniaProperty.Register<BindingBehavior, AvaloniaProperty?>(nameof(TargetProperty));

    /// <summary>
    /// Gets or sets the object that owns the <see cref="TargetProperty"/>.
    /// </summary>
    public static readonly StyledProperty<AvaloniaObject?> TargetObjectProperty =
        AvaloniaProperty.Register<BindingBehavior, AvaloniaObject?>(nameof(TargetObject));

    /// <summary>
    /// Gets or sets the binding to apply.
    /// </summary>
    public static readonly StyledProperty<IBinding?> BindingProperty =
        AvaloniaProperty.Register<BindingBehavior, IBinding?>(nameof(Binding));

    /// <summary>
    /// 
    /// </summary>
    public AvaloniaProperty? TargetProperty
    {
        get => GetValue(TargetPropertyProperty);
        set => SetValue(TargetPropertyProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    [ResolveByName]
    public AvaloniaObject? TargetObject
    {
        get => GetValue(TargetObjectProperty);
        set => SetValue(TargetObjectProperty, value);
    }

    /// <summary>
    /// 
    /// </summary>
    [AssignBinding]
    public IBinding? Binding
    {
        get => GetValue(BindingProperty);
        set => SetValue(BindingProperty, value);
    }

    /// <summary>
    /// Applies the binding when the behavior is attached to the visual tree.
    /// </summary>
    /// <returns>A disposable that clears the binding when disposed.</returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        if (TargetObject is not null && TargetProperty is not null && Binding is not null)
        {
            return TargetObject.Bind(TargetProperty, Binding);
        }

        return DisposableAction.Empty;
    }
}
