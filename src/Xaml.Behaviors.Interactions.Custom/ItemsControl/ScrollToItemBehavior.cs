// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
#else
using Avalonia.Controls;
using Avalonia.Reactive;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

/// <summary>
/// Scrolls the associated <see cref="ItemsControl"/> to a specific item.
/// </summary>
public partial class ScrollToItemBehavior : AttachedToVisualTreeBehavior<ItemsControl>
{

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial IObservable<object>? Item { get; set; }

    /// <summary>
    /// Subscribes to the <see cref="Item"/> observable and scrolls to incoming values.
    /// </summary>
    /// <returns>A disposable that unsubscribes from <see cref="Item"/>.</returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        var disposable = Item?.Subscribe(new AnonymousObserver<object>(item =>
        {
            AssociatedObject?.ScrollIntoView(item);
        }));

        if (disposable is not null)
        {
            return disposable;
        }
        
        return DisposableAction.Empty;
    }
}
