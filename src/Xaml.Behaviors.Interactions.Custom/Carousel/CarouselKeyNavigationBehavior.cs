// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
using Carousel = Microsoft.UI.Xaml.Controls.FlipView;
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
/// Enables keyboard navigation for a <see cref="Carousel"/> using arrow keys.
/// </summary>
public partial class CarouselKeyNavigationBehavior : StyledElementBehavior<Carousel>
{

    /// <summary>
    /// Gets or sets the orientation used for navigation. This is an avalonia property.
    /// </summary>
    [StyledProperty(DefaultValue = Orientation.Horizontal)]
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
                AssociatedObject.Next();
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                AssociatedObject.Previous();
                e.Handled = true;
            }
        }
        else
        {
            if (e.Key == Key.Down)
            {
                AssociatedObject.Next();
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                AssociatedObject.Previous();
                e.Handled = true;
            }
        }
    }
}
