using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Hides the flyout when the button is clicked.
/// </summary>
public class ButtonHideFlyoutOnClickBehavior : AttachedToVisualTreeBehavior<Button>
{
    /// <inheritdoc />
    protected override System.IDisposable OnAttachedToVisualTreeOverride()
    {
        var button = AssociatedObject;

        if (button is null)
        {
            return DisposableAction.Empty;
        }

        var flyoutPresenter = button.FindAncestorOfType<FlyoutPresenter>();
        if (flyoutPresenter?.Parent is not Popup popup)
        {
            return DisposableAction.Empty;
        }

        button.Click += AssociatedObjectOnClick;

        return DisposableAction.Create(() =>
        {
            button.Click -= AssociatedObjectOnClick;
        });

        void AssociatedObjectOnClick(object? sender, RoutedEventArgs e)
        {
            // Execute Command if any before closing. Otherwise, it won't execute because Close will destroy the associated object before Click can execute it.
            if (button.Command != null && button.IsEnabled)
            {
                button.Command.Execute(button.CommandParameter);
            }

            popup.Close();
        }
    }
}
