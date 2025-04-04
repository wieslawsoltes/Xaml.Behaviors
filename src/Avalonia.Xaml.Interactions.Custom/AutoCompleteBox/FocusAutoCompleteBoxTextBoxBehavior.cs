using System.Linq;
using System.Reactive.Disposables;
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
    protected override void OnAttachedToVisualTree(CompositeDisposable disposable)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        AssociatedObject.GotFocus += AssociatedObjectOnGotFocus;

        Disposable.Create(() => AssociatedObject.GotFocus -= AssociatedObjectOnGotFocus);
    }

    private void AssociatedObjectOnGotFocus(object? sender, GotFocusEventArgs e)
    {
        var textBox = AssociatedObject?.GetVisualDescendants().OfType<TextBox>().FirstOrDefault();
        Dispatcher.UIThread.Post(() => textBox?.Focus());
    }
}
