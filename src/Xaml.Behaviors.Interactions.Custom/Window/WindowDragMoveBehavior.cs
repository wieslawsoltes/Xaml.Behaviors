// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that allows the user to drag the window by clicking and holding the associated control.
/// </summary>
public class WindowDragMoveBehavior : Behavior<Control>
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        base.OnAttachedToVisualTree();
        if (AssociatedObject is not null)
        {
            AssociatedObject.PointerPressed += AssociatedObject_PointerPressed;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        base.OnDetachedFromVisualTree();
        if (AssociatedObject is not null)
        {
            AssociatedObject.PointerPressed -= AssociatedObject_PointerPressed;
        }
    }

    private void AssociatedObject_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (AssociatedObject is not null)
        {
            var window = Window.GetTopLevel(AssociatedObject) as Window;
            if (window is not null)
            {
                window.BeginMoveDrag(e);
            }
        }
    }
}
