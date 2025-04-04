using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// 
/// </summary>
public class FocusAutoCompleteBoxTextBoxBehavior : AttachedToVisualTreeBehavior<AutoCompleteBox>
{
    /// <inheritdoc />
    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        if (AssociatedObject is null)
        {
            return DisposableAction.Empty;
        }

        AssociatedObject.GotFocus += AssociatedObjectOnGotFocus;

        return DisposableAction.Create(() => AssociatedObject.GotFocus -= AssociatedObjectOnGotFocus);
    }

    private void AssociatedObjectOnGotFocus(object? sender, GotFocusEventArgs e)
    {
        var textBox = AssociatedObject?.GetVisualDescendants().OfType<TextBox>().FirstOrDefault();
        Dispatcher.UIThread.Post(() => textBox?.Focus());
    }
}
