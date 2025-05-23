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
    /// <returns></returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        AssociatedObject?.UnselectAll();

        return DisposableAction.Empty;
    }
}
