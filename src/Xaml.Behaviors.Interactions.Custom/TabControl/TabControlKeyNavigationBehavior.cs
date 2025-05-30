using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Xaml.Interactivity;

namespace Avalonia.Xaml.Interactions.Custom;

/// <summary>
/// Enables keyboard navigation for a <see cref="TabControl"/> using arrow keys.
/// </summary>
public class TabControlKeyNavigationBehavior : StyledElementBehavior<TabControl>
{
    /// <summary>
    /// Identifies the <seealso cref="Orientation"/> avalonia property.
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<TabControlKeyNavigationBehavior, Orientation>(nameof(Orientation), Orientation.Horizontal);

    /// <summary>
    /// Gets or sets the orientation used for navigation. This is an avalonia property.
    /// </summary>
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject?.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        AssociatedObject?.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);

        base.OnDetaching();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (!IsEnabled || AssociatedObject is null)
        {
            return;
        }

        if (Orientation == Orientation.Horizontal)
        {
            if (e.Key == Key.Right)
            {
                AssociatedObject.SelectedIndex = Math.Min(AssociatedObject.SelectedIndex + 1, AssociatedObject.ItemCount - 1);
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                AssociatedObject.SelectedIndex = Math.Max(AssociatedObject.SelectedIndex - 1, 0);
                e.Handled = true;
            }
        }
        else
        {
            if (e.Key == Key.Down)
            {
                AssociatedObject.SelectedIndex = Math.Min(AssociatedObject.SelectedIndex + 1, AssociatedObject.ItemCount - 1);
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                AssociatedObject.SelectedIndex = Math.Max(AssociatedObject.SelectedIndex - 1, 0);
                e.Handled = true;
            }
        }
    }
}
