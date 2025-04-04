using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Reactive;
using Avalonia.Threading;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class FocusSelectedItemBehavior : AttachedToVisualTreeBehavior<ItemsControl>
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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
