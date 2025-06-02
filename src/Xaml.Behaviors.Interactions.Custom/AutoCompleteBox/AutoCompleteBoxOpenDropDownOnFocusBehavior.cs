using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Opens the drop-down of an <see cref="AutoCompleteBox"/> when it receives focus.
/// </summary>
public class AutoCompleteBoxOpenDropDownOnFocusBehavior : AttachedToVisualTreeBehavior<AutoCompleteBox>
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
        if (AssociatedObject is null)
        {
            return;
        }

        AssociatedObject.IsDropDownOpen = true;
    }
}
