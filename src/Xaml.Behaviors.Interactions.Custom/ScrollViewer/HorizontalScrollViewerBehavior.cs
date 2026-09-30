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
/// Enables horizontal scrolling of a <see cref="ScrollViewer"/> using the mouse wheel.
/// </summary>
public partial class HorizontalScrollViewerBehavior : StyledElementBehavior<ScrollViewer>
{
    /// <summary>
    /// 
    /// </summary>
    public enum ChangeSize
    {
        /// <summary>
        /// Scrolls by a single line.
        /// </summary>
        Line,

        /// <summary>
        /// Scrolls by a full page.
        /// </summary>
        Page
    }

    /// <summary>
    /// Called when the behavior is attached to the associated object.
    /// </summary>
    [StyledProperty]
    public partial bool RequireShiftKey { get; set; }

    /// <summary>
    /// Called when the behavior is detached from the associated object.
    /// </summary>
    [StyledProperty]
    public partial ChangeSize ScrollChangeSize { get; set; }

    /// <summary>
    /// 
    /// </summary>
    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject!.AddHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged,
            RoutingStrategies.Tunnel);
    }

    /// <summary>
    /// 
    /// </summary>
    protected override void OnDetaching()
    {
        base.OnDetaching();

        AssociatedObject!.RemoveRoutedEventHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged);
    }

    /// <summary>
    /// Handles the pointer wheel changed event.
    /// </summary>
    /// <param name="sender">Event source.</param>
    /// <param name="e">Event arguments.</param>
    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (!IsEnabled)
        {
            e.Handled = true;
            return;
        }

        if (RequireShiftKey && e.KeyModifiers == KeyModifiers.Shift || !RequireShiftKey)
        {
#if UNO
            // WinUI reports the wheel rotation in the pointer point (positive when rotated away from the user).
            if (e.GetCurrentPoint(AssociatedObject).Properties.MouseWheelDelta < 0)
#else
            if (e.Delta.Y < 0)
#endif
            {
                if (ScrollChangeSize == ChangeSize.Line)
                {
                    AssociatedObject!.LineRight();
                }
                else
                {
                    AssociatedObject!.PageRight();
                }
            }
            else
            {
                if (ScrollChangeSize == ChangeSize.Line)
                {
                    AssociatedObject!.LineLeft();
                }
                else
                {
                    AssociatedObject!.PageLeft();
                }
            }
        }
    }
}
