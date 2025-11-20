using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// A behavior that loses focus when the Enter key is pressed.
/// </summary>
public class LoseFocusOnEnterBehavior : StyledElementBehavior<Control>
{
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not null)
        {
            AssociatedObject.KeyDown += OnKeyDown;
        }
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject is not null)
        {
            AssociatedObject.KeyDown -= OnKeyDown;
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var topLevel = TopLevel.GetTopLevel(AssociatedObject);
            topLevel?.FocusManager?.ClearFocus();
        }
    }
}
