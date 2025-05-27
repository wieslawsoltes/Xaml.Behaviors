// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Enables horizontal scrolling of a <see cref="ScrollViewer"/> using the mouse wheel.
/// </summary>
public class HorizontalScrollViewerBehavior : StyledElementBehavior<ScrollViewer>
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
    /// Gets or sets a value indicating whether the Shift key must be held while scrolling.
    /// </summary>
    public static readonly StyledProperty<bool> RequireShiftKeyProperty =
        AvaloniaProperty.Register<HorizontalScrollViewerBehavior, bool>(nameof(RequireShiftKey));

    /// <summary>
    /// Gets or sets the scroll amount used for each wheel delta.
    /// </summary>
    public static readonly StyledProperty<ChangeSize> ScrollChangeSizeProperty =
        AvaloniaProperty.Register<HorizontalScrollViewerBehavior, ChangeSize>(nameof(ScrollChangeSize));

    /// <summary>
    /// Called when the behavior is attached to the associated object.
    /// </summary>
    public bool RequireShiftKey
    {
        get => GetValue(RequireShiftKeyProperty);
        set => SetValue(RequireShiftKeyProperty, value);
    }

    /// <summary>
    /// Called when the behavior is detached from the associated object.
    /// </summary>
    public ChangeSize ScrollChangeSize
    {
        get => GetValue(ScrollChangeSizeProperty);
        set => SetValue(ScrollChangeSizeProperty, value);
    }

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

        AssociatedObject!.RemoveHandler(InputElement.PointerWheelChangedEvent, OnPointerWheelChanged);
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
            if (e.Delta.Y < 0)
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
