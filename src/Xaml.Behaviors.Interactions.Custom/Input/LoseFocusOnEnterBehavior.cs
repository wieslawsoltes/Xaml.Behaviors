#if UNO
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Xaml.Interactivity;
#else
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
#endif

#if UNO
namespace Xaml.Interactions.Custom;
#else
namespace Avalonia.Xaml.Interactions.Custom;
#endif

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
#if UNO
            // WinUI cannot clear the focus: it moves to the root element of the XAML island when it is focusable.
            AssociatedObject?.XamlRoot?.Content?.Focus(FocusState.Programmatic);
#else
            var topLevel = TopLevel.GetTopLevel(AssociatedObject);
            topLevel?.FocusManager?.Focus(null, NavigationMethod.Unspecified, KeyModifiers.None);
#endif
        }
    }
}
