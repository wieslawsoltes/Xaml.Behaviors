// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.
using System;
#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Xaml.Interactivity;
using ItemsControl = Microsoft.UI.Xaml.Controls.ListViewBase;
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
/// Scrolls the associated <see cref="ItemsControl"/> to a given item index.
/// </summary>
/// <remarks>
/// On Uno Platform the associated object is a WinUI <c>ListViewBase</c> (<c>ListView</c>, <c>GridView</c>).
/// </remarks>
public partial class ScrollToItemIndexBehavior : AttachedToVisualTreeBehavior<ItemsControl>
{

    /// <summary>
    /// 
    /// </summary>
    [StyledProperty]
    public partial IObservable<int>? ItemIndex { get; set; }

    /// <summary>
    /// Subscribes to the <see cref="ItemIndex"/> observable and scrolls to incoming indexes.
    /// </summary>
    /// <returns>A disposable that unsubscribes from <see cref="ItemIndex"/>.</returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        var disposable = ItemIndex?.Subscribe(new AnonymousObserver<int>(index =>
        {
#if UNO
            // WinUI scrolls to items: resolve the item at the index.
            if (AssociatedObject is { } itemsControl && index >= 0 && index < itemsControl.Items.Count)
            {
                itemsControl.ScrollIntoView(itemsControl.Items[index]);
            }
#else
            AssociatedObject?.ScrollIntoView(index);
#endif
        }));

        if (disposable is not null)
        {
            return disposable;
        }
        
        return DisposableAction.Empty;
    }
}
