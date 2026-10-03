// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
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
/// Changes the cursor when the pointer is over the associated control.
/// </summary>
public partial class PointerOverCursorBehavior : StyledElementBehavior<InputElement>
{

    /// <summary>
    /// Gets or sets the cursor to apply while the pointer is over the control.
    /// </summary>
    [StyledProperty]
    public partial Cursor? Cursor { get; set; }

    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.AddHandler(InputElement.PointerEnteredEvent, OnPointerEntered,
                RoutingStrategies.Direct);
            AssociatedObject.AddHandler(InputElement.PointerExitedEvent, OnPointerExited,
                RoutingStrategies.Direct);
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.RemoveRoutedEventHandler(InputElement.PointerEnteredEvent, OnPointerEntered);
            AssociatedObject.RemoveRoutedEventHandler(InputElement.PointerExitedEvent, OnPointerExited);
        }
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        if (AssociatedObject is not null && Cursor is not null)
        {
            AssociatedObject.SetCurrentValue(InputElement.CursorProperty, Cursor);
        }
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        AssociatedObject?.ClearValue(InputElement.CursorProperty);
    }
}
