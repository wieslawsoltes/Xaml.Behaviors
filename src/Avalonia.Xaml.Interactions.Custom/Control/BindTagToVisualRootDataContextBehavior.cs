using System;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Binds AssociatedObject object Tag property to root visual DataContext.
/// </summary>
public class BindTagToVisualRootDataContextBehavior : DisposingBehavior<Control>
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected override IDisposable OnAttachedOverride()
    {
        var visualRoot = (Control?)AssociatedObject?.GetVisualRoot();
        if (visualRoot is not null)
        {
            return BindDataContextToTag(visualRoot, AssociatedObject);
        }

        return DisposableAction.Empty;
    }

    private static IDisposable BindDataContextToTag(Control source, Control? target)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (target is null)
        {
            throw new ArgumentNullException(nameof(target));
        }

        return target.Bind(
            Control.TagProperty, 
            source.GetObservable(StyledElement.DataContextProperty));
    }
}
