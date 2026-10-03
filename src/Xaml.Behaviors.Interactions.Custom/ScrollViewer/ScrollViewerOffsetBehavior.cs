// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
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
/// Sets the <see cref="ScrollViewer.Offset"/> of the associated <see cref="ScrollViewer"/>.
/// </summary>
public partial class ScrollViewerOffsetBehavior : AttachedToVisualTreeBehavior<ScrollViewer>
{

    /// <summary>
    /// Gets or sets the horizontal offset value. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial double? HorizontalOffset { get; set; }

    /// <summary>
    /// Gets or sets the vertical offset value. This is an avalonia property.
    /// </summary>
    [StyledProperty]
    public partial double? VerticalOffset { get; set; }

    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        SetOffset();
        return DisposableAction.Empty;
    }

    /// <inheritdoc />
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == HorizontalOffsetProperty || change.Property == VerticalOffsetProperty)
        {
            SetOffset();
        }
    }

    private void SetOffset()
    {
        if (AssociatedObject is null)
        {
            return;
        }

#if UNO
        // WinUI scrolls through ChangeView; a null offset keeps the current one.
        AssociatedObject.ChangeView(HorizontalOffset, VerticalOffset, null, disableAnimation: true);
#else
        var offset = AssociatedObject.Offset;

        if (HorizontalOffset.HasValue)
        {
            offset = offset.WithX(HorizontalOffset.Value);
        }

        if (VerticalOffset.HasValue)
        {
            offset = offset.WithY(VerticalOffset.Value);
        }

        AssociatedObject.Offset = offset;
#endif
    }
}
