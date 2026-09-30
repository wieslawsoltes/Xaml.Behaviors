// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Scrolls a <see cref="ScrollViewer"/> to the specified offsets when executed.
/// </summary>
public partial class ScrollToOffsetAction : StyledElementAction
{

    /// <summary>
    /// Gets or sets the <see cref="ScrollViewer"/> instance to scroll. This is an avalonia property.
    /// </summary>
    [StyledProperty(ResolveByName = true)]
    public partial ScrollViewer? ScrollViewer { get; set; }

    /// <summary>
    /// Gets or sets the horizontal offset to scroll to. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial double? HorizontalOffset { get; set; }

    /// <summary>
    /// Gets or sets the vertical offset to scroll to. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial double? VerticalOffset { get; set; }

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

#if UNO
        // WinUI scrolls through ChangeView; a null offset keeps the current one.
        scroller.ChangeView(HorizontalOffset, VerticalOffset, null, disableAnimation: true);
#else
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
#endif
        return true;
    }
}
