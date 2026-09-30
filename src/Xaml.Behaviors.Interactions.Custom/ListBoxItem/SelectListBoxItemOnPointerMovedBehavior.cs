// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
using ListBoxItem = Microsoft.UI.Xaml.Controls.Primitives.SelectorItem;
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
/// Sets <see cref="ListBoxItem.IsSelected"/> property to true of the associated <see cref="ListBoxItem"/> control on <see cref="InputElement.PointerMoved"/> event.
/// </summary>
public class SelectListBoxItemOnPointerMovedBehavior : StyledElementBehavior<Control>
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
#if UNO
        // WinUI item templates are hosted by a content presenter: the container is the nearest selector item.
        if (AssociatedObject?.FindAncestorOfType<ListBoxItem>() is { } item)
#else
        if (AssociatedObject is {Parent: ListBoxItem item})
#endif
        {
            item.SetCurrentValue(ListBoxItem.IsSelectedProperty, true);
            Dispatcher.UIThread.Post(() => item.Focus());
        }
    }
}
