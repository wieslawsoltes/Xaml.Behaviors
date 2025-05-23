
namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class ListBoxSelectAllBehavior : AttachedToVisualTreeBehavior<Controls.ListBox>
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        AssociatedObject?.SelectAll();

        return DisposableAction.Empty;
    }
}
