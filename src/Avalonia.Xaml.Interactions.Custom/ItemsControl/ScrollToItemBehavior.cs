using System;
using Avalonia.Controls;
using Avalonia.Reactive;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Scrolls the associated <see cref="ItemsControl"/> to a specific item.
/// </summary>
public class ScrollToItemBehavior : AttachedToVisualTreeBehavior<ItemsControl>
{
    /// <summary>
    /// Gets or sets the observable that produces items to scroll into view.
    /// </summary>
    public static readonly StyledProperty<IObservable<object>?> ItemProperty =
        AvaloniaProperty.Register<ScrollToItemBehavior, IObservable<object>?>(nameof(Item));

    /// <summary>
    /// 
    /// </summary>
    public IObservable<object>? Item
    {
        get => GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

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
