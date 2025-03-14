using System.Reactive.Disposables;
using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class ListBoxUnselectAllBehavior : AttachedToVisualTreeBehavior<ListBox>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="disposable"></param>
    protected override void OnAttachedToVisualTree(CompositeDisposable disposable)
    {
        AssociatedObject?.UnselectAll();
    }
}
