// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Scrolls a <see cref="ScrollViewer"/> to the specified offsets when executed.
/// </summary>
public class ScrollToOffsetAction : StyledElementAction
{
    /// <summary>
    /// Identifies the <see cref="ScrollViewer"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<ScrollViewer?> ScrollViewerProperty =
        AvaloniaProperty.Register<ScrollToOffsetAction, ScrollViewer?>(nameof(ScrollViewer));

    /// <summary>
    /// Identifies the <see cref="HorizontalOffset"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double?> HorizontalOffsetProperty =
        AvaloniaProperty.Register<ScrollToOffsetAction, double?>(nameof(HorizontalOffset));

    /// <summary>
    /// Identifies the <see cref="VerticalOffset"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<double?> VerticalOffsetProperty =
        AvaloniaProperty.Register<ScrollToOffsetAction, double?>(nameof(VerticalOffset));

    /// <summary>
    /// Gets or sets the <see cref="ScrollViewer"/> instance to scroll. This is an avalonia property.
    /// </summary>
    [ResolveByName]
    public ScrollViewer? ScrollViewer
    {
        get => GetValue(ScrollViewerProperty);
        set => SetValue(ScrollViewerProperty, value);
    }

    /// <summary>
    /// Gets or sets the horizontal offset to scroll to. This is an avalonia property.
    /// </summary>
    public double? HorizontalOffset
    {
        get => GetValue(HorizontalOffsetProperty);
        set => SetValue(HorizontalOffsetProperty, value);
    }

    /// <summary>
    /// Gets or sets the vertical offset to scroll to. This is an avalonia property.
    /// </summary>
    public double? VerticalOffset
    {
        get => GetValue(VerticalOffsetProperty);
        set => SetValue(VerticalOffsetProperty, value);
    }

    /// <inheritdoc />
    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var scroller = ScrollViewer ?? sender as ScrollViewer;
        if (scroller is null)
        {
            return false;
        }

        var offset = scroller.Offset;

        if (HorizontalOffset.HasValue)
        {
            offset = offset.WithX(HorizontalOffset.Value);
        }

        if (VerticalOffset.HasValue)
        {
            offset = offset.WithY(VerticalOffset.Value);
        }

        scroller.Offset = offset;
        return true;
    }
}
