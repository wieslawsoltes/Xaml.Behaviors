// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

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
                            if (container is not null)
                            {
                                container.Focus();
                            }
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
