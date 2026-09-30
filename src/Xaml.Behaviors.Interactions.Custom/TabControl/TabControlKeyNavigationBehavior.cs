// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
using TabControl = Microsoft.UI.Xaml.Controls.TabView;
using Key = Windows.System.VirtualKey;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Enables keyboard navigation for a <see cref="TabControl"/> using arrow keys.
/// </summary>
public partial class TabControlKeyNavigationBehavior : StyledElementBehavior<TabControl>
{

    /// <summary>
    /// Gets or sets the orientation used for navigation. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial Orientation Orientation { get; set; }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject?.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        AssociatedObject?.RemoveRoutedEventHandler(InputElement.KeyDownEvent, OnKeyDown);

        base.OnDetaching();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (!IsEnabled || AssociatedObject is null)
        {
            return;
        }

        if (Orientation == Orientation.Horizontal)
        {
            if (e.Key == Key.Right)
            {
                AssociatedObject.SelectedIndex = Math.Min(AssociatedObject.SelectedIndex + 1, AssociatedObject.ItemCount - 1);
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                AssociatedObject.SelectedIndex = Math.Max(AssociatedObject.SelectedIndex - 1, 0);
                e.Handled = true;
            }
        }
        else
        {
            if (e.Key == Key.Down)
            {
                AssociatedObject.SelectedIndex = Math.Min(AssociatedObject.SelectedIndex + 1, AssociatedObject.ItemCount - 1);
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                AssociatedObject.SelectedIndex = Math.Max(AssociatedObject.SelectedIndex - 1, 0);
                e.Handled = true;
            }
        }
    }
}
