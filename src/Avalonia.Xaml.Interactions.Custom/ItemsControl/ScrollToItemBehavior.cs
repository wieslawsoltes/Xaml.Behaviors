using System;
using Avalonia.Controls;
using Avalonia.Reactive;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class ScrollToItemBehavior : AttachedToVisualTreeBehavior<ItemsControl>
{
    /// <summary>
    /// 
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
    /// 
    /// </summary>
    /// <returns></returns>
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
