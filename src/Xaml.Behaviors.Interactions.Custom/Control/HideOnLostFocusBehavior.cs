// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// A behavior that allows to hide control on lost focus event.
/// </summary>
public partial class HideOnLostFocusBehavior : StyledElementBehavior<Control>
{

    /// <summary>
    /// Gets or sets the target control. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial Control? TargetControl { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        AssociatedObject?.AddHandler(InputElement.LostFocusEvent, AssociatedObject_LostFocus,
            RoutingStrategies.Tunnel | RoutingStrategies.Bubble);
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        AssociatedObject?.RemoveRoutedEventHandler(InputElement.LostFocusEvent, AssociatedObject_LostFocus);
    }

    private void AssociatedObject_LostFocus(object? sender, RoutedEventArgs e)
    {
        if (TargetControl is not null)
        {
            TargetControl.IsVisible = false;
        }
    }
}
