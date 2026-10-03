// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Applies a binding to a target property when the control is attached to the visual tree.
/// </summary>
public partial class BindingBehavior : AttachedToVisualTreeBehavior<Control>
{

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial AvaloniaProperty? TargetProperty { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial AvaloniaObject? TargetObject { get; set; }

#if UNO
    /// <summary>
    /// Gets or sets the value applied to <see cref="TargetProperty"/>. This is a dependency property.
    /// </summary>
    /// <remarks>
    /// WinUI applies a binding written in XAML to this property (there is no <c>[AssignBinding]</c>), so its value is
    /// the bound value: the behavior sets it to the target property and updates the target when it changes, for example
    /// <c>Binding="{x:Bind SourceBox.Text, Mode=OneWay}"</c>. A <see cref="BindingBase"/> value (assigned in code) is
    /// applied to the target property like on Avalonia.
    /// </remarks>
    [StyledProperty]
    public partial object? Binding { get; set; }
#else
    /// <summary>
    /// 
    /// </summary>
    [StyledProperty(AssignBinding = true)]
    public partial BindingBase? Binding { get; set; }
#endif

    /// <summary>
    /// Applies the binding when the behavior is attached to the visual tree.
    /// </summary>
    /// <returns>A disposable that clears the binding when disposed.</returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
#if UNO
        return ApplyWhileAttached();
#else
        if (TargetObject is not null && TargetProperty is not null && Binding is not null)
        {
            return TargetObject.Bind(TargetProperty, Binding);
        }

        return DisposableAction.Empty;
#endif
    }
}
