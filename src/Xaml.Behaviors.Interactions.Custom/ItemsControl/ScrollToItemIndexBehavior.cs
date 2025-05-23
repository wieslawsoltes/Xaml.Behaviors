using System;
using Avalonia.Controls;
using Avalonia.Reactive;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Scrolls the associated <see cref="ItemsControl"/> to a given item index.
/// </summary>
public class ScrollToItemIndexBehavior : AttachedToVisualTreeBehavior<ItemsControl>
{
    /// <summary>
    /// Gets or sets the observable that produces item indexes to scroll into view.
    /// </summary>
    public static readonly StyledProperty<IObservable<int>?> ItemIndexProperty =
        AvaloniaProperty.Register<ScrollToItemIndexBehavior, IObservable<int>?>(nameof(ItemIndex));

    /// <summary>
    /// 
    /// </summary>
    public IObservable<int>? ItemIndex
    {
        get => GetValue(ItemIndexProperty);
        set => SetValue(ItemIndexProperty, value);
    }

    /// <summary>
    /// Subscribes to the <see cref="ItemIndex"/> observable and scrolls to incoming indexes.
    /// </summary>
    /// <returns>A disposable that unsubscribes from <see cref="ItemIndex"/>.</returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        var disposable = ItemIndex?.Subscribe(new AnonymousObserver<int>(index =>
        {
            AssociatedObject?.ScrollIntoView(index);
        }));

        if (disposable is not null)
        {
            return disposable;
        }
        
        return DisposableAction.Empty;
    }
}
