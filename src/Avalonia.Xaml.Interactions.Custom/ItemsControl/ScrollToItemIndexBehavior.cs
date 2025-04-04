using System;
using Avalonia.Controls;
using Avalonia.Reactive;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class ScrollToItemIndexBehavior : AttachedToVisualTreeBehavior<ItemsControl>
{
    /// <summary>
    /// 
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
    /// 
    /// </summary>
    /// <returns></returns>
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
