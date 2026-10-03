// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Dispatching;
using Xaml.Interactivity;
using SelectingItemsControl = Microsoft.UI.Xaml.Controls.Primitives.Selector;
#else
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Focuses the container of the currently selected item when attached.
/// </summary>
public class FocusSelectedItemBehavior : AttachedToVisualTreeBehavior<ItemsControl>
{
    /// <summary>
    /// Subscribes to <see cref="SelectingItemsControl.SelectedItemProperty"/> and focuses
    /// the corresponding container when the selected item changes.
    /// </summary>
    /// <returns>A disposable used to clean up the subscription.</returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        var dispose = AssociatedObject?
            .GetObservable(SelectingItemsControl.SelectedItemProperty)
            .Subscribe(new AnonymousObserver<object?>(
                selectedItem =>
                {
                    var item = selectedItem;
                    if (item is not null)
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            var container = AssociatedObject.ContainerFromItem(item);
#if UNO
                            (container as UIElement)?.Focus();
#else
                            if (container is not null)
                            {
                                container.Focus();
                            }
#endif
                        });
                    }
                }));

        if (dispose is not null)
        {
            return dispose;
        }
        
        return DisposableAction.Empty;
    }
}
