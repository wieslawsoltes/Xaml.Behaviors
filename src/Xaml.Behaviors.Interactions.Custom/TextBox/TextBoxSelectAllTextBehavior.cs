using System;
using Avalonia.Controls;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class TextBoxSelectAllTextBehavior : AttachedToVisualTreeBehavior<TextBox>
{
    /// <summary>
    /// 
    /// </summary>
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        AssociatedObject?.SelectAll();

        return DisposableAction.Empty;
    }
}
