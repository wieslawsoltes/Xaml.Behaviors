
namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class FocusOnAttachedBehavior : FocusBehaviorBase
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        Focus();

        return DisposableAction.Empty;
    }
}
