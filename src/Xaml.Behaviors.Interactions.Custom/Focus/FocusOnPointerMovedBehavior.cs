// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Focuses the <see cref="IBehavior.AssociatedObject"/> on <see cref="InputElement.PointerMoved"/> event.
/// </summary>
public class FocusOnPointerMovedBehavior : StyledElementBehavior<Control>
{
    /// <inheritdoc />
    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.PointerMoved += PointerMoved;
        }
    }

    /// <inheritdoc />
    protected override void OnDetachedFromVisualTree()
    {
        if (AssociatedObject is not null)
        {
            AssociatedObject.PointerMoved -= PointerMoved;
        }
    }

    private void PointerMoved(object? sender, PointerEventArgs args)
    {
        Dispatcher.UIThread.Post(() => AssociatedObject?.Focus());
    }
}
