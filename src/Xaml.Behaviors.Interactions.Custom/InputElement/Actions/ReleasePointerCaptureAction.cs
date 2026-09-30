// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Releases the pointer capture.
/// </summary>
public class ReleasePointerCaptureAction : StyledElementAction
{
    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="sender">The <see cref="object"/> that is passed to the action by the behavior. Generally this is <seealso cref="IBehavior.AssociatedObject"/> or a target object.</param>
    /// <param name="parameter">The value of this parameter is determined by the caller.</param>
    /// <returns>Returns null after executed.</returns>
    public override object? Execute(object? sender, object? parameter)
    {
        if (parameter is not PointerEventArgs pointerEventArgs)
        {
            return null;
        }

#if UNO
        if (pointerEventArgs.OriginalSource is not UIElement source)
        {
            return null;
        }

        // WinUI releases a capture on the capturing element: the source or one of its ancestors.
        for (DependencyObject? current = source; current is not null; current = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(current))
        {
            if (current is UIElement element && element.PointerCaptures is { } captures)
            {
                foreach (var captured in captures)
                {
                    if (captured.PointerId == pointerEventArgs.Pointer.PointerId)
                    {
                        element.ReleasePointerCapture(pointerEventArgs.Pointer);
                        return null;
                    }
                }
            }
        }
#else
        if (pointerEventArgs.Source is not IInputElement)
        {
            return null;
        }

        pointerEventArgs.Pointer.Capture(control: null);
#endif

        return null;
    }
}
